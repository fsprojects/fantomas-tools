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

[<Emit("import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V2")>]
let private v2Backend : string = jsNative

[<Emit("import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V3")>]
let private v3Backend : string = jsNative

[<Emit("import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V4")>]
let private v4Backend : string = jsNative

[<Emit("import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_PREVIEW")>]
let private previewBackend : string = jsNative

let private backend =
    Map.ofList [ (FantomasMode.V2, v2Backend)
                 (FantomasMode.V3, v3Backend)
                 (FantomasMode.V4, v4Backend)
                 (FantomasMode.Preview, previewBackend) ]

let private getVersion mode =
    sprintf "%s/%s" (Map.find mode backend) "api/version"
    |> Http.getText

let private getOptions mode =
    let url =
        sprintf "%s/%s" (Map.find mode backend) "api/options"

    fetch url [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map
        (fun (json: string) ->
            match Decoders.decodeOptions json with
            | Ok v -> v
            | Error e -> failwithf "%A" e)

let private getFormattedCode code model dispatch =
    let url =
        sprintf "%s/%s" (Map.find model.Mode backend) "api/format"

    let json = Encoders.encodeRequest code model

    Http.postJson url json
    |> Promise.iter
        (fun (status, body) ->
            match status with
            | 200 ->
                match Decode.fromString Decoders.decodeFormatResponse body with
                | Ok res -> Msg.FormattedReceived res
                | Error err -> Msg.FormatException err

            | 400 -> Msg.FormatException body
            | 413 -> Msg.FormatException "the input was too large to process"
            | _ -> Msg.FormatException body
            |> dispatch)

let private updateUrl code model _ =
    let json =
        Encode.toString 2 (Encoders.encodeUrlModel code model)

    UrlTools.updateUrlWithData json

let getOptionsCmd mode =
    Cmd.OfPromise.either getOptions mode OptionsReceived (fun exn -> Msg.FormatException exn.Message)

let getVersionCmd mode =
    Cmd.OfPromise.either getVersion mode VersionReceived (fun exn -> Msg.FormatException exn.Message)

let init (mode: FantomasMode) =
    let cmd =
        let versionCmd = getVersionCmd mode
        let optionsCmd = getOptionsCmd mode
        Cmd.batch [ versionCmd; optionsCmd ]

    { IsFsi = false
      Version = "???"
      DefaultOptions = []
      UserOptions = Map.empty
      Mode = mode
      State = LoadingOptions },
    cmd

let optionsListToMap options =
    options
    |> List.map
        (function
        | FantomasOption.BoolOption (_, k, _) as fo -> k, fo
        | FantomasOption.IntOption (_, k, _) as fo -> k, fo
        | FantomasOption.MultilineFormatterTypeOption (_, k, _) as fo -> k, fo
        | FantomasOption.EndOfLineStyleOption (_, k, _) as fo -> k, fo)
    |> Map.ofList

let private updateOptionValue defaultOption userOption =
    match defaultOption, userOption with
    | IntOption (o, k, _), IntOption (_, _, v) -> IntOption(o, k, v)
    | BoolOption (o, k, _), BoolOption (_, _, v) -> BoolOption(o, k, v)
    | _ -> defaultOption

let private restoreUserOptionsFromUrl (defaultOptions: FantomasOption list) =
    let userOptions, isFsi =
        UrlTools.restoreModelFromUrl (Decoders.decodeOptionsFromUrl) ([], false)

    let reconstructedOptions =
        match userOptions with
        | [] -> optionsListToMap defaultOptions
        | uo ->
            defaultOptions
            |> List.map
                (fun defOpt ->
                    // map the value from the url if found
                    let key = getOptionKey defOpt

                    let matchingUserOption =
                        List.tryFind (fun uOpt -> (getOptionKey uOpt) = key) uo

                    match matchingUserOption with
                    | Some muo -> updateOptionValue defOpt muo
                    | None -> defOpt)
            |> optionsListToMap

    reconstructedOptions, isFsi

[<Emit("navigator.clipboard.writeText($0)")>]
let private writeText _text : JS.Promise<unit> = jsNative

let private showSuccess _message = import "showSuccess" "../../js/notifications"
let private showError _message = import "showSuccess" "../../js/notifications"

let private copySettings (model: Model) _ =
    let supportedProperties =
        [ "max_line_length"
          "indent_size"
          "end_of_line" ]

    let toEditorConfigName value =
        value
        |> Seq.map
            (fun c ->
                if System.Char.IsUpper(c) then
                    sprintf "_%s" (c.ToString().ToLower())
                else
                    c.ToString())
        |> String.concat ""
        |> fun s -> s.TrimStart([| '_' |])
        |> fun name ->
            if List.contains name supportedProperties then
                name
            else
                sprintf "fsharp_%s" name

    let editorconfig =
        model.SettingsChangedByTheUser
        |> List.map
            (function
            | FantomasOption.BoolOption (_, k, v) ->
                if v then
                    toEditorConfigName k |> sprintf "%s=true"
                else
                    toEditorConfigName k |> sprintf "%s=false"
            | FantomasOption.IntOption (_, k, v) -> sprintf "%s=%i" (toEditorConfigName k) v
            | FantomasOption.MultilineFormatterTypeOption (_, k, v)
            | FantomasOption.EndOfLineStyleOption (_, k, v) -> sprintf "%s=%s" (toEditorConfigName k) v)
        |> String.concat "\n"
        |> sprintf "[*.fs]\n%s"

    writeText editorconfig
    |> Promise.catch
        (fun err ->
            showError "Something went wrong while copying settings to the clipboard."
            printfn "%A" err)
    |> Promise.iter (fun () -> showSuccess "Copied fantomas-config settings to clipboard!")

let update isActiveTab code msg model =
    match msg with
    | VersionReceived version -> { model with Version = version }, Cmd.none
    | OptionsReceived options ->
        let userOptions, isFsi =
            if isActiveTab then
                restoreUserOptionsFromUrl options
            else
                optionsListToMap options, model.IsFsi

        let cmd =
            if not (System.String.IsNullOrWhiteSpace code)
               && isActiveTab then
                Cmd.ofMsg Format
            else
                Cmd.none

        { model with
              DefaultOptions = options
              UserOptions = userOptions
              IsFsi = isFsi
              State = OptionsLoaded },
        cmd
    | Format ->
        let cmd =
            Cmd.batch [ Cmd.ofSub (getFormattedCode code model)
                        Cmd.ofSub (updateUrl code model) ]

        { model with
              State = LoadingFormatRequest },
        cmd

    | FormatException error -> { model with State = FormatError error }, Cmd.none

    | FormattedReceived result ->
        { model with
              State = FormatResult result },
        Cmd.none
    | UpdateOption (key, value) ->
        let userOptions = Map.add key value model.UserOptions
        { model with UserOptions = userOptions }, Cmd.none
    | ChangeMode _ -> model, Cmd.none // handle in upper update function

    | SetFsiFile isFsi -> { model with IsFsi = isFsi }, Cmd.none

    | CopySettings -> model, Cmd.ofSub (copySettings model)
