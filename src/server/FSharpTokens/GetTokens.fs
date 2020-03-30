namespace FSharpTokens.Server

open Fantomas
open FSharpTokens.Shared
open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open FSharpTokens.Server.Decoders
open FSharpTokens.Server.Encoders

module GetTokens =
    let getTokens (req: HttpRequest) =
        let content = using (new StreamReader(req.Body)) (fun stream -> stream.ReadToEnd())
        let model = Decode.fromString decodeTokenRequest content
        match model with
        | Ok model ->
            let json =
                TokenParser.tokenize model.Defines model.SourceCode
                |> fst
                |> toJson
            new HttpResponseMessage(HttpStatusCode.OK,
                                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
        | Error err ->
            printfn "Failed to decode: %A" err
            new HttpResponseMessage(HttpStatusCode.BadRequest,
                                    Content = new StringContent(err, System.Text.Encoding.UTF8, "text/plain"))

    let getVersion() =
        let version =
            let assembly = typeof<FSharp.Compiler.SourceCodeServices.FSharpChecker>.Assembly
            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        let json =
            Encode.string version |> Encode.toString 4
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let notFound() =
        let json =
            Encode.string "Not found" |> Encode.toString 4
        new HttpResponseMessage(HttpStatusCode.NotFound,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    [<FunctionName("Tokens")>]
    let run([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req : HttpRequest)  (log: ILogger) =
        log.LogInformation ("F# HTTP trigger function processed a request..")
        let path = req.Path.Value.ToLower()
        let method = req.Method.ToUpper()

        match method, path with
        | "POST", "/api/get-tokens" -> getTokens req
        | "GET", "/api/version" -> getVersion()
        | _ -> notFound()