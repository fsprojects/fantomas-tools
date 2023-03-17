module FantomasOnlineV5.FormatCode

open FSharp.Compiler.Diagnostics
open Fantomas.FCS.Parse
open Fantomas.Core
open Fantomas.Core.FormatConfig
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http

let private mapFantomasOptionsToRecord options =
    let newValues =
        options
        |> Seq.map (function
            | BoolOption(_, _, v) -> box v
            | IntOption(_, _, v) -> box v
            | MultilineFormatterTypeOption(_, _, v) ->
                MultilineFormatterType.OfConfigString(v)
                |> Option.defaultValue CharacterWidth
                |> box
            | EndOfLineStyleOption(_, _, v) ->
                EndOfLineStyle.OfConfigString(v)
                |> Option.defaultValue EndOfLineStyle.CRLF
                |> box
            | MultilineBracketStyleOption(_, _, v) ->
                MultilineBracketStyle.OfConfigString(v)
                |> Option.defaultValue MultilineBracketStyle.Cramped
                |> box)
        |> Seq.toArray

    let formatConfigType = typeof<FormatConfig.FormatConfig>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig

let private format (fileName: string) code config =
    let isSignature = fileName.EndsWith(".fsi")
    CodeFormatter.FormatDocumentAsync(isSignature, code, config)

let private validate (fileName: string) code =
    async {
        let isSignature = fileName.EndsWith(".fsi")

        let _tree, diagnostics =
            parseFile isSignature (FSharp.Compiler.Text.SourceText.ofString code) []

        return
            diagnostics
            |> List.map (fun (e: FSharpParserDiagnostic) ->
                let orZero f = Option.map f e.Range |> Option.defaultValue 0

                { SubCategory = e.SubCategory
                  Range =
                    { StartLine = orZero (fun r -> r.StartLine)
                      StartCol = orZero (fun r -> r.StartColumn)
                      EndLine = orZero (fun r -> r.EndLine)
                      EndCol = orZero (fun r -> r.EndColumn) }
                  Severity =
                    match e.Severity with
                    | FSharpDiagnosticSeverity.Warning -> ASTErrorSeverity.Warning
                    | FSharpDiagnosticSeverity.Error -> ASTErrorSeverity.Error
                    | FSharpDiagnosticSeverity.Hidden -> ASTErrorSeverity.Hidden
                    | FSharpDiagnosticSeverity.Info -> ASTErrorSeverity.Info
                  ErrorNumber = Option.defaultValue 0 e.ErrorNumber
                  Message = e.Message })
    }

let getVersion () =
    let assembly = typeof<FormatConfig.FormatConfig>.Assembly

    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Build

let getOptions () : string =
    Reflection.getRecordFields FormatConfig.FormatConfig.Default
    |> Seq.indexed
    |> Seq.choose (fun (idx, (k: string, v: obj)) ->
        match v with
        | :? int as i -> FantomasOption.IntOption(idx, k, i) |> Some
        | :? bool as b -> FantomasOption.BoolOption(idx, k, b) |> Some
        | :? MultilineFormatterType as mft ->
            FantomasOption.MultilineFormatterTypeOption(idx, k, (MultilineFormatterType.ToConfigString mft))
            |> Some
        | :? EndOfLineStyle as eol ->
            FantomasOption.EndOfLineStyleOption(idx, k, (EndOfLineStyle.ToConfigString eol))
            |> Some
        | :? MultilineBracketStyle as mbs ->
            FantomasOption.MultilineBracketStyleOption(idx, k, (MultilineBracketStyle.ToConfigString mbs))
            |> Some
        | _ -> None)
    |> Seq.toList
    |> mapOptionsToJson

let formatCode: string -> Async<FormatResponse> =
    formatCode mapFantomasOptionsToRecord format validate
