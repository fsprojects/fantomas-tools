module FantomasOnlinePreview.FormatCode

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
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
    let isSignature = fileName.EndsWith(".fsi")
    let sourceCode = SourceText.ofString code
    let _, diagnostics = Fantomas.FCS.Parse.parseFile isSignature sourceCode []

    diagnostics
    |> List.map (fun e ->
        let range =
            match e.Range with
            | None ->
                { StartLine = 0
                  StartCol = 0
                  EndLine = 0
                  EndCol = 0 }
            | Some r ->
                { StartLine = r.StartLine
                  StartCol = r.StartColumn
                  EndLine = r.EndLine
                  EndCol = r.EndColumn }

        { SubCategory = e.SubCategory
          Range = range
          Severity =
            match e.Severity with
            | FSharpDiagnosticSeverity.Warning -> ASTErrorSeverity.Warning
            | FSharpDiagnosticSeverity.Error -> ASTErrorSeverity.Error
            | FSharpDiagnosticSeverity.Info -> ASTErrorSeverity.Info
            | FSharpDiagnosticSeverity.Hidden -> ASTErrorSeverity.Hidden
          ErrorNumber = Option.defaultValue -1 e.ErrorNumber
          Message = e.Message })
    |> fun errors -> async { return errors }

let getFantomasVersion () =
    let date =
        let lastCommitInfo =
            sprintf
                "%s - %s"
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_TIMESTAMP"))
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_SHA"))

        if lastCommitInfo.Trim() <> "-" then
            lastCommitInfo
        else
            let assembly = typeof<FormatConfig.FormatConfig>.Assembly

            System.IO.FileInfo assembly.Location
            |> fun f -> f.LastWriteTime.ToShortDateString()

    $"main branch at %s{date}"

let getVersion = getFantomasVersion

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
