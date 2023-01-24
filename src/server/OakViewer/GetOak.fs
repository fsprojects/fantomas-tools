module OakViewer.GetOak

open FSharp.Compiler.Text
open Fantomas.Core
open OakViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private parseAST source defines isFsi = Fantomas.FCS.Parse.parseFile isFsi source defines

[<RequireQualifiedAccess>]
type GetOakResponse =
    | Ok of text: string
    | BadRequest of body: string

let getOak json : GetOakResponse =
    let parseRequest = Decoders.decodeParseRequest json

    match parseRequest with
    | Ok pr ->
        let { SourceCode = content
              Defines = defines
              IsFsi = isFsi } =
            pr

        let source = SourceText.ofString content
        let ast, _diags = parseAST source (List.ofArray defines) isFsi

        let oak =
            ASTTransformer.mkOak (Some source) ast
            |> Trivia.enrichTree FormatConfig.Default source ast

        let responseText = Encoders.encodeNode oak id |> Thoth.Json.Net.Encode.toString 4

        GetOakResponse.Ok responseText

    | Error err -> GetOakResponse.BadRequest(string err)
