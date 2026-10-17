open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open SuaveExtensions
open FantomasOnline.Server.Shared.Http

let private serveLocally argv =
    let mapFormatResponseToWebPart (response: FormatResponse) : WebPart =
        match response with
        | FormatResponse.Ok body -> (applicationJson >=> OK body)
        | FormatResponse.BadRequest error -> (applicationText >=> BAD_REQUEST error)
        | FormatResponse.Failed error ->
            let statusCode, json = describeFailure error

            if statusCode = 400 then
                (applicationJson >=> BAD_REQUEST json)
            else
                (applicationJson >=> INTERNAL_SERVER_ERROR json)

    let formatWebPart =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let! formatResponse = FantomasOnlinePreview.FormatCode.formatCode json
                return! (mapFormatResponseToWebPart formatResponse) ctx
            }
        )

    let routes =
        [
            GET
            >=> path "/fantomas/preview/version"
            >=> textPlain
            >=> OK(FantomasOnlinePreview.FormatCode.getVersion ())
            GET
            >=> path "/fantomas/preview/options"
            >=> applicationJson
            >=> OK(FantomasOnlinePreview.FormatCode.getOptions ())
            POST >=> path "/fantomas/preview/format" >=> formatWebPart
        ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 12007us

    startFantomasTool port routes

    0

// Lambda sets AWS_LAMBDA_RUNTIME_API for the `bootstrap` it starts. Anywhere else this is the local
// server the Watch and Start pipelines run.
[<EntryPoint>]
let main argv =
    match Option.ofObj (System.Environment.GetEnvironmentVariable "AWS_LAMBDA_RUNTIME_API") with
    | None -> serveLocally argv
    | Some runtimeApi -> (LambdaRuntime.run runtimeApi FantomasOnlinePreview.Lambda.handlers).GetAwaiter().GetResult()
