module FantomasTools.Client.FantomasOnline.State

open Elmish
open Fable.Core
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline
open FantomasTools.Client.FantomasOnline.Model
open Fetch
open Thoth.Json

[<Emit("import.meta.env.VITE_FANTOMAS_V2")>]
let private v2Backend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_V3")>]
let private v3Backend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_V4")>]
let private v4Backend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_V5")>]
let private v5Backend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_V6")>]
let private v6Backend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_MAIN")>]
let private mainBackend: string = jsNative

[<Emit("import.meta.env.VITE_FANTOMAS_PREVIEW")>]
let private previewBackend: string = jsNative

let private backend =
    Map.ofList
        [ (FantomasMode.V4, v4Backend)
          (FantomasMode.V5, v5Backend)
          (FantomasMode.V6, v6Backend)
          (FantomasMode.Main, mainBackend)
          (FantomasMode.Preview, previewBackend) ]

let private getVersion mode =
    sprintf "%s/%s" (Map.find mode backend) "version" |> Http.getText

let private getOptions mode =
    let url = sprintf "%s/%s" (Map.find mode backend) "options"

    fetch url [ RequestProperties.Method HttpMethod.GET ]
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map (fun (json: string) ->
        match Decoders.decodeOptions json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private getFormattedCode code isFsi model dispatch =
    let url = sprintf "%s/%s" (Map.find model.Mode backend) "format"

    let json = Encoders.encodeRequest code isFsi model

    Http.postJson url json
    |> Promise.iter (fun (status, body) ->
        match status with
        | 200 ->
            match Decode.fromString Decoders.decodeFormatResponse body with
            | Ok res -> Msg.FormattedReceived res
            | Error err -> Msg.FormatException err

        | 400 -> Msg.FormatException body
        | 413 -> Msg.FormatException "the input was too large to process"
        | _ -> Msg.FormatException body
        |> dispatch)

let private updateUrl code isFsi model _ =
    let json = Encode.toString 2 (Encoders.encodeUrlModel code isFsi model)

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

    { Version = "???"
      DefaultOptions = []
      UserOptions = Map.empty
      Mode = mode
      State = FantomasTabState.LoadingOptions
      SettingsFilter = "" },
    cmd

let optionsListToMap options =
    options
    |> List.map (function
        | FantomasOption.BoolOption(_, k, _) as fo -> k, fo
        | FantomasOption.IntOption(_, k, _) as fo -> k, fo
        | FantomasOption.MultilineFormatterTypeOption(_, k, _) as fo -> k, fo
        | FantomasOption.EndOfLineStyleOption(_, k, _) as fo -> k, fo
        | FantomasOption.MultilineBracketStyleOption(_, k, _) as fo -> k, fo)
    |> Map.ofList

let private updateOptionValue defaultOption userOption =
    match defaultOption, userOption with
    | IntOption(o, k, _), IntOption(_, _, v) -> IntOption(o, k, v)
    | BoolOption(o, k, _), BoolOption(_, _, v) -> BoolOption(o, k, v)
    | MultilineFormatterTypeOption(o, k, _), MultilineFormatterTypeOption(_, _, v) ->
        MultilineFormatterTypeOption(o, k, v)
    | EndOfLineStyleOption(o, k, _), EndOfLineStyleOption(_, _, v) -> EndOfLineStyleOption(o, k, v)
    | _ -> defaultOption

let private restoreUserOptionsFromUrl (defaultOptions: FantomasOption list) =
    let userOptions, isFsi =
        UrlTools.restoreModelFromUrl Decoders.decodeOptionsFromUrl ([], false)

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
                | None -> defOpt)
            |> optionsListToMap

    reconstructedOptions, isFsi

[<Emit("navigator.clipboard.writeText($0)")>]
let private writeText _text : JS.Promise<unit> = jsNative

[<Import("Notyf", from = "notyf")>]
type Notyf() =
    class
        abstract success: string -> unit
        default this.success(_: string) : unit = jsNative
        abstract error: string -> unit
        default this.error(_: string) : unit = jsNative
    end

let private notify = Notyf()
let private showSuccess message = notify.success message
let private showError message = notify.error message

let private copySettings (model: Model) _ =
    let editorconfig =
        model.SettingsChangedByTheUser
        |> List.map (function
            | FantomasOption.BoolOption(_, k, v) ->
                if v then
                    toEditorConfigName k |> sprintf "%s = true"
                else
                    toEditorConfigName k |> sprintf "%s = false"
            | FantomasOption.IntOption(_, k, v) -> sprintf "%s = %i" (toEditorConfigName k) v
            | FantomasOption.MultilineFormatterTypeOption(_, k, v)
            | FantomasOption.EndOfLineStyleOption(_, k, v)
            | FantomasOption.MultilineBracketStyleOption(_, k, v) -> sprintf "%s = %s" (toEditorConfigName k) v)
        |> String.concat "\n"
        |> sprintf "[*.{fs,fsx}]\n%s"

    writeText editorconfig
    |> Promise.catch (fun err ->
        showError "Something went wrong while copying settings to the clipboard."
        printfn "%A" err)
    |> Promise.iter (fun () -> showSuccess "Copied .editorconfig settings to clipboard!")

let update isActiveTab (bubble: BubbleModel) msg model =
    match msg with
    | Msg.Bubble _ -> model, Cmd.none // handle in upper update function
    | VersionReceived version -> { model with Version = version }, Cmd.none
    | OptionsReceived options ->
        let userOptions, isFsi =
            if isActiveTab then
                restoreUserOptionsFromUrl options
            else
                optionsListToMap options, bubble.IsFsi

        let cmd =
            if not (System.String.IsNullOrWhiteSpace bubble.SourceCode) && isActiveTab then
                Cmd.batch [ Cmd.ofMsg Format; Cmd.ofMsg (Bubble(BubbleMessage.SetFsi isFsi)) ]
            else
                Cmd.ofMsg (Bubble(BubbleMessage.SetFsi isFsi))

        { model with
            DefaultOptions = options
            UserOptions = userOptions
            State = FantomasTabState.OptionsLoaded },
        cmd
    | Format ->
        let cmd =
            Cmd.batch
                [ Cmd.ofEffect (getFormattedCode bubble.SourceCode bubble.IsFsi model)
                  Cmd.ofEffect (updateUrl bubble.SourceCode bubble.IsFsi model) ]

        { model with
            State = FantomasTabState.LoadingFormatRequest },
        cmd

    | FormatException error ->
        { model with
            State = FantomasTabState.FormatError error },
        Cmd.none

    | FormattedReceived result ->
        { model with
            State = FantomasTabState.FormatResult result },
        Cmd.none
    | UpdateOption(key, value) ->
        let userOptions = Map.add key value model.UserOptions
        { model with UserOptions = userOptions }, Cmd.none
    | ChangeMode _ -> model, Cmd.none // handle in upper update function

    | CopySettings -> model, Cmd.ofEffect (copySettings model)

    | UpdateSettingsFilter v -> { model with SettingsFilter = v }, Cmd.none
