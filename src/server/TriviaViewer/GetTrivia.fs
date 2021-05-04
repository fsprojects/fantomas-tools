namespace TriviaViewer.Server

open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree
open System.Net
open System.Threading.Tasks
open FSharp.Control.Tasks
open Fantomas
open Fantomas.AstTransformer
open Thoth.Json.Net
open TriviaViewer.Shared
open TriviaViewer.Server

module GetTrivia =
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

    let private getProjectOptionsFromScript file source defines (checker: FSharpChecker) =
        async {
            let otherFlags =
                defines
                |> Seq.map (sprintf "-d:%s")
                |> Seq.toArray

            let! (opts, _) =
                checker.GetProjectOptionsFromScript(file, source, otherFlags = otherFlags, assumeDotNetFramework = true)

            return opts
        }

    let private collectAST (log: ILogger) fileName defines source =
        async {
            let sourceText = FSharp.Compiler.Text.SourceText.ofString (source)

            let checker = FSharpChecker.Create(keepAssemblyContents = false)

            let! checkOptions = getProjectOptionsFromScript fileName sourceText defines checker

            let parsingOptions =
                checker.GetParsingOptionsFromProjectOptions(checkOptions)
                |> fst

            let! ast = checker.ParseFile(fileName, sourceText, parsingOptions)

            if ast.ParseHadErrors then
                let errors =
                    ast.Errors
                    |> Array.filter (fun e -> e.Severity = FSharpDiagnosticSeverity.Error)

                if not <| Array.isEmpty errors then
                    log.LogError(sprintf "Parsing failed with errors: %A\nAnd options: %A" errors checkOptions)

                return Error ast.Errors
            else
                match ast.ParseTree with
                | Some tree -> return Result.Ok tree
                | _ -> return Error Array.empty // Not sure this branch can be reached.
        }

    let private getVersion (res: HttpResponseData) : Task<HttpResponseData> =
        let version =
            let assembly = typeof<FSharpChecker>.Assembly
            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        sendText version res

    let private collectTriviaCandidates tokens ast =
        let triviaNodesFromAST =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (_, _, _, _, hds, mns, _)) ->
                astToNode hds mns

            | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (_, _, _, _, mns)) -> sigAstToNode mns

        let mkRange (sl, sc) (el, ec) =
            FSharp.Compiler.Text.Range.mkRange
                ast.Range.FileName
                (FSharp.Compiler.Text.Pos.mkPos sl sc)
                (FSharp.Compiler.Text.Pos.mkPos el ec)

        let triviaNodesFromTokens =
            TokenParser.getTriviaNodesFromTokens mkRange tokens

        triviaNodesFromAST @ triviaNodesFromTokens
        |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

    let private getTrivia (log: ILogger) (req: HttpRequestData) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync()
            let parseRequest = Decoders.decodeParseRequest json

            match parseRequest with
            | Ok pr ->
                let { SourceCode = content
                      Defines = defines
                      FileName = fileName } =
                    pr

                let _, defineHashTokens = TokenParser.getDefines content

                let tokens =
                    TokenParser.tokenize defines defineHashTokens content

                let! astResult = collectAST log fileName defines content

                match astResult with
                | Result.Ok ast ->
                    let mkRange (sl, sc) (el, ec) =
                        FSharp.Compiler.Text.Range.mkRange
                            ast.Range.FileName
                            (FSharp.Compiler.Text.Pos.mkPos sl sc)
                            (FSharp.Compiler.Text.Pos.mkPos el ec)

                    let trivias = TokenParser.getTriviaFromTokens mkRange tokens
                    let triviaCandidates = collectTriviaCandidates tokens ast
                    let triviaNodes = Trivia.collectTrivia mkRange tokens ast

                    let json =
                        Encoders.encodeParseResult trivias triviaNodes triviaCandidates

                    return! sendJson json res
                | Error err -> return! sendBadRequest (sprintf "%A" err) res
            | Error err -> return! sendBadRequest (sprintf "%A" err) res
        }

    [<Function "GetTrivia">]
    let run
        (
            [<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequestData,
            executionContext: FunctionContext
        )
        =
        let log : ILogger = executionContext.GetLogger("GetTrivia")
        log.LogInformation("F# HTTP trigger function processed a request.")
        let path = req.Url.LocalPath.ToLower()
        let method = req.Method.ToUpper()
        let res = req.CreateResponse()

        match method, path with
        | "POST", "/api/get-trivia" -> getTrivia log req res
        | "GET", "/api/version" -> getVersion res
        | _ -> notFound res
