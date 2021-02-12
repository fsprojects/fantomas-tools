namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net
open FSharp.Compiler.SourceCodeServices

module FormatCode =
    let private checker = FakeHelpers.sharedChecker.Force()

    let private getOptions () =
        FantomasOnline.Server.Shared.Http.Reflection.getRecordFields FormatConfig.FormatConfig.Default
        |> Seq.indexed
        |> Seq.choose
            (fun (idx, (k: string, v: obj)) ->
                match v with
                | :? int as i -> FantomasOption.IntOption(idx, k, i) |> Some
                | :? bool as b -> FantomasOption.BoolOption(idx, k, b) |> Some
                | _ -> None)
        |> Seq.toList

    let private mapFantomasOptionsToRecord options =
        let newValues =
            options
            |> Seq.map
                (function
                | BoolOption (_, _, v) -> box v
                | IntOption (_, _, v) -> box v
                | _ -> failwith "option not supported in this version")
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
            let errors = 
                result.Errors 
                |> Array.choose (fun e -> match e.Severity with FSharpErrorSeverity.Error -> Some e.Message | _ -> None)
            let warnings =
                result.Errors
                |> Array.choose (fun e -> match e.Severity with FSharpErrorSeverity.Warning -> Some e.Message | _ -> None)
            
            if not (Array.isEmpty errors) then
                return FormatResult.Errors errors
            elif not (Array.isEmpty warnings) then
                return FormatResult.Warnings warnings
            else
                return FormatResult.Valid
        }

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        Http.main CodeFormatter.GetVersion getOptions mapFantomasOptionsToRecord format validate log req
        |> Async.StartAsTask
