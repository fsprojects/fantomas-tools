module FantomasOnlineMain.FormatCode

open Fantomas.FCS.Text
open Fantomas.Core
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open FantomasTools.Client

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

    let formatConfigType = typeof<FormatConfig>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig

let private format (fileName: string) code config =
    let isSignature = fileName.EndsWith(".fsi")

    async {
        let! result = CodeFormatter.FormatDocumentAsync(isSignature, code, config)
        return result.Code
    }

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
                  StartColumn = 0
                  EndLine = 0
                  EndColumn = 0 }
            | Some r ->
                { StartLine = r.StartLine
                  StartColumn = r.StartColumn
                  EndLine = r.EndLine
                  EndColumn = r.EndColumn }

        { SubCategory = e.SubCategory
          Range = range
          Severity = $"{e.Severity}".ToLower()
          ErrorNumber = Option.defaultValue -1 e.ErrorNumber
          Message = e.Message }
        : Diagnostic)
    |> fun errors -> async { return errors }

let getVersion () =
    let date =
        let lastCommitInfo =
            sprintf
                "%s - %s"
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_TIMESTAMP"))
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_SHA"))

        if lastCommitInfo.Trim() <> "-" then
            lastCommitInfo
        else
            let assembly = typeof<FormatConfig>.Assembly

            System.IO.FileInfo assembly.Location
            |> fun f -> f.LastWriteTime.ToShortDateString()

    $"main branch at %s{date}"

let getOptions () : string =
    Reflection.getRecordFields FormatConfig.Default
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
