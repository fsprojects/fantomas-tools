﻿module FantomasTools.Client.OakViewer.State

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
        | 200 ->
            match Decode.fromString decodeOak body with
            | Ok response -> Msg.OakReceived response
            | Result.Error err -> Msg.Error err
        | _ -> Msg.Error body
        |> dispatch)

let private fetchFSCVersion () = sprintf "%s/version" backend |> Http.getText

let private initialModel: Model =
    { State =
        OakViewerTabState.Result(
            { Type = "Oak"
              Text = None
              Range = Range.Zero
              ContentBefore = Array.empty
              Children = Array.empty
              ContentAfter = Array.empty }
        )
      Version = "???"
      IsGraphView = false
      GraphViewOptions =
        { Layout = GraphView.TopDown
          NodeLimit = 25
          Scale = GraphView.SubTreeNodes
          ScaleMaxSize = 25 }
      GraphViewRootNodes = [] }

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest (bubble: BubbleModel) : OakViewer.ParseRequest =
    { SourceCode = bubble.SourceCode
      Defines = splitDefines bubble.Defines
      IsFsi = bubble.IsFsi }

let init () =
    let isGraphView = UrlTools.restoreModelFromUrl decodeUrlModel false

    let cmd =
        Cmd.OfPromise.either fetchFSCVersion () FSCVersionReceived (fun ex -> Error ex.Message)

    { initialModel with
        IsGraphView = isGraphView },
    cmd

let private updateUrl (bubble: BubbleModel) (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel bubble model)
    UrlTools.updateUrlWithData json

let update (bubble: BubbleModel) (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | Msg.Bubble _ -> model, Cmd.none // handle in upper update function
    | Msg.GetOak ->
        let parseRequest = modelToParseRequest bubble

        let cmd =
            Cmd.batch [ Cmd.ofEffect (fetchOak parseRequest); Cmd.ofEffect (updateUrl bubble model) ]

        { model with
            State = OakViewerTabState.Loading },
        cmd
    | Msg.OakReceived(oak, diagnostics) ->
        let cmd = Cmd.ofMsg (Msg.Bubble(BubbleMessage.SetDiagnostics diagnostics))

        { model with
            State = OakViewerTabState.Result oak
            GraphViewRootNodes = [] },
        cmd
    | Msg.Error error ->
        { initialModel with
            State = OakViewerTabState.Error error },
        Cmd.none
    | FSCVersionReceived version -> { model with Version = version }, Cmd.none
    | SetGraphView value -> let m = { model with IsGraphView = value } in m, Cmd.ofEffect (updateUrl bubble m)
    | SetGraphViewLayout value ->
        { model with
            GraphViewOptions =
                { model.GraphViewOptions with
                    Layout = value } },
        Cmd.none
    | SetGraphViewNodeLimit value ->
        { model with
            GraphViewOptions =
                { model.GraphViewOptions with
                    NodeLimit = value } },
        Cmd.none
    | SetGraphViewScale value ->
        { model with
            GraphViewOptions =
                { model.GraphViewOptions with
                    Scale = value } },
        Cmd.none
    | SetGraphViewScaleMax value ->
        { model with
            GraphViewOptions =
                { model.GraphViewOptions with
                    ScaleMaxSize = value } },
        Cmd.none
    | GraphViewSetRoot nodeId ->
        { model with
            GraphViewRootNodes = nodeId :: model.GraphViewRootNodes },
        Cmd.none
    | GraphViewGoBack ->
        { model with
            GraphViewRootNodes =
                if model.GraphViewRootNodes = [] then
                    []
                else
                    List.tail model.GraphViewRootNodes },
        Cmd.none
