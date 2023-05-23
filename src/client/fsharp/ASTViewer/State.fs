module FantomasTools.Client.ASTViewer.State

open System
open Elmish
open Thoth.Json
open Fable.Core
open ASTViewer
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Decoders
open FantomasTools.Client.ASTViewer.Encoders

[<Emit("import.meta.env.VITE_AST_BACKEND")>]
let backend: string = jsNative

let getVersion () = sprintf "%s/%s" backend "version" |> Http.getText

let fetchNodeRequest url (payload: Shared.Request) dispatch =
    let json = encodeInput payload

    Http.postJson url json
    |> Promise.iter (fun (status, body) ->
        match status with
        | 200 ->
            match decodeResult body with
            | Ok r -> ASTParsed r
            | Result.Error err -> Error $"failed to decode response: %A{err}"
        | 400 -> Error body
        | 413 -> Error "the input was too large to process"
        | _ -> Error body
        |> dispatch)

let fetchUntypedAST (payload: Shared.Request) dispatch =
    let url = $"%s{backend}/untyped-ast"
    fetchNodeRequest url payload dispatch

let initialModel =
    { State = AstViewerTabState.Result { Ast = ""; Diagnostics = Array.empty }
      Version = ""
      Expand = false }

let getMessageFromError (ex: exn) = Error ex.Message

// defines the initial state and initial command (= side-effect) of the application
let init isActive : Model * Cmd<Msg> =
    let cmd =
        Cmd.batch
            [ if isActive then
                  yield! UrlTools.restoreModelFromUrl decodeUrlModel []
              yield Cmd.OfPromise.either getVersion () VersionFound getMessageFromError ]

    initialModel, cmd

let getDefines (bubble: BubbleModel) =
    bubble.Defines.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

let modelToParseRequest expand (bubble: BubbleModel) : Shared.Request =
    { SourceCode = bubble.SourceCode
      Defines = getDefines bubble
      IsFsi = bubble.IsFsi
      Expand = expand }

let updateUrl expand (bubble: BubbleModel) _ =
    let json = Encode.toString 2 (encodeUrlModel expand bubble)
    UrlTools.updateUrlWithData json

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (bubble: BubbleModel) (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Msg.Bubble _ -> model, Cmd.none // handle in upper update function
    | ASTParsed astResult ->
        let nextModel =
            { model with
                State = AstViewerTabState.Result astResult }

        let resultCmd = Cmd.ofMsg (BubbleMessage.SetResultCode astResult.Ast |> Msg.Bubble)

        let diagnosticsCmd =
            Cmd.ofMsg (BubbleMessage.SetDiagnostics astResult.Diagnostics |> Msg.Bubble)

        nextModel, Cmd.batch [ resultCmd; diagnosticsCmd ]
    | Error e ->
        let nextModel =
            { model with
                State = AstViewerTabState.Error e }

        nextModel, Cmd.none
    | DoParse ->
        let parseRequest = modelToParseRequest model.Expand bubble

        let cmd =
            Cmd.batch
                [ Cmd.ofEffect (fetchUntypedAST parseRequest)
                  Cmd.ofEffect (updateUrl model.Expand bubble) ]

        { model with
            State = AstViewerTabState.Loading },
        cmd

    | VersionFound version -> { model with Version = version }, Cmd.none
    | SetExpand value -> { model with Expand = value }, Cmd.ofMsg DoParse
