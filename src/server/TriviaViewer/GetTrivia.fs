module TriviaViewer.GetTrivia

open FSharp.Compiler.Syntax
open Fantomas.Core
open Fantomas.Core.AstTransformer
open Fantomas.Core.Trivia
open TriviaViewer.Shared
open TriviaViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private collectTriviaCandidates ast =
    let triviaNodesFromAST =
        match ast with
        | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (hashDirectives = hds; modules = mns)) ->
            astToNode hds mns

        | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (modules = mns)) -> sigAstToNode mns

    triviaNodesFromAST
    |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

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

        let triviaNodesFromAST, directives, codeComments =
            match ast with
            | ParsedInput.ImplFile (SourceParser.ParsedImplFileInput (hds, mns, directives, codeComments)) ->
                astToNode hds mns, directives, codeComments
            | ParsedInput.SigFile (SourceParser.ParsedSigFileInput (_, mns, directives, codeComments)) ->
                sigAstToNode mns, directives, codeComments

        let triviaNodes =
            triviaNodesFromAST
            |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

        let trivia =
            [ yield! collectTriviaFromDirectives source directives
              yield! collectTriviaFromCodeComments source codeComments
              yield! collectTriviaFromBlankLines source triviaNodes codeComments ]
            |> List.sortBy (fun n -> n.Range.Start.Line, n.Range.Start.Column)

        let triviaCandidates = collectTriviaCandidates ast
        let triviaNodes = collectTrivia source ast

        Encoders.encodeParseResult trivia triviaNodes triviaCandidates
        |> GetTriviaResponse.Ok

    | Error err -> GetTriviaResponse.BadRequest(string err)
