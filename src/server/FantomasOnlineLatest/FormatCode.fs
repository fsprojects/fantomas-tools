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
        let options = Fantomas.FakeHelpers.createParsingOptionsFromFile filename
        let source = SourceOrigin.SourceString code
        CodeFormatter.FormatDocumentAsync(filename, source, config, options, checker)

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger) =
        Http.main CodeFormatter.GetVersion format FormatConfig.FormatConfig.Default log req |> Async.StartAsTask
