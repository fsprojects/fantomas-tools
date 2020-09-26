namespace FantomasOnlineLatest.Server

open Fantomas
open Fantomas.FormatConfig
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =

    let private checker = Fantomas.Extras.FakeHelpers.sharedChecker.Force()

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
                    |> Option.defaultValue (box CharacterWidth))
            |> Seq.toArray

        let formatConfigType = typeof<FormatConfig.FormatConfig>
        Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig.FormatConfig


    let private format fileName code config =
        let options =
            Fantomas.Extras.FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.FormatDocumentAsync(fileName, source, config, options, checker)

    let private validate fileName code =
        let options =
            Fantomas.Extras.FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.IsValidFSharpCodeAsync(fileName, source, options, checker)

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

        sprintf "Master at %s" date

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        Http.main getFantomasVersion getOptions mapFantomasOptionsToRecord format validate log req
        |> Async.StartAsTask
