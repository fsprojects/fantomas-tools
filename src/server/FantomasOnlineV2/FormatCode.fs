namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =
    let private checker = CodeFormatterImpl.sharedChecker.Value

    let private version () =
        let assembly = typeof<FormatConfig.FormatConfig>.Assembly

        let version = assembly.GetName().Version
        sprintf "%i.%i.%i" version.Major version.Minor version.Build

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
        async { return CodeFormatter.FormatDocument(fileName, code, config) }

    let private validate fileName code =
        let options =
            { Microsoft.FSharp.Compiler.SourceCodeServices.FSharpParsingOptions.Default with
                  SourceFiles = [| fileName |] }

        CodeFormatter.IsValidFSharpCodeAsync(fileName, code, options, checker)

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        Http.main version getOptions mapFantomasOptionsToRecord format validate log req
        |> Async.StartAsTask
