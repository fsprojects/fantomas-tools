module FantomasOnline.Server.Shared.Http

open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net

module Async =
    let lift a = async { return a }

module Reflection =
    open FSharp.Reflection

    let getRecordFields x =
        let names =
            FSharpType.GetRecordFields(x.GetType())
            |> Seq.map (fun x -> x.Name)

        let values = FSharpValue.GetRecordFields x
        Seq.zip names values |> Seq.toArray

let private notFound () =
    let json = Encode.string "Not found" |> Encode.toString 4

    new HttpResponseMessage(HttpStatusCode.NotFound,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
    |> Async.lift

let private sendText text =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text"))

let private sendBadRequest error =
    new HttpResponseMessage(HttpStatusCode.BadRequest,
                            Content = new StringContent(error, System.Text.Encoding.UTF8, "application/text"))

let private sendJson json =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

let private sendInternalError err =
    new HttpResponseMessage(HttpStatusCode.InternalServerError,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let private getVersionResponse version = sendText version |> Async.lift

let private formatResponse<'options>
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validateResult: string -> string -> Async<bool>)
    (req: HttpRequest)
    =
    async {
        use stream = new StreamReader(req.Body)
        let! json = stream.ReadToEndAsync() |> Async.AwaitTask
        let model = Decoders.decodeRequest json

        let configResult =
            Result.map (fun r -> r, mapFantomasOptionsToRecord r.Options) model

        match configResult with
        | Ok ({ SourceCode = code; IsFsi = isFsi }, config) ->
            let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"

            try
                let! formatted = format fileName code config
                let! isValid = validateResult fileName formatted

                if isValid then
                    return sendText formatted
                else
                    let content =
                        sprintf """Fantomas was able to format the code but the result appears to be invalid F# code.
Please open an issue.

Formatted result:

%O"""                    formatted

                    return sendBadRequest content
            with exn -> return sendBadRequest (sprintf "%A" exn)
        | Error err -> return sendInternalError (err)
    }

let private mapOptionsToResponse (options: FantomasOption list) =
    options
    |> Encoders.encodeOptions
    |> sendJson
    |> Async.lift

let main
    (getVersion: unit -> string)
    (getOptions: unit -> FantomasOption list)
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validate: string -> string -> Async<bool>)
    (log: ILogger)
    (req: HttpRequest)
    =
    let version = getVersion ()
    let path = req.Path.Value.ToLower()
    let method = req.Method.ToUpper()

    log.LogInformation(sprintf "Running request for %s, version: %s" path version)

    match method, path with
    | "POST", "/api/format" -> formatResponse mapFantomasOptionsToRecord format validate req
    | "GET", "/api/options" -> getOptions () |> mapOptionsToResponse
    | "GET", "/api/version" -> getVersionResponse version
    | _ -> notFound ()
