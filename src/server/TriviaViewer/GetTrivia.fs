module TriviaViewer.GetTrivia

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open Fantomas
open Fantomas.AstTransformer
open TriviaViewer.Shared
open TriviaViewer.Server

let private checker = FSharpChecker.Create(keepAssemblyContents = true)

let getVersion () : string =
    let assembly = typeof<FSharpChecker>.Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private collectTriviaCandidates tokens ast =
    let triviaNodesFromAST =
        match ast with
        | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (_, _, _, _, hds, mns, _)) -> astToNode hds mns

        | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (_, _, _, _, mns)) -> sigAstToNode mns

    let mkRange (sl, sc) (el, ec) =
        FSharp.Compiler.Text.Range.mkRange
            ast.Range.FileName
            (FSharp.Compiler.Text.Position.mkPos sl sc)
            (FSharp.Compiler.Text.Position.mkPos el ec)

    let triviaNodesFromTokens =
        TokenParser.getTriviaNodesFromTokens mkRange tokens

    triviaNodesFromAST @ triviaNodesFromTokens
    |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

let private parseAST source defines isFsi =
    let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"
    // create ISourceText
    let sourceText = FSharp.Compiler.Text.SourceText.ofString source

    async {
        // Get compiler options for a single script file
        let parsingOptions =
            { FSharpParsingOptions.Default with
                SourceFiles = [| fileName |]
                IsExe = true
                ConditionalCompilationDefines = Array.toList defines
                LangVersionText = "preview" }

        // Run the first phase (untyped parsing) of the compiler
        let untypedRes =
            checker.ParseFile(fileName, sourceText, parsingOptions)
            |> Async.RunSynchronously

        return Result.Ok(untypedRes.ParseTree, untypedRes.Diagnostics)
    }

[<RequireQualifiedAccess>]
type GetTriviaResponse =
    | Ok of json: string
    | BadRequest of body: string

let getTrivia json : Async<GetTriviaResponse> =
    async {
        let parseRequest = Decoders.decodeParseRequest json

        match parseRequest with
        | Ok pr ->
            let { SourceCode = content
                  Defines = defines
                  IsFsi = isFsi } =
                pr

            let _, defineHashTokens = TokenParser.getDefines content

            let tokens =
                TokenParser.tokenize (List.ofArray defines) defineHashTokens content

            let! astResult = parseAST content defines isFsi

            match astResult with
            | Result.Ok (ast, errors) when (Array.isEmpty errors) ->
                let mkRange (sl, sc) (el, ec) =
                    FSharp.Compiler.Text.Range.mkRange
                        ast.Range.FileName
                        (FSharp.Compiler.Text.Position.mkPos sl sc)
                        (FSharp.Compiler.Text.Position.mkPos el ec)

                let trivias = TokenParser.getTriviaFromTokens mkRange tokens
                let triviaCandidates = collectTriviaCandidates tokens ast
                let triviaNodes = Trivia.collectTrivia mkRange tokens ast

                let json =
                    Encoders.encodeParseResult trivias triviaNodes triviaCandidates
                return GetTriviaResponse.Ok json
            | Ok (_, errors) -> return GetTriviaResponse.BadRequest(Array.map string errors |> String.concat "\n")
            | Error err -> return GetTriviaResponse.BadRequest(string err)
        | Error err -> return GetTriviaResponse.BadRequest(string err)
    }
