namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =

    let private format filename code config =
        let checker = Fantomas.FakeHelpers.sharedChecker.Force()

        let options =
            Fantomas.FakeHelpers.createParsingOptionsFromFile filename

        let source = SourceOrigin.SourceString code
        CodeFormatter.FormatDocumentAsync(filename, source, config, options, checker)

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
        Http.main getFantomasVersion format FormatConfig.FormatConfig.Default log req
        |> Async.StartAsTask
