module FantomasOnline.Server.Shared.Http

open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging

open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net

module Async =
    let lift a = async { return a }

module Reflection =
    open FSharp.Reflection

    let inline getRecordFields x =
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

let private sendJson json =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

let private sendInternalError err =
    new HttpResponseMessage(HttpStatusCode.InternalServerError,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let private getVersionResponse version =
    let json = Encode.string version |> Encode.toString 4

    sendJson json |> Async.lift

let private mapFantomasOptionsToRecord<'t> options =
    let newValues =
        options
        |> Seq.map (function
            | BoolOption (_, _, v) -> box v
            | IntOption (_, _, v) -> box v)
        |> Seq.toArray

    let formatConfigType = typeof<'t>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> 't

let private formatResponse<'options> (format: string -> string -> 'options -> Async<string>) (req: HttpRequest) =
    async {
        use stream = new StreamReader(req.Body)
        let! json = stream.ReadToEndAsync() |> Async.AwaitTask
        let model = Decoders.decodeRequest json

        let configResult =
            Result.map (fun r -> r, mapFantomasOptionsToRecord r.Options) model

        match configResult with
        | Ok ({ SourceCode = code; IsFsi = isFsi }, config) ->
            let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"
            let! formatted = format fileName code config
            return sendText formatted
        | Error err -> return sendInternalError (err)
    }

let private getOptions defaultInstance =
    Reflection.getRecordFields defaultInstance
    |> Seq.indexed
    |> Seq.choose (fun (idx, (k: string, v: obj)) ->
        match v with
        | :? int as i -> FantomasOption.IntOption(idx, k, i) |> Some
        | :? bool as b -> FantomasOption.BoolOption(idx, k, b) |> Some
        | _ -> None)
    |> Seq.toList
    |> Encoders.encodeOptions
    |> sendJson
    |> Async.lift

let main
    (getVersion: unit -> string)
    (format: string -> string -> 'options -> Async<string>)
    (defaultInstance: 't)
    (log: ILogger)
    (req: HttpRequest)
    =
    let version = getVersion ()
    let path = req.Path.Value.ToLower()
    let method = req.Method.ToUpper()

    log.LogInformation(sprintf "Running request for %s, version: %s" path version)

    match method, path with
    | "POST", "/api/format" -> formatResponse format req
    | "GET", "/api/options" -> getOptions defaultInstance
    | "GET", "/api/version" -> getVersionResponse version
    | _ -> notFound ()
