namespace FantomasOnlineLatest.Server

open Fantomas
open Fantomas.FormatConfig
open Fantomas.Extras
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open System.Net
open FSharp.Compiler.SourceCodeServices

module FormatCode =
    let private checker = FakeHelpers.sharedChecker.Force()

    let private getOptions () =
        FantomasOnline.Server.Shared.Http.Reflection.getRecordFields FormatConfig.FormatConfig.Default
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

    let private mapFantomasOptionsToRecord options =
        let newValues =
            options
            |> Seq.map (function
                | BoolOption (_, _, v) -> box v
                | IntOption (_, _, v) -> box v
                | MultilineFormatterTypeOption (_, _, v) ->
                    MultilineFormatterType.OfConfigString(v)
                    |> Option.defaultValue (box CharacterWidth)
                | EndOfLineStyleOption (_, _, v) ->
                    EndOfLineStyle.OfConfigString(v)
                    |> Option.defaultValue EndOfLineStyle.CRLF
                    |> box)
            |> Seq.toArray

        let formatConfigType = typeof<FormatConfig.FormatConfig>
        Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig

    let private format fileName code config =
        let options = FakeHelpers.createParsingOptionsFromFile fileName

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
                result.Errors
                |> Array.map (fun e ->
                    { SubCategory = e.Subcategory
                      Range =
                          { StartLine = e.Range.StartLine
                            StartCol = e.Range.StartColumn
                            EndLine = e.Range.EndLine
                            EndCol = e.Range.EndColumn }
                      Severity =
                          match e.Severity with
                          | FSharpDiagnosticSeverity.Warning -> ASTErrorSeverity.Warning
                          | FSharpDiagnosticSeverity.Error -> ASTErrorSeverity.Error
                          | FSharpDiagnosticSeverity.Hidden -> ASTErrorSeverity.Hidden
                          | FSharpDiagnosticSeverity.Info -> ASTErrorSeverity.Info
                      ErrorNumber = e.ErrorNumber
                      Message = e.Message })
                |> Array.toList
        }

    [<Function "FormatCode">]
    let run
        (
            [<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequestData,
            executionContext: FunctionContext
        )
        =
        let log: ILogger = executionContext.GetLogger("FormatCode")
        Http.main CodeFormatter.GetVersion getOptions mapFantomasOptionsToRecord format validate log req
