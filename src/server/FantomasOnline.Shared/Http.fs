module FantomasOnline.Server.Shared.Http

open Microsoft.Azure.Functions.Worker.Http
open FantomasOnline.Server.Shared
open FantomasOnline.Shared
open Microsoft.Extensions.Logging
open System.IO
open System.Threading.Tasks
open FSharp.Control.Tasks
open System.Net
open Thoth.Json.Net

module Reflection =
    open FSharp.Reflection

    let getRecordFields x =
        let names =
            FSharpType.GetRecordFields(x.GetType())
            |> Seq.map (fun x -> x.Name)

        let values = FSharpValue.GetRecordFields x
        Seq.zip names values |> Seq.toArray

let private sendJson json (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        res.StatusCode <- HttpStatusCode.OK
        do! res.WriteStringAsync(json)
        res.Headers.Add("Content-Type", "application/json")
        return res
    }

let private sendText (text: string) (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        do! res.WriteStringAsync(text, System.Text.Encoding.UTF8)
        res.StatusCode <- HttpStatusCode.OK
        res.Headers.Add("Content-Type", "text/plain")
        return res
    }

let private sendInternalError (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        do! res.WriteStringAsync(error)
        res.Headers.Add("Content-Type", "application/text")
        res.StatusCode <- HttpStatusCode.InternalServerError
        return res
    }

let private sendTooLargeError (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        do! res.WriteStringAsync("File was too large")
        res.Headers.Add("Content-Type", "application/text")
        res.StatusCode <- HttpStatusCode.RequestEntityTooLarge
        return res
    }

let private sendBadRequest (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        do! res.WriteStringAsync(error)
        res.Headers.Add("Content-Type", "application/text")
        res.StatusCode <- HttpStatusCode.BadRequest
        return res
    }

let private notFound (res: HttpResponseData) : Task<HttpResponseData> =
    task {
        let json = Encode.string "Not found" |> Encode.toString 4
        do! res.WriteStringAsync(json)
        res.StatusCode <- HttpStatusCode.NotFound
        return res
    }

let private getVersionResponse version res = sendText version res

let private formatResponse<'options>
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validateResult: string -> string -> Async<ASTError list>)
    (req: HttpRequestData)
    (res: HttpResponseData)
    : Task<HttpResponseData>
    =
    task {
        use stream = new StreamReader(req.Body)
        let! json = stream.ReadToEndAsync() |> Async.AwaitTask
        let model = Decoders.decodeRequest json

        let configResult =
            Result.map (fun r -> r, mapFantomasOptionsToRecord r.Options) model

        match configResult with
        | Ok ({ SourceCode = code; IsFsi = isFsi }, config) ->
            let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"

            try
                let! firstFormat = format fileName code config
                let! firstValidation = validateResult fileName firstFormat

                let! secondFormat, secondValidation =
                    if not (List.isEmpty firstValidation) then
                        async.Return(None, [])
                    else
                        async {
                            let! secondFormat = format fileName firstFormat config
                            let! secondValidation = validateResult fileName secondFormat
                            return (Some secondFormat, secondValidation)
                        }

                let response =
                    { FirstFormat = firstFormat
                      FirstValidation = firstValidation
                      SecondFormat = secondFormat
                      SecondValidation = secondValidation }
                    |> Encoders.encodeFormatResponse
                    |> Encode.toString 4

                return! sendJson response res
            with
            | exn -> return! sendBadRequest (sprintf "%A" exn) res
        | Error err -> return! sendInternalError err res
    }

let private mapOptionsToResponse (options: FantomasOption list) (res: HttpResponseData) =
    options
    |> Encoders.encodeOptions
    |> fun json -> sendJson json res

let main
    (getVersion: unit -> string)
    (getOptions: unit -> FantomasOption list)
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validate: string -> string -> Async<ASTError list>)
    (log: ILogger)
    (req: HttpRequestData)
    =
    let version = getVersion ()
    let path = req.Url.LocalPath.ToLower()
    let method = req.Method.ToUpper()
    let res = req.CreateResponse()
    log.LogInformation(sprintf "Running request for %s, version: %s" path version)

    match method, path with
    | "POST", "/api/format" -> formatResponse mapFantomasOptionsToRecord format validate req res
    | "GET", "/api/options" ->
        getOptions ()
        |> fun options -> mapOptionsToResponse options res
    | "GET", "/api/version" -> getVersionResponse version res
    | _ -> notFound res
