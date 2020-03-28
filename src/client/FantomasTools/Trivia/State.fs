module FantomasTools.Client.Trivia.State

open System
open Browser
open Browser.Types
open Fable.Core.JsInterop
open Fetch
open Elmish
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model
open FantomasTools.Client.Trivia.Encoders
open FantomasTools.Client.Trivia.Decoders

//[<Emit("process.env.BACKEND")>]
//let private backend: string = jsNative

let private backend: string = "http://localhost:7896"

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

let init _ =
    let model = initialModel
    let cmd =
//        let parseCmd =
//            match parseRequest with
//            | Some pr -> Cmd.OfPromise.either fetchTrivia pr TriviaReceived NetworkError
//            | None -> Cmd.none

        let versionCmd = Cmd.OfPromise.either fetchFSCVersion () FSCVersionReceived NetworkError
        Cmd.batch [ versionCmd ] //; parseCmd ]

    model, cmd

let private selectRange (range: Range) _ =
    let data =
        jsOptions<CustomEventInit> (fun o ->
            o.detail <-
                {| startColumn = range.StartColumn + 1
                   startLineNumber = range.StartLine
                   endColumn = range.EndColumn + 1
                   endLineNumber = range.EndLine |})

    let event = CustomEvent.Create("trivia_select_range", data)
    Dom.window.dispatchEvent (event) |> ignore

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

let private modelToParseRequest sourceCode (model: Model) =
    { SourceCode = sourceCode
      Defines = splitDefines model.Defines
      FileName =
          if model.IsFsi then "script.fsi" else "script.fsx"
      KeepNewlineAfter = model.KeepNewlineAfter }

let update code msg model =
    match msg with
    | SelectTab tab ->
        { model with ActiveTab = tab }, Cmd.none
    | GetTrivia ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch
                [ Cmd.OfPromise.either fetchTrivia parseRequest TriviaReceived NetworkError
                  ]
                  // Cmd.ofSub (updateUrl model) ]

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
            |> Option.map (fun r -> Cmd.ofSub (selectRange r))
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