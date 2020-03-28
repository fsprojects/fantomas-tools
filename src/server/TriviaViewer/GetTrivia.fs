namespace TriviaViewer.Server

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.IO
open FSharp.Compiler.SourceCodeServices
open System.Net
open System.Net.Http
open Fantomas
open Fantomas.FormatConfig
open Thoth.Json.Net
open TriviaViewer.Shared
open TriviaViewer.Server


module GetTrivia =

    let private sendJson json =
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let private sendText text =
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text"))

    let private sendInternalError err =
        new HttpResponseMessage(HttpStatusCode.InternalServerError,
                                Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

    let private getProjectOptionsFromScript file source defines (checker: FSharpChecker) =
        async {
            let otherFlags =
                defines
                |> Seq.map (fun d -> sprintf "-d:%s" d)
                |> Seq.toArray

            let! (opts, _) = checker.GetProjectOptionsFromScript
                                 (file, source, otherFlags = otherFlags, assumeDotNetFramework = true)
            return opts
        }


    let private collectAST (log: ILogger) fileName defines source =
        async {
            let sourceText = FSharp.Compiler.Text.SourceText.ofString (source)
            let checker = FSharpChecker.Create(keepAssemblyContents = false)
            let! checkOptions = getProjectOptionsFromScript fileName sourceText defines checker
            let parsingOptions = checker.GetParsingOptionsFromProjectOptions(checkOptions) |> fst
            let! ast = checker.ParseFile(fileName, sourceText, parsingOptions)

            match ast.ParseTree with
            | Some tree -> return (Result.Ok tree)
            | None ->
                log.LogError
                    (sprintf "Error file getting project options:\nSource:\n%s\n\nErrors:\n%A" source ast.Errors)
                return Error ast.Errors
        }

    let private getVersion() =
        let version =
            let assembly = typeof<FSharp.Compiler.SourceCodeServices.FSharpChecker>.Assembly
            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        let json =
            Encode.string version |> Encode.toString 4
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let private notFound() =
        let json =
            Encode.string "Not found" |> Encode.toString 4
        new HttpResponseMessage(HttpStatusCode.NotFound,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let private getTrivia (log: ILogger) (req: HttpRequest) =
        async {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
            let parseRequest = Decoders.decodeParseRequest json

            match parseRequest with
            | Ok pr ->
                let { SourceCode = content; Defines = defines; FileName = fileName; KeepNewlineAfter = keepNewlineAfter } =
                    pr
                let (tokens, lineCount) = TokenParser.tokenize defines content
                let! astResult = collectAST log fileName defines content

                match astResult with
                | Result.Ok ast ->
                    let config = ({ FormatConfig.Default with KeepNewlineAfter = keepNewlineAfter })
                    let trivias = TokenParser.getTriviaFromTokens config tokens lineCount
                    let triviaNodes = Trivia.collectTrivia config tokens lineCount ast
                    let json = Encoders.encodeParseResult trivias triviaNodes
                    return sendJson json
                | Error err ->
                    return sendInternalError (sprintf "%A" err)
            | Error err ->
                return sendInternalError (sprintf "%A" err)
        }

    [<FunctionName("GetTrivia")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger) =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")

            let path = req.Path.Value.ToLower()
            let method = req.Method.ToUpper()

            match method, path with
            | "POST", "/api/get-trivia" -> return! getTrivia log req
            | "GET", "/api/version" -> return getVersion()
            | _ -> return notFound()
        }
        |> Async.StartAsTask