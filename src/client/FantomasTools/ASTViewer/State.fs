module FantomasTools.Client.ASTViewer.State

open System
open Elmish
open Thoth.Json
open Fetch
open Fable.Core.JsInterop
open ASTViewer
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Decoders
open FantomasTools.Client.ASTViewer.Encoders

let private backendRoot = "http://localhost:7412"

let private getVersion() =
    let url = sprintf "%s/%s" backendRoot "api/version"
    Fetch.fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun (json: string) ->
        match Decode.fromString Decode.string json with
        | Ok v -> v
        | Result.Error e -> failwithf "%A" e)

let private fetchNodeRequest url (payload: Shared.Input) =
    let json = encodeInput payload
    Fetch.fetch url
        [ RequestProperties.Body(!^json)
          RequestProperties.Method HttpMethod.POST ]
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun json ->
        match decodeResult json with
        | Ok r -> r
        | Result.Error err -> failwithf "failed to decode result: %A" err)

let private fetchUntypedAST (payload: Shared.Input) =
    let url = sprintf "%s/api/untyped-ast" backendRoot
    fetchNodeRequest url payload

let private fetchTypedAst (payload: Shared.Input) =
    let url = sprintf "%s/api/typed-ast" backendRoot
    fetchNodeRequest url payload

let private initialGraphModel : Graph.Model =
    { RootsPath = []
      Options = { MaxNodes = 30
                  MaxNodesInRow = 7
                  Layout = Graph.HierarchicalLeftRight } }

let private initialModel =
    { Source = ""
      Defines = ""
      IsFsi = false
      Parsed = Ok None
      IsLoading = false
      Version = ""
      View = Editor
      FSharpEditorState = Loading
      Graph = initialGraphModel }

let private getMessageFromError (ex:exn) = Error ex.Message

// defines the initial state and initial command (= side-effect) of the application
let init code : Model * Cmd<Msg> =
    let model = UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel

    let cmd =
        let fetchCmd = if String.IsNullOrWhiteSpace code then Cmd.none else Cmd.ofMsg DoParse
        let versionCmd = Cmd.OfPromise.either getVersion () VersionFound getMessageFromError
        Cmd.batch [ versionCmd; fetchCmd ]

    model, cmd


//    let query = URI.parseQuery()
//    let code = query |> Map.tryFind "code" |> Option.defaultValue ""
//    let filename = query |> Map.tryFind "filename" |> Option.defaultValue initialModel.FileName
//    let defines = query |> Map.tryFind "defines" |> Option.defaultValue ""
//    { initialModel with
//        Source = code
//        FileName = filename
//        Defines = defines}, Cmd.batch [cmd; Cmd.ofMsg (DoParse)]

let private getDefines (model:Model) =
    model.Defines.Split([|' ';',';';'|], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest sourceCode (model: Model) : Shared.Input =
    { SourceCode = sourceCode
      Defines = getDefines model
      IsFsi = model.IsFsi }

let private updateUrl code (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code model)
    UrlTools.updateUrlWithData json

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update code (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg with
    | SetSourceText x ->
        let nextModel = { model with Source = x }
        nextModel, Cmd.none
    | Parsed x ->
        let nextModel = { model with IsLoading = false; Parsed = Ok (Some x) }
        nextModel, Cmd.none
    | TypeChecked x ->
        let nextModel = { model with IsLoading = false; Parsed = Ok (Some x) }
        nextModel, Cmd.none
    | Error e ->
        let nextModel = { model with IsLoading = false; Parsed = Result.Error e }
        nextModel, Cmd.none
    | DoParse ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch
                [ Cmd.OfPromise.either fetchUntypedAST parseRequest Parsed getMessageFromError
                  Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd
    | DoTypeCheck ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch
                [ Cmd.OfPromise.either fetchTypedAst parseRequest Parsed getMessageFromError
                  Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd

    | VersionFound version -> { model with Version = version }, Cmd.none
    | ShowJsonViewer -> {model with View = JsonViewer}, Cmd.none
    | ShowEditor -> {model with View = Editor}, Cmd.none
    | ShowRaw -> {model with View = Raw}, Cmd.none
    | ShowGraph ->
        {model with View = Graph},
        Cmd.OfAsync.either (fun _ -> Async.Sleep 100) () (fun _ -> Msg.Graph <| SetOptions model.Graph.Options) (fun _ -> Error "")
    | Msg.Graph (GraphMsg.SetRoot node) -> { model with Graph = { model.Graph with RootsPath = node :: model.Graph.RootsPath }}, Cmd.none
    | Msg.Graph RootBack ->
        { model with Graph = { model.Graph with RootsPath = model.Graph.RootsPath |> function | [] -> [] | _::tl -> tl }}, Cmd.none
    | Msg.Graph (GraphMsg.SetOptions opt) -> { model with Graph = { model.Graph with Options = opt }}, Cmd.none
    | DefinesUpdated defines -> { model with Defines = defines }, Cmd.none
    | SetFsiFile isFsi -> { model with IsFsi = isFsi }, Cmd.none
