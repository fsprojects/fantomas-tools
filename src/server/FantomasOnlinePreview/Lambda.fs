module FantomasOnlinePreview.Lambda

open System.Net
open HttpConstants
open FantomasOnline.Server.Shared.Http
open FantomasOnlinePreview.FormatCode

/// The functions of this lambda, by the handler name each is deployed with in infrastructure/Program.fs.
let handlers: (string * LambdaRuntime.Handler) list =
    [
        "GetVersion", (fun _ -> task { return HttpStatusCode.OK, HeaderValues.TextPlain, getVersion () })
        "GetOptions", (fun _ -> task { return HttpStatusCode.OK, HeaderValues.ApplicationJson, getOptions () })
        "PostFormat",
        (fun body ->
            task {
                let! response = Async.StartImmediateAsTask(formatCode body)
                return mapFormatResponse response
            }
        )
    ]
