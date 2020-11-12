namespace FantomasOnlineLatest.Server

open Fantomas
open Fantomas.Extras
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

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
                | MultilineFormatterTypeOption _ -> failwith "option not supported in this version")
            |> Seq.toArray

        let formatConfigType = typeof<FormatConfig.FormatConfig>
        Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig

    let private format fileName code config =
        let options = FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.FormatDocumentAsync(fileName, source, config, options, checker)

    let private validate fileName code =
        let options = FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.IsValidFSharpCodeAsync(fileName, source, options, checker)

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        Http.main CodeFormatter.GetVersion getOptions mapFantomasOptionsToRecord format validate log req
        |> Async.StartAsTask
