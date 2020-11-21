namespace TriviaViewer.Server

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree
open System.Net
open System.Net.Http
open Fantomas
open Fantomas.AstTransformer
open Fantomas.TriviaTypes
open Thoth.Json.Net
open TriviaViewer.Shared
open TriviaViewer.Server


module GetTrivia =

    let private sendJson json =
        new HttpResponseMessage(
            HttpStatusCode.OK,
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        )

    let private sendText text =
        new HttpResponseMessage(
            HttpStatusCode.OK,
            Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text")
        )

    let private sendInternalError err =
        new HttpResponseMessage(
            HttpStatusCode.InternalServerError,
            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text")
        )

    let private sendBadRequest error =
        new HttpResponseMessage(
            HttpStatusCode.BadRequest,
            Content = new StringContent(error, System.Text.Encoding.UTF8, "application/text")
        )

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
                    |> Array.filter (fun e -> e.Severity = FSharpErrorSeverity.Error)

                if not <| Array.isEmpty errors
                then log.LogError(sprintf "Parsing failed with errors: %A\nAnd options: %A" errors checkOptions)

                return Error ast.Errors
            else
                match ast.ParseTree with
                | Some tree -> return Result.Ok tree
                | _ -> return Error Array.empty // Not sure this branch can be reached.
        }

    let private getVersion () =
        let version =
            let assembly = typeof<FSharpChecker>.Assembly
            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        sendText version

    let private notFound () =
        let json = Encode.string "Not found" |> Encode.toString 4

        new HttpResponseMessage(
            HttpStatusCode.NotFound,
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        )

    let private collectTriviaCandidates tokens ast =
        let node =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (_, _, _, _, hds, mns, _)) ->
                astToNode hds mns

            | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (_, _, _, _, mns)) -> sigAstToNode mns

        let rec flattenNodeToList (node: Node) =
            [ yield node
              yield!
                  (node.Childs
                   |> List.map flattenNodeToList
                   |> List.collect id) ]

        let mapNodeToTriviaNode (node: Node) =
            node.Range
            |> Option.map
                (fun range ->
                    let attributeParent =
                        Map.tryFind "linesBetweenParent" node.Properties
                        |> Option.bind
                            (fun v ->
                                match v with
                                | :? int as i when (i > 0) -> Some i
                                | _ -> None)

                    match attributeParent with
                    | Some i -> TriviaNodeAssigner(TriviaTypes.MainNode(node.Type), range, i)
                    | None -> TriviaNodeAssigner(TriviaTypes.MainNode(node.Type), range))

        let triviaNodesFromAST =
            flattenNodeToList node
            |> Trivia.filterNodes
            |> List.choose mapNodeToTriviaNode

        let triviaNodesFromTokens = TokenParser.getTriviaNodesFromTokens tokens

        triviaNodesFromAST @ triviaNodesFromTokens
        |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

    let private getTrivia (log: ILogger) (req: HttpRequest) =
        async {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
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
                    let trivias = TokenParser.getTriviaFromTokens tokens
                    let triviaCandidates = collectTriviaCandidates tokens ast
                    let triviaNodes = Trivia.collectTrivia tokens ast

                    let json =
                        Encoders.encodeParseResult trivias triviaNodes triviaCandidates

                    return sendJson json
                | Error err -> return sendBadRequest (sprintf "%A" err)
            | Error err -> return sendBadRequest (sprintf "%A" err)
        }

    [<FunctionName("GetTrivia")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")

            let path = req.Path.Value.ToLower()
            let method = req.Method.ToUpper()

            match method, path with
            | "POST", "/api/get-trivia" -> return! getTrivia log req
            | "GET", "/api/version" -> return getVersion ()
            | _ -> return notFound ()
        }
        |> Async.StartAsTask
