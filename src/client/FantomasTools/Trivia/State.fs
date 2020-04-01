module FantomasTools.Client.Trivia.State

open System
open Fable.Core
open Fable.Core.JsInterop
open FantomasTools.Client
open Fetch
open Elmish
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model
open FantomasTools.Client.Trivia.Encoders
open FantomasTools.Client.Trivia.Decoders
open Thoth.Json

[<Emit("process.env.TRIVIA_BACKEND")>]
let private backend: string = jsNative

// let private backend: string = "http://localhost:7896"

let private fetchTrivia (payload: ParseRequest) =
    let url = sprintf "%s/api/get-trivia" backend
    let json = encodeParseRequest payload
    Fetch.fetch url
        [ RequestProperties.Body(!^json)
          RequestProperties.Method HttpMethod.POST ]
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun json ->
        match decodeResult json with
        | Ok r -> r
        | Error err -> failwithf "failed to decode result: %A" err)

let private fetchFSCVersion() =
    let url = sprintf "%s/api/version" backend
    Fetch.fetch url []
    |> Promise.bind (fun response -> response.text())
    |> Promise.map (fun json ->
        match decodeVersion json with
        | Ok v -> v
        | Error err -> failwithf "failed to decode version: %A" err)

let private initialModel : Model =
    { ActiveTab = ByTriviaNodes
      Trivia = []
      TriviaNodes = []
      ActiveByTriviaIndex = 0
      ActiveByTriviaNodeIndex = 0
      Defines = ""
      FSCVersion = "???"
      IsFsi = false
      KeepNewlineAfter = false
      Exception = None
      IsLoading = true }

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

let private modelToParseRequest sourceCode (model: Model) =
    { SourceCode = sourceCode
      Defines = splitDefines model.Defines
      FileName = if model.IsFsi then "script.fsi" else "script.fsx"
      KeepNewlineAfter = model.KeepNewlineAfter }

let init code =
    let model = UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel

    let cmd =
        let fetchCmd = if String.IsNullOrWhiteSpace code then Cmd.none else Cmd.ofMsg GetTrivia
        let versionCmd = Cmd.OfPromise.either fetchFSCVersion () FSCVersionReceived NetworkError
        Cmd.batch [ versionCmd; fetchCmd ]

    model, cmd

let private updateUrl code (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code model)
    UrlTools.updateUrlWithData json

let update code msg model =
    match msg with
    | SelectTab tab ->
        { model with ActiveTab = tab }, Cmd.none
    | GetTrivia ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch
                [ Cmd.OfPromise.either fetchTrivia parseRequest TriviaReceived NetworkError
                  Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd
    | TriviaReceived result ->
        { model with
              IsLoading = false
              Trivia = result.Trivia
              TriviaNodes = result.TriviaNodes
              ActiveByTriviaIndex = 0
              ActiveByTriviaNodeIndex = 0 }, Cmd.none
    | NetworkError err ->
        { initialModel with Exception = Some err }, Cmd.none
    | ActiveItemChange(tab, index) ->
        let model, range =
            match tab with
            | ByTriviaNodes ->
                let range =
                    List.tryItem index model.TriviaNodes |> Option.map (fun t -> t.Range)
                { model with ActiveByTriviaNodeIndex = index }, range
            | ByTrivia ->
                let range =
                    List.tryItem index model.Trivia |> Option.map (fun tv -> tv.Range)
                { model with ActiveByTriviaIndex = index }, range

        let cmd =
            range
            |> Option.map (fun r ->
                let highLightRange: FantomasTools.Client.Editor.HighLightRange =
                    { StartLine = r.StartLine
                      StartColumn = r.StartColumn
                      EndLine = r.EndLine
                      EndColumn = r.EndColumn }
                Cmd.ofSub (FantomasTools.Client.Editor.selectRange highLightRange))
            |> Option.defaultValue Cmd.none

        model, cmd
    | UpdateDefines d ->
        { model with Defines = d }, Cmd.none
    | FSCVersionReceived version ->
        { model with FSCVersion = version; IsLoading = false }, Cmd.none
    | SetFsiFile v ->
        { model with IsFsi = v }, Cmd.none
    | SetKeepNewlineAfter kna ->
        { model with KeepNewlineAfter = kna }, Cmd.none