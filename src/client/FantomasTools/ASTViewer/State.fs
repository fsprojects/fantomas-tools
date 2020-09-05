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

[<Emit("process.env.AST_BACKEND")>]
let private backend: string = jsNative

let private getVersion () =
    sprintf "%s/%s" backend "api/version"
    |> Http.getText

let private fetchNodeRequest url (payload: Shared.Input) dispatch =
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

let private fetchUntypedAST (payload: Shared.Input) dispatch =
    let url = sprintf "%s/api/untyped-ast" backend
    fetchNodeRequest url payload dispatch

let private fetchTypedAst (payload: Shared.Input) dispatch =
    let url = sprintf "%s/api/typed-ast" backend
    fetchNodeRequest url payload dispatch

let initialGraphModel: Graph.Model =
    { RootsPath = []
      Options =
          { MaxNodes = 30
            MaxNodesInRow = 7
            Layout = Graph.HierarchicalLeftRight } }

let private initialModel =
    { Source = ""
      Defines = ""
      IsFsi = false
      Parsed = Ok None
      IsLoading = false
      Version = ""
      View = Raw
      FSharpEditorState = Loading
      Graph = initialGraphModel }

let private getMessageFromError (ex: exn) = Error ex.Message

// defines the initial state and initial command (= side-effect) of the application
let init isActive: Model * Cmd<Msg> =
    let model =
        if isActive
        then UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel
        else initialModel

    let cmd =
        Cmd.OfPromise.either getVersion () VersionFound getMessageFromError

    model, cmd

let private getDefines (model: Model) =
    model.Defines.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest sourceCode (model: Model): Shared.Input =
    { SourceCode = sourceCode
      Defines = getDefines model
      IsFsi = model.IsFsi }

let private updateUrl code (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code model)

    UrlTools.updateUrlWithData json

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update code (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | SetSourceText x ->
        let nextModel = { model with Source = x }
        nextModel, Cmd.none
    | ASTParsed x ->
        let nextModel =
            { model with
                  IsLoading = false
                  Parsed = Ok(Some x) }

        nextModel, Cmd.none
    | Error e ->
        let nextModel =
            { model with
                  IsLoading = false
                  Parsed = Result.Error e }

        nextModel, Cmd.none
    | DoParse ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch [ Cmd.ofSub (fetchUntypedAST parseRequest)
                        Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd
    | DoTypeCheck ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch [ Cmd.ofSub (fetchTypedAst parseRequest)
                        Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd

    | VersionFound version -> { model with Version = version }, Cmd.none
    | ShowJsonViewer -> { model with View = JsonViewer }, Cmd.none
    | ShowEditor -> { model with View = Editor }, Cmd.none
    | ShowRaw -> { model with View = Raw }, Cmd.none
    | ShowGraph ->
        { model with View = Graph },
        Cmd.OfAsync.either
            (fun _ -> Async.Sleep 100)
            ()
            (fun _ -> Msg.Graph <| SetOptions model.Graph.Options)
            (fun _ -> Error "")
    | Msg.Graph (GraphMsg.SetRoot node) ->
        { model with
              Graph =
                  { model.Graph with
                        RootsPath = node :: model.Graph.RootsPath } },
        Cmd.none
    | Msg.Graph RootBack ->
        { model with
              Graph =
                  { model.Graph with
                        RootsPath =
                            model.Graph.RootsPath
                            |> function
                            | [] -> []
                            | _ :: tl -> tl } },
        Cmd.none
    | Msg.Graph (GraphMsg.SetOptions opt) ->
        { model with
              Graph = { model.Graph with Options = opt } },
        Cmd.none
    | DefinesUpdated defines -> { model with Defines = defines }, Cmd.none
    | SetFsiFile isFsi -> { model with IsFsi = isFsi }, Cmd.none
    | HighLight hlr -> model, Cmd.ofSub (FantomasTools.Client.Editor.selectRange hlr)
