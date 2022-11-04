module TriviaViewer.GetTrivia

open FSharp.Compiler.Syntax
open Fantomas.Core
open Fantomas.Core.SourceParser
open Fantomas.Core.AstTransformer
open Fantomas.Core.Trivia
open Fantomas.Core.AstExtensions
open TriviaViewer.Shared
open TriviaViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private parseAST source defines isFsi = Fantomas.FCS.Parse.parseFile isFsi source defines

[<RequireQualifiedAccess>]
type GetTriviaResponse =
    | Ok of json: string
    | BadRequest of body: string

let getTrivia json : GetTriviaResponse =
    let parseRequest = Decoders.decodeParseRequest json

    match parseRequest with
    | Ok pr ->
        let { SourceCode = content
              Defines = defines
              IsFsi = isFsi } =
            pr

        let source = FSharp.Compiler.Text.SourceText.ofString content
        let ast, _diags = parseAST source (List.ofArray defines) isFsi

        let rootNode, directives, codeComments =
            match ast with
            | ParsedInput.ImplFile(ParsedImplFileInput(hds, mns, directives, codeComments)) ->
                let rootNode = astToNode ast.FullRange hds mns
                rootNode, directives, codeComments
            | ParsedInput.SigFile(ParsedSigFileInput(_, mns, directives, codeComments)) ->
                let rootNode = sigAstToNode ast.FullRange mns
                rootNode, directives, codeComments

        let trivia =
            let codeRange = ast.FullRange

            [ yield! collectTriviaFromDirectives source directives None
              yield! collectTriviaFromCodeComments source codeComments None
              yield!
                  collectTriviaFromBlankLines FormatConfig.FormatConfig.Default source rootNode codeComments codeRange ]
            |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

        let triviaInstructions =
            collectTrivia FormatConfig.FormatConfig.Default source ast None

        Encoders.encodeParseResult trivia rootNode triviaInstructions
        |> GetTriviaResponse.Ok

    | Error err -> GetTriviaResponse.BadRequest(string err)
