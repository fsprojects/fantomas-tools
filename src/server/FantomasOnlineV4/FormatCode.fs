module FantomasOnlineV4.FormatCode

open FSharp.Compiler.CodeAnalysis
open Fantomas
open Fantomas.FormatConfig
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open FantomasTools.Client

let private checker = CodeFormatterImpl.sharedChecker.Force()

let private mapFantomasOptionsToRecord options =
    let newValues =
        options
        |> Seq.map (function
            | BoolOption(_, _, v) -> box v
            | IntOption(_, _, v) -> box v
            | MultilineFormatterTypeOption(_, _, v) ->
                MultilineFormatterType.OfConfigString(v)
                |> Option.defaultValue (box CharacterWidth)
            | EndOfLineStyleOption(_, _, v) ->
                EndOfLineStyle.OfConfigString(v)
                |> Option.defaultValue EndOfLineStyle.CRLF
                |> box
            | MultilineBracketStyleOption _ -> failwith "Version 4 does not accept fsharp_multiline_bracket_style")
        |> Seq.toArray

    let formatConfigType = typeof<FormatConfig.FormatConfig>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig

let private format fileName code config =
    let options = CodeFormatterImpl.createParsingOptionsFromFile fileName
    let source = SourceOrigin.SourceString code
    CodeFormatter.FormatDocumentAsync(fileName, source, config, options, checker)

let private validate fileName code =
    let options =
        { FSharpParsingOptions.Default with
            SourceFiles = [| fileName |] }

    let sourceCode = FSharp.Compiler.Text.SourceText.ofString code

    async {
        let! result = checker.ParseFile(fileName, sourceCode, options)

        return
            result.Diagnostics
            |> Array.map (fun e ->
                { SubCategory = e.Subcategory
                  Range =
                    { StartLine = e.Range.StartLine
                      StartColumn = e.Range.StartColumn
                      EndLine = e.Range.EndLine
                      EndColumn = e.Range.EndColumn }
                  Severity = $"{e.Severity}".ToLower()
                  ErrorNumber = e.ErrorNumber
                  Message = e.Message }
                : Diagnostic)
            |> Array.toList
    }

let getVersion () =
    let assembly = typeof<FormatConfig.FormatConfig>.Assembly

    let version = assembly.GetName().Version
    $"%i{version.Major}.%i{version.Minor}.%i{version.Build}"

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
        | _ -> None)
    |> Seq.toList
    |> mapOptionsToJson

let formatCode: string -> Async<FormatResponse> =
    formatCode mapFantomasOptionsToRecord format validate
