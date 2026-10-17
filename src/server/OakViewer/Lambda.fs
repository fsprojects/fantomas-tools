module OakViewer.Lambda

open System.Net
open HttpConstants
open OakViewer.GetOak

let private mapOakResponse response =
    match response with
    | GetOakResponse.Ok body -> HttpStatusCode.OK, HeaderValues.ApplicationText, body
    | GetOakResponse.BadRequest body -> HttpStatusCode.BadRequest, HeaderValues.ApplicationText, body
    | GetOakResponse.Failed error ->
        let statusCode, json = describeFailure error
        enum<HttpStatusCode> statusCode, HeaderValues.ApplicationJson, json

/// The functions of this lambda, by the handler name each is deployed with in infrastructure/Program.fs.
let handlers: (string * LambdaRuntime.Handler) list =
    [
        "GetVersion", (fun _ -> task { return HttpStatusCode.OK, HeaderValues.TextPlain, getVersion () })
        "GetOak", (fun body -> task { return getOak body |> mapOakResponse })
    ]
