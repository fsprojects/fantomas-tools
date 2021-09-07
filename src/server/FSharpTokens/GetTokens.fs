namespace FSharpTokens.Server

open Fantomas
open FSharpTokens.Shared
open System.IO
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open Thoth.Json.Net
open Microsoft.Extensions.Logging
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Azure.Functions.Worker
open FSharpTokens.Server.Decoders
open FSharpTokens.Server.Encoders

module GetTokens =
    let private sendJson json (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            res.StatusCode <- HttpStatusCode.OK
            do! res.WriteStringAsync(json)
            res.Headers.Add("Content-Type", "application/json")
            return res
        }

    let private sendText (text: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(text, System.Text.Encoding.UTF8)
            res.StatusCode <- HttpStatusCode.OK
            res.Headers.Add("Content-Type", "text/plain")
            return res
        }

    let private sendInternalError (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(error)
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.InternalServerError
            return res
        }

    let private sendTooLargeError (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync("File was too large")
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.RequestEntityTooLarge
            return res
        }

    let private sendBadRequest (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(error)
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.BadRequest
            return res
        }

    let private notFound (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            let json = Encode.string "Not found" |> Encode.toString 4
            do! res.WriteStringAsync(json)
            res.StatusCode <- HttpStatusCode.NotFound
            return res
        }

    let getTokens (log: ILogger) (req: HttpRequestData) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            use stream = new StreamReader(req.Body)
            let! content = stream.ReadToEndAsync() |> Async.AwaitTask

            let model = Decode.fromString decodeTokenRequest content

            match model with
            | Ok model ->
                let _, defineHashTokens = TokenParser.getDefines content

                let json =
                    TokenParser.tokenize model.Defines defineHashTokens model.SourceCode
                    |> toJson

                return! sendJson json res
            | Error err ->
                log.LogError($"Failed to decode: {err}")
                return! sendBadRequest err res
        }


    let getVersion (res: HttpResponseData) : Task<HttpResponseData> =
        let version =
            let assembly =
                typeof<FSharp.Compiler.CodeAnalysis.FSharpChecker>
                    .Assembly

            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        sendText version res

    [<Function "Tokens">]
    let run
        (
            [<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequestData,
            executionContext: FunctionContext
        )
        =
        let log: ILogger = executionContext.GetLogger("Tokens")
        log.LogInformation("F# HTTP trigger function processed a request..")
        let path = req.Url.LocalPath.ToLower()
        let method = req.Method.ToUpper()
        let res = req.CreateResponse()

        match method, path with
        | "POST", "/api/get-tokens" -> getTokens log req res
        | "GET", "/api/version" -> getVersion res
        | _ -> notFound res
