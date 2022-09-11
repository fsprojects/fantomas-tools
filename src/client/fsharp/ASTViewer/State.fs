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
let private backend: string = jsNative

let private getVersion () = sprintf "%s/%s" backend "version" |> Http.getText

let private fetchNodeRequest url (payload: Shared.Request) dispatch =
    let json = encodeInput payload

    Http.postJson url json
    |> Promise.iter (fun (status, body) ->
        match status with
        | 200 ->
            match decodeResult body with
            | Ok r -> ASTParsed r
            | Result.Error err -> Error(sprintf "failed to decode response: %A" err)
        | 400 -> Error body
        | 413 -> Error "the input was too large to process"
        | _ -> Error body
        |> dispatch)

let private fetchUntypedAST (payload: Shared.Request) dispatch =
    let url = sprintf "%s/untyped-ast" backend
    fetchNodeRequest url payload dispatch

let private fetchTypedAst (payload: Shared.Request) dispatch =
    let url = sprintf "%s/typed-ast" backend
    fetchNodeRequest url payload dispatch

let private initialModel =
    { Source = ""
      Defines = ""
      Parsed = None
      IsLoading = false
      Version = ""
      FSharpEditorState = Loading }

let private getMessageFromError (ex: exn) = Error ex.Message

// defines the initial state and initial command (= side-effect) of the application
let init isActive : Model * Cmd<Msg> =
    let model =
        if isActive then
            UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel
        else
            initialModel

    let cmd = Cmd.OfPromise.either getVersion () VersionFound getMessageFromError

    model, cmd

let private getDefines (model: Model) =
    model.Defines.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest sourceCode isFsi (model: Model) : Shared.Request =
    { SourceCode = sourceCode
      Defines = getDefines model
      IsFsi = isFsi }

let private updateUrl code isFsi (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code isFsi model)

    UrlTools.updateUrlWithData json

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update code isFsi (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | SetSourceText x ->
        let nextModel = { model with Source = x }
        nextModel, Cmd.none
    | ASTParsed x ->
        let nextModel =
            { model with
                IsLoading = false
                Parsed = Some(Ok x) }

        nextModel, Cmd.none
    | Error e ->
        let nextModel =
            { model with
                IsLoading = false
                Parsed = Some(Result.Error e) }

        nextModel, Cmd.none
    | DoParse ->
        let parseRequest = modelToParseRequest code isFsi model

        let cmd =
            Cmd.batch
                [ Cmd.ofSub (fetchUntypedAST parseRequest)
                  Cmd.ofSub (updateUrl code isFsi model) ]

        { model with IsLoading = true }, cmd

    | VersionFound version -> { model with Version = version }, Cmd.none
    | DefinesUpdated defines -> { model with Defines = defines }, Cmd.none
    | SetFsiFile _ -> model, Cmd.none // handle in upper update function
    | HighLight hlr -> model, Cmd.ofSub (Editor.selectRange hlr)
