module FantomasTools.Client.OakViewer.State

open System
open Fable.Core
open FantomasTools.Client
open Elmish
open Thoth.Json
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Encoders
open FantomasTools.Client.OakViewer.Decoders

[<Emit("import.meta.env.VITE_OAK_BACKEND")>]
let private backend: string = jsNative

let private fetchOak (payload: OakViewer.ParseRequest) dispatch =
    let url = sprintf "%s/get-oak" backend
    let json = encodeParseRequest payload

    Http.postJson url json
    |> Promise.iter (fun (status, body) ->
        match status with
        | 200 -> Msg.OakReceived body
        | _ -> Msg.Error body
        |> dispatch)

let private fetchFSCVersion () = sprintf "%s/version" backend |> Http.getText

let private initialModel: Model =
    { Oak = ""
      Error = None
      IsLoading = true
      Defines = ""
      Version = "???"
      IsStroustrup = false
      ShowGraph = false }

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest sourceCode isFsi (model: Model) : OakViewer.ParseRequest =
    { SourceCode = sourceCode
      Defines = splitDefines model.Defines
      IsFsi = isFsi
      IsStroustrup = model.IsStroustrup }

let init isActive =
    let model =
        if isActive then
            UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel
        else
            initialModel

    let cmd =
        Cmd.OfPromise.either fetchFSCVersion () FSCVersionReceived (fun ex -> Error ex.Message)

    model, cmd

let private updateUrl code isFsi (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code isFsi model)
    UrlTools.updateUrlWithData json

let update code isFsi (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | Msg.GetOak ->
        let parseRequest = modelToParseRequest code isFsi model

        let cmd =
            Cmd.batch [ Cmd.ofSub (fetchOak parseRequest); Cmd.ofSub (updateUrl code isFsi model) ]

        { model with IsLoading = true }, cmd
    | Msg.ShowGraph ->
        { model with
            ShowGraph = not model.ShowGraph },
        Cmd.none
    | Msg.OakReceived result ->
        { model with
            IsLoading = false
            Oak = result },
        Cmd.none
    | Msg.Error error ->
        { initialModel with
            Error = Some error
            IsLoading = false },
        Cmd.none
    | DefinesUpdated d -> { model with Defines = d }, Cmd.none
    | FSCVersionReceived version ->
        { model with
            Version = version
            IsLoading = false },
        Cmd.none
    | SetFsiFile _ -> model, Cmd.none // handle in upper update function
    | SetStroustrup value -> { model with IsStroustrup = value }, Cmd.none
    | HighLight hlr -> model, Cmd.ofSub (Editor.selectRange hlr)
