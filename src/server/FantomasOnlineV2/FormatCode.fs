module FantomasOnlineV2.FormatCode

open Fantomas
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open Microsoft.FSharp.Compiler.SourceCodeServices

let private checker = CodeFormatterImpl.sharedChecker.Value

let private mapFantomasOptionsToRecord options =
    let newValues =
        options
        |> Seq.map (function
            | BoolOption (_, _, v) -> box v
            | IntOption (_, _, v) -> box v
            | _ -> failwith "option not supported in this version")
        |> Seq.toArray

    let formatConfigType = typeof<FormatConfig.FormatConfig>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig

let private format fileName code config =
    async { return CodeFormatter.FormatDocument(fileName, code, config) }

let private validate fileName code =
    let options =
        { FSharpParsingOptions.Default with SourceFiles = [| fileName |] }

    async {
        let! result = checker.ParseFile(fileName, code, options)

        return
            result.Errors
            |> Array.map (fun e ->
                { SubCategory = e.Subcategory
                  Range =
                    { StartLine = e.StartLineAlternate
                      StartCol = e.StartColumn
                      EndLine = e.EndLineAlternate
                      EndCol = e.EndColumn }
                  Severity =
                    match e.Severity with
                    | FSharpErrorSeverity.Warning -> ASTErrorSeverity.Warning
                    | FSharpErrorSeverity.Error -> ASTErrorSeverity.Error
                  ErrorNumber = e.ErrorNumber
                  Message = e.Message })
            |> Array.toList
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
        | _ -> None)
    |> Seq.toList
    |> mapOptionsToJson

let formatCode: string -> Async<FormatResponse> =
    formatCode mapFantomasOptionsToRecord format validate
