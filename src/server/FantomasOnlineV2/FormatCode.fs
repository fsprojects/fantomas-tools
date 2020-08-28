namespace FantomasOnlineLatest.Server

open Fantomas
open FantomasOnline.Server.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.Net

module FormatCode =
    let private checker = CodeFormatterImpl.sharedChecker.Value

    let version () =
        let assembly = typeof<FormatConfig.FormatConfig>.Assembly

        let version = assembly.GetName().Version
        sprintf "%i.%i.%i" version.Major version.Minor version.Build

    let format fileName code config =
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
        Http.main version format validate FormatConfig.FormatConfig.Default log req
        |> Async.StartAsTask
