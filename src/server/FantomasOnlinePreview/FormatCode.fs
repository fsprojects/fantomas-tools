namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =

    let private checker = Fantomas.FakeHelpers.sharedChecker.Force()

    let private format fileName code config =
        let options =
            Fantomas.FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.FormatDocumentAsync(fileName, source, config, options, checker)

    let private validate fileName code =
        let options =
            Fantomas.FakeHelpers.createParsingOptionsFromFile fileName

        let source = SourceOrigin.SourceString code
        CodeFormatter.IsValidFSharpCodeAsync(fileName, source, options, checker)

    let getFantomasVersion () =
        let assembly =
            typeof<Fantomas.FormatConfig.FormatConfig>.Assembly

        let version = CodeFormatter.GetVersion()

        let date =
            System.IO.FileInfo assembly.Location
            |> fun f -> f.LastWriteTime.ToShortDateString()

        sprintf "Next - %s-%s" version date

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        Http.main getFantomasVersion format validate FormatConfig.FormatConfig.Default log req
        |> Async.StartAsTask
