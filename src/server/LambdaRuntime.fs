/// The loop a lambda compiled with Native AOT runs in place of the managed .NET runtime.
///
/// On `provided.al2023` Lambda starts the `bootstrap` executable once per execution environment and
/// hands it one request at a time over the Lambda Runtime API, a small HTTP server at the address in
/// `AWS_LAMBDA_RUNTIME_API`: https://docs.aws.amazon.com/lambda/latest/dg/runtimes-api.html
///
/// The events are API Gateway proxy events, payload format 1.0. A handler only ever needs the
/// request body, so that is all this reads, and it writes the response JSON by hand. Both go through
/// Thoth rather than a serializer, which would need reflection or a C# source generator.
module LambdaRuntime

open System
open System.Net
open System.Net.Http
open System.Text
open System.Threading
open System.Threading.Tasks
open Thoth.Json.Core
open Thoth.Json.System.Text.Json
open HttpConstants

/// A handler takes the request body and answers with a status code, a content type and a body.
type Handler = string -> Task<HttpStatusCode * string * string>

let private decodeBody: Decoder<string> =
    Decode.object (fun get ->
        let body = get.Optional.Field "body" Decode.string |> Option.defaultValue ""

        let isBase64Encoded =
            get.Optional.Field "isBase64Encoded" Decode.bool |> Option.defaultValue false

        if isBase64Encoded then
            Encoding.UTF8.GetString(Convert.FromBase64String body)
        else
            body
    )

let private encodeResponse (statusCode: HttpStatusCode, contentType: string, body: string) : string =
    Encode.object
        [
            "statusCode", Encode.int (int statusCode)
            "headers", Encode.object [ "Content-Type", Encode.string contentType ]
            "body", Encode.string body
        ]
    |> LambdaJson.toString

let private encodeError (errorType: string) (message: string) : string =
    Encode.object [ "errorType", Encode.string errorType; "errorMessage", Encode.string message ]
    |> LambdaJson.toString

/// Lambda passes on at most 6 MB from a function, and fails the invocation when it is sent more.
let private maxResponseBytes = 6 * 1024 * 1024

/// The response for an answer, or a 413 that says why when the response is too large for Lambda: the
/// Oak of a long file runs past 6 MB, and a 413 is something the client can show.
let private toResponse (answer: HttpStatusCode * string * string) : string =
    let response = encodeResponse answer
    let size = Encoding.UTF8.GetByteCount response

    if size <= maxResponseBytes then
        response
    else
        let megabytes =
            (float size / 1024.0 / 1024.0).ToString("0.0", Globalization.CultureInfo.InvariantCulture)

        encodeResponse (
            HttpStatusCode.RequestEntityTooLarge,
            HeaderValues.TextPlain,
            "The answer is "
            + megabytes
            + " MB, and Lambda passes on at most 6 MB. Try less code."
        )

/// The error types follow the `Category.Reason` form the Runtime API asks for.
let private invoke (handle: Handler) (event: string) : Task<Result<string, string * string>> =
    task {
        try
            match Decode.fromString decodeBody event with
            | Error error -> return Error("Function.InvalidEvent", error)
            | Ok body ->
                let! answer = handle body
                return Ok(toResponse answer)
        with
        // The Oak of a long file does not fit in the memory the function has. What the request
        // allocated is free again once it fails, so this answers it rather than the invocation
        // failing with nothing for the client to show.
        | :? OutOfMemoryException ->
            return
                Ok(
                    encodeResponse (
                        HttpStatusCode.RequestEntityTooLarge,
                        HeaderValues.TextPlain,
                        "The answer takes more memory than this function has. Try less code."
                    )
                )
        | ex -> return Error("Function." + ex.GetType().Name, ex.Message)
    }

type private Invocation =
    {
        RequestId: string
        /// Echoed back on `/response` and `/error`, so Lambda can tell this attempt from a retry.
        InvocationId: string option
        /// The X-Ray trace of the invocation, which the X-Ray SDK reads from `_X_AMZN_TRACE_ID`.
        TraceId: string option
        Event: string
    }

let private isSuccess (status: HttpStatusCode) : bool = int status >= 200 && int status < 300

/// Whether the loop can go on after the Runtime API answered `status`. A 500, or no answer at all,
/// is "Container error. Non-recoverable state. Runtime should exit promptly."
let private canGoOn (status: HttpStatusCode option) : bool =
    match status with
    | Some status -> status <> HttpStatusCode.InternalServerError
    | None -> false

/// Serves requests until Lambda stops the process, or until the Runtime API says it cannot go on,
/// which ends the process with 1. `handlers` names each handler by the `Handler` its function is
/// deployed with in infrastructure/Program.fs, which Lambda passes on in `_HANDLER`.
let run (runtimeApi: string) (handlers: (string * Handler) list) : Task<int> =
    task {
        let runtimeUrl = "http://" + runtimeApi + "/2018-06-01/runtime/"
        // The request for the next invocation waits for as long as there is none.
        use http = new HttpClient(Timeout = Timeout.InfiniteTimeSpan)

        let post (path: string) (headers: (string * string) list) (json: string) : Task<HttpStatusCode option> =
            task {
                try
                    use request = new HttpRequestMessage(HttpMethod.Post, runtimeUrl + path)
                    request.Content <- new StringContent(json, Encoding.UTF8, "application/json")

                    for name, value in headers do
                        request.Headers.TryAddWithoutValidation(name, value) |> ignore

                    use! response = http.SendAsync request

                    if not (isSuccess response.StatusCode) then
                        Console.Error.WriteLine(
                            "The Runtime API answered "
                            + path
                            + " with "
                            + string<int>(int response.StatusCode)
                        )

                    return Some response.StatusCode
                with ex ->
                    Console.Error.WriteLine("The Runtime API did not take " + path + ": " + ex.Message)
                    return None
            }

        let nextInvocation () : Task<Result<Invocation, string>> =
            task {
                try
                    use! response = http.GetAsync(runtimeUrl + "invocation/next")

                    let header (name: string) : string option =
                        match response.Headers.TryGetValues name with
                        | true, values -> Seq.tryHead values
                        | false, _ -> None

                    if not (isSuccess response.StatusCode) then
                        return Error("invocation/next answered " + string<int>(int response.StatusCode))
                    else

                    match header "Lambda-Runtime-Aws-Request-Id" with
                    | None -> return Error "invocation/next answered without a request id"
                    | Some requestId ->
                        let! event = response.Content.ReadAsStringAsync()

                        return
                            Ok
                                {
                                    RequestId = requestId
                                    InvocationId = header "Lambda-Runtime-Invocation-Id"
                                    TraceId = header "Lambda-Runtime-Trace-Id"
                                    Event = event
                                }
                with ex ->
                    return Error("invocation/next failed: " + ex.Message)
            }

        /// Answers one invocation, and says whether the loop can go on.
        let serve (handle: Handler) (invocation: Invocation) : Task<bool> =
            task {
                let path = "invocation/" + invocation.RequestId

                let headers =
                    match invocation.InvocationId with
                    | Some invocationId -> [ "Lambda-Runtime-Invocation-Id", invocationId ]
                    | None -> []

                let fail (errorType: string) (message: string) : Task<bool> =
                    task {
                        let! status =
                            post
                                (path + "/error")
                                (("Lambda-Runtime-Function-Error-Type", errorType) :: headers)
                                (encodeError errorType message)

                        return canGoOn status
                    }

                // The runtime API asks a runtime to pass the trace on this way, invocation by invocation.
                Environment.SetEnvironmentVariable("_X_AMZN_TRACE_ID", Option.toObj invocation.TraceId)

                match! invoke handle invocation.Event with
                | Error(errorType, message) -> return! fail errorType message
                | Ok response ->
                    let! status = post (path + "/response") headers response

                    match status with
                    | Some status when isSuccess status -> return true
                    // Refused, so the invocation is still open: fail it with the reason rather than
                    // leave it to time out.
                    | Some status when canGoOn (Some status) ->
                        return!
                            fail
                                "Runtime.ResponseRejected"
                                ("Lambda refused the response with " + string<int>(int status))
                    | _ -> return false
            }

        let handlerName = Environment.GetEnvironmentVariable "_HANDLER"

        match handlers |> List.tryFind (fun (name, _) -> name = handlerName) with
        | None ->
            let errorType = "Runtime.NoSuchHandler"

            let! _ =
                post
                    "init/error"
                    [ "Lambda-Runtime-Function-Error-Type", errorType ]
                    (encodeError errorType ("No handler is named " + handlerName))

            return 1
        | Some(_, handle) ->
            let mutable goOn = true

            while goOn do
                match! nextInvocation () with
                | Error message ->
                    Console.Error.WriteLine message
                    goOn <- false
                | Ok invocation ->
                    let! served = serve handle invocation
                    goOn <- served

            return 1
    }
