module FantomasTools.Client.FantomasOnline.State

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline
open FantomasTools.Client.FantomasOnline.Model
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

    fetch url [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map (fun (json: string) ->
        match Decode.fromString Decode.string json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private getOptions mode =
    let url = sprintf "%s/%s" (Map.find mode backend) "api/options"
    fetch url [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map (fun (json: string) ->
        match Decoders.decodeOptions json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private getFormattedCode code model =
    let url = sprintf "%s/%s" (Map.find model.Mode backend) "api/format"
    let json = Encoders.encodeRequest code model
    fetch url
        [ RequestProperties.Method HttpMethod.POST
          RequestProperties.Body(!^json) ]
    |> Promise.bind (fun res -> res.text ())

let private updateUrl code model _ =
    let json = Encode.toString 2 (Encoders.encodeUrlModel code model)
    UrlTools.updateUrlWithData json

let getOptionsCmd mode = Cmd.OfPromise.either getOptions mode OptionsReceived NetworkError

let init (mode: FantomasMode) =
    let cmd =
        let versionCmd = Cmd.OfPromise.either getVersion mode VersionReceived NetworkError
        let optionsCmd = getOptionsCmd mode
        Cmd.batch [ versionCmd; optionsCmd ]

    { IsFsi = false
      IsLoading = true
      Version = "???"
      DefaultOptions = []
      UserOptions = Map.empty
      Mode = mode
      Result = None }, cmd

let optionsListToMap options =
    options
    |> List.map (function
        | FantomasOption.BoolOption (_, k, _) as fo -> k, fo
        | FantomasOption.IntOption (_, k, _) as fo -> k, fo)
    |> Map.ofList

let private updateOptionValue defaultOption userOption =
    match defaultOption, userOption with
    | IntOption(o,k, _), IntOption(_,_,v) -> IntOption(o,k,v)
    | BoolOption(o,k,_), BoolOption(_,_,v) -> BoolOption(o,k,v)
    | _ -> defaultOption

let private restoreUserOptionsFromUrl (defaultOptions: FantomasOption list) =
    let userOptions, isFsi = UrlTools.restoreModelFromUrl (Decoders.decodeOptionsFromUrl) ([], false)
    let reconstructedOptions =
        match userOptions with
        | [] -> optionsListToMap defaultOptions
        | uo ->
            defaultOptions
            |> List.map (fun defOpt ->
                // map the value from the url if found
                 let key = getOptionKey defOpt
                 let matchingUserOption = List.tryFind (fun uOpt -> (getOptionKey uOpt) = key) uo
                 match matchingUserOption with
                 | Some muo -> updateOptionValue defOpt muo
                 | None -> defOpt
            )
            |> optionsListToMap
    reconstructedOptions, isFsi

let update isActiveTab code msg model =
    match msg with
    | VersionReceived version -> { model with Version = version }, Cmd.none
    | OptionsReceived options ->
        let userOptions,isFsi = restoreUserOptionsFromUrl options
        let cmd =
            if not (System.String.IsNullOrWhiteSpace code) && isActiveTab
            then Cmd.ofMsg Format
            else Cmd.none

        { model with
              DefaultOptions = options
              UserOptions = userOptions
              IsFsi = isFsi
              IsLoading = false }, cmd
    | Format ->
        let cmd =
            Cmd.batch [
                Cmd.OfPromise.either (getFormattedCode code) model FormattedReceived NetworkError
                Cmd.ofSub (updateUrl code model)
            ]

        { model with IsLoading = true }, cmd

    | FormattedReceived result ->
        { model with
              Result = Some result
              IsLoading = false }, Cmd.none
    | UpdateOption (key, value) ->
        let userOptions = Map.add key value model.UserOptions
        { model with UserOptions = userOptions }, Cmd.none
    | ChangeMode _ ->
        model, Cmd.none // handle in upper update function

    | SetFsiFile isFsi ->
        { model with IsFsi = isFsi }, Cmd.none
