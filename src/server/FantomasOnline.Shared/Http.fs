module FantomasOnline.Server.Shared.Http

open System.Net
open FantomasTools.Client
open Thoth.Json.Core
open Thoth.Json.System.Text.Json
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
    /// The request itself could not be read.
    | BadRequest of error: string
    /// The request was understood and formatting it failed. What kind of failure it was decides how
    /// it is reported, which is the backend's call rather than this module's.
    | Failed of error: FormatError

/// The status code and body that report a failed format, so every host says the same thing about
/// the same failure. Source the user wrote is answered with a 400: the request was fine, what it
/// carried was not. Everything else is Fantomas falling over and is a 500.
let describeFailure (error: FormatError) : int * string =
    let statusCode =
        match error.Kind with
        | FormatErrorKind.InvalidSource -> 400
        | FormatErrorKind.FantomasBug
        | FormatErrorKind.Unknown -> 500

    statusCode, (FormatError.Encode error |> Encode.toString 4)

let formatCode
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string>)
    (validate: string -> string -> Async<Diagnostic list>)
    (describeException: exn -> FormatError)
    (json: string)
    : Async<FormatResponse>
    =
    async {
        let model = Decoders.decodeRequest json

        let configResult =
            Result.map (fun r -> r, mapFantomasOptionsToRecord r.Options) model

        // The code is formatted twice: once to produce a result and once to see whether Fantomas
        // agrees with what it wrote. The two are caught apart, because they fail for different
        // reasons and only one of them can be the user's doing. Code that formatted once parses;
        // anything that goes wrong after that is Fantomas', however the exception words it.
        let respond firstFormat firstValidation secondFormat secondValidation =
            {
                FirstFormat = firstFormat
                FirstValidation = Array.ofList firstValidation
                SecondFormat = secondFormat
                SecondValidation = Array.ofList secondValidation
            }
            |> Encoders.encodeFormatResponse
            |> Encode.toString 4
            |> FormatResponse.Ok

        match configResult with
        | Ok({ SourceCode = code; IsFsi = isFsi }, config) ->
            let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"

            try
                let! firstAttempt = Async.Catch(format fileName code config)

                match firstAttempt with
                | Choice2Of2 exn ->
                    let error = describeException exn

                    // A failure that blames the code the user submitted but names no position in it
                    // leaves them hunting for it. The validator reads that code the way Fantomas
                    // does, define combinations and all, so ask it where the trouble is.
                    if error.Kind = FormatErrorKind.InvalidSource && Array.isEmpty error.Diagnostics then
                        let! diagnostics = validate fileName code

                        return
                            FormatResponse.Failed
                                { error with
                                    Diagnostics = Array.ofList diagnostics
                                }
                    else
                        return FormatResponse.Failed error
                | Choice1Of2 firstFormat ->
                    let! firstValidation = validate fileName firstFormat

                    if not (List.isEmpty firstValidation) then
                        // Fantomas wrote something it does not accept. The result is reported as a
                        // result, so the tab shows it beside the diagnostics that condemn it and
                        // keeps the button that files the issue.
                        return respond firstFormat firstValidation None []
                    else
                        let! secondAttempt = Async.Catch(format fileName firstFormat config)

                        match secondAttempt with
                        | Choice2Of2 exn ->
                            let error = describeException exn

                            return
                                FormatResponse.Failed
                                    { error with
                                        Kind = FormatErrorKind.FantomasBug
                                        Message =
                                            $"Fantomas formatted your code, and formatting that result again failed: %s{error.Message}"
                                        Detail = Some firstFormat
                                    }
                        | Choice1Of2 secondFormat ->
                            let! secondValidation = validate fileName secondFormat
                            return respond firstFormat firstValidation (Some secondFormat) secondValidation
            with exn ->
                return FormatResponse.Failed(describeException exn)
        | Error err -> return FormatResponse.BadRequest err
    }

let mapFormatResponseToAPIGatewayProxyResponse (response: FormatResponse) =
    match response with
    | FormatResponse.Ok json -> HttpStatusCode.OK, HeaderValues.ApplicationJson, json
    | FormatResponse.BadRequest error -> HttpStatusCode.BadRequest, HeaderValues.ApplicationText, error
    | FormatResponse.Failed error ->
        let statusCode, json = describeFailure error
        enum<HttpStatusCode> statusCode, HeaderValues.ApplicationJson, json
    |> mkAPIGatewayProxyResponse
