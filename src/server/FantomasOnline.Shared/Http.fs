module FantomasOnline.Server.Shared.Http

open System.Net
open Thoth.Json.Net
open HttpConstants
open AWSLambdaExtensions
open FantomasOnline.Server.Shared
open FantomasOnline.Shared

module Reflection =
    open FSharp.Reflection

    let getRecordFields x =
        let names = FSharpType.GetRecordFields(x.GetType()) |> Seq.map (fun x -> x.Name)

        let values = FSharpValue.GetRecordFields x
        Seq.zip names values |> Seq.toArray

let mapOptionsToJson (options: FantomasOption list) = options |> Encoders.encodeOptions

[<RequireQualifiedAccess>]
type FormatResponse =
    | Ok of json: string
    | BadRequest of error: string
    | InternalError of error: string

let formatCode
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validate: string -> string -> Async<ASTError list>)
    (json: string)
    : Async<FormatResponse>
    =
    async {
        let model = Decoders.decodeRequest json

        let configResult =
            Result.map (fun r -> r, mapFantomasOptionsToRecord r.Options) model

        match configResult with
        | Ok({ SourceCode = code; IsFsi = isFsi }, config) ->
            let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"

            try
                let! firstFormat = format fileName code config
                let! firstValidation = validate fileName firstFormat

                let! secondFormat, secondValidation =
                    if not (List.isEmpty firstValidation) then
                        async.Return(None, [])
                    else
                        async {
                            let! secondFormat = format fileName firstFormat config
                            let! secondValidation = validate fileName secondFormat
                            return (Some secondFormat, secondValidation)
                        }

                let response =
                    { FirstFormat = firstFormat
                      FirstValidation = firstValidation
                      SecondFormat = secondFormat
                      SecondValidation = secondValidation }
                    |> Encoders.encodeFormatResponse
                    |> Encode.toString 4

                return FormatResponse.Ok response
            with exn ->
                return FormatResponse.InternalError(string exn)
        | Error err -> return FormatResponse.BadRequest err
    }

let mapFormatResponseToAPIGatewayProxyResponse (response: FormatResponse) =
    match response with
    | FormatResponse.Ok json -> HttpStatusCode.OK, HeaderValues.ApplicationJson, json
    | FormatResponse.BadRequest error -> HttpStatusCode.BadRequest, HeaderValues.ApplicationText, error
    | FormatResponse.InternalError error -> HttpStatusCode.InternalServerError, HeaderValues.ApplicationText, error
    |> mkAPIGatewayProxyResponse
