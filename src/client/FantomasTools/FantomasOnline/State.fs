module FantomasTools.Client.FantomasOnline.State

open Elmish
open Fable.Core
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline
open FantomasTools.Client.FantomasOnline
open FantomasTools.Client.FantomasOnline
open FantomasTools.Client.FantomasOnline.Model
open System
open Fable.Core
open Fable.Core.JsInterop
open Elmish
open Fetch
open Thoth.Json


[<Emit("process.env.FANTOMAS_PREVIOUS")>]
let private previousBackend: string = jsNative

[<Emit("process.env.FANTOMAS_LATEST")>]
let private latestBackend: string = jsNative

[<Emit("process.env.FANTOMAS_PREVIEW")>]
let private previewBackend: string = jsNative

let private backend =
    Map.ofList
        [ (FantomasMode.Previous, previousBackend)
          (FantomasMode.Latest, latestBackend)
          (FantomasMode.Preview, previewBackend) ]
let private getVersion mode =
    let url = sprintf "%s/%s" (Map.find mode backend) "api/version"

    fetch url
        [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun (json: string) ->
        match Decode.fromString Decode.string json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private getOptions mode =
    let url = sprintf "%s/%s" (Map.find mode backend) "api/options"
    fetch url [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun (json: string) ->
        match Decoders.decodeOptions json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private getFormattedCode code model =
    let url = sprintf "%s/%s" (Map.find model.Mode backend) "api/format"
    let json = Encoders.encodeRequest code model
    fetch url [ RequestProperties.Method HttpMethod.POST; RequestProperties.Body (!^json) ]
    |> Promise.bind (fun res -> res.text())

let init (mode: FantomasMode) =
    let cmd =
        let versionCmd = Cmd.OfPromise.either getVersion mode VersionReceived NetworkError
        let optionsCmd = Cmd.OfPromise.either getOptions mode OptionsReceived NetworkError
        Cmd.batch [ versionCmd; optionsCmd ]

    { IsFsi = false
      IsLoading = true
      Version = "???"
      Options = []
      Mode = mode
      Result = None }, cmd

let update code msg model =
    match msg with
    | VersionReceived version ->
        { model with Version = version }, Cmd.none
    | OptionsReceived options ->
        { model with Options = options; IsLoading = false }, Cmd.none
    | Format ->
        { model with IsLoading = true }, Cmd.OfPromise.either (getFormattedCode code) model FormattedReceived NetworkError
    | FormattedReceived result ->
        { model with Result = Some result; IsLoading = false }, Cmd.none
    | _ ->
        model, Cmd.none
