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

/// Formatting checked the way a Fantomas without checks of its own allows: the result is parsed, and
/// formatted again when it parses. Fantomas 9 runs these checks and more itself; before it, the
/// tool has to. What they find is reported the way Fantomas 9 reports it, so the tab reads every
/// version's findings the same way. A failure of the first format is left to the caller: that one
/// can be the user's doing. Anything that goes wrong after it is Fantomas', however the exception
/// words it, and is reported as an issue with the result rather than in place of it.
let checkByHand
    (format: string -> string -> 'options -> Async<string>)
    (validate: string -> string -> Async<Diagnostic list>)
    (describeException: exn -> FormatError)
    (fileName: string)
    (code: string)
    (config: 'options)
    : Async<string * FormatIssue list>
    =
    async {
        let! formatted = format fileName code config
        let! diagnostics = validate fileName formatted

        if not (List.isEmpty diagnostics) then
            return formatted, [ FormatIssue.NotValidFSharp([], Array.ofList diagnostics) ]
        else
            let! secondAttempt = Async.Catch(format fileName formatted config)

            match secondAttempt with
            | Choice2Of2 exn ->
                let error = describeException exn
                return formatted, [ FormatIssue.CheckFailed("Idempotency", error.Message) ]
            | Choice1Of2 formattedAgain when formattedAgain <> formatted ->
                return formatted, [ FormatIssue.NotIdempotent formattedAgain ]
            | Choice1Of2 _ -> return formatted, []
    }

let formatCode
    (mapFantomasOptionsToRecord: FantomasOption list -> 'options)
    (format: string -> string -> 'options -> Async<string * FormatIssue list>)
    (validate: string -> string -> Async<Diagnostic list>)
    (describeException: exn -> FormatError)
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
                let! attempt = Async.Catch(format fileName code config)

                match attempt with
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
                | Choice1Of2(formatted, issues) ->
                    // Whatever the checks found, the result is reported as a result, so the tab
                    // shows it beside the issues that condemn it and keeps the button that files them.
                    return
                        {
                            Code = formatted
                            Issues = Array.ofList issues
                        }
                        |> Encoders.encodeFormatResponse
                        |> Encode.toString 4
                        |> FormatResponse.Ok
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
