namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =

    let version () =
        let assembly = typeof<Fantomas.FormatConfig.FormatConfig>.Assembly
        let version = assembly.GetName().Version
        sprintf "%i.%i.%i" version.Major version.Minor version.Build

    let format fileName code config =
        async { return CodeFormatter.FormatDocument(fileName, code, config) }

    [<FunctionName("FormatCode")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger) =
            Http.main version format FormatConfig.FormatConfig.Default log req
            |> Async.StartAsTask