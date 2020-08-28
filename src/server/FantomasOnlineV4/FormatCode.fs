namespace FantomasOnlineLatest.Server

open Fantomas
open Fantomas.Extras
open FantomasOnline.Server.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =
    let private checker = FakeHelpers.sharedChecker.Force()

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
        Http.main CodeFormatter.GetVersion format validate FormatConfig.FormatConfig.Default log req
        |> Async.StartAsTask
