module OakViewer.GetOak

open Fantomas.Core
open OakViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    $"%i{version.Major}.%i{version.Minor}.%i{version.Revision}"

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

        let oakResult =
            try
                // TODO: at some point return the diagnostics as well to the client.
                let ast, _diagnostics =
                    Fantomas.FCS.Parse.parseFile
                        isFsi
                        (FSharp.Compiler.Text.SourceText.ofString content)
                        (List.ofArray defines)

                CodeFormatter.TransformAST(ast, content) |> Ok
            with ex ->
                Error ex

        match oakResult with
        | Error ex -> GetOakResponse.BadRequest(ex.Message)
        | Ok oak ->
            let responseText = Encoders.encodeNode oak id |> Thoth.Json.Net.Encode.toString 4
            GetOakResponse.Ok responseText

    | Error err -> GetOakResponse.BadRequest(string err)
