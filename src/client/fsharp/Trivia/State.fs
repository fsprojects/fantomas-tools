module FantomasTools.Client.Trivia.State

open System
open Fable.Core
open FantomasTools.Client
open Elmish
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model
open FantomasTools.Client.Trivia.Encoders
open FantomasTools.Client.Trivia.Decoders
open Thoth.Json

[<Emit("import.meta.env.VITE_TRIVIA_BACKEND")>]
let private backend: string = jsNative

let private fetchTrivia (payload: ParseRequest) dispatch =
    let url = sprintf "%s/api/get-trivia" backend
    let json = encodeParseRequest payload

    Http.postJson url json
    |> Promise.iter (fun (status, body) ->
        match status with
        | 200 ->
            match decodeResult body with
            | Ok r -> TriviaReceived r
            | Result.Error err -> Error(sprintf "failed to decode response: %A" err)
        | 400 -> Error body
        | 413 -> Error "the input was too large to process"
        | _ -> Error body
        |> dispatch)

let private fetchFSCVersion () = sprintf "%s/api/version" backend |> Http.getText

let private initialModel: Model =
    { ActiveTab = ByTriviaNodes
      Trivia = []
      TriviaNodeCandidates = []
      TriviaNodes = []
      ActiveByTriviaIndex = 0
      ActiveByTriviaNodeIndex = 0
      Defines = ""
      Version = "???"
      IsFsi = false
      Error = None
      IsLoading = true }

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries)
    |> List.ofArray

let private modelToParseRequest sourceCode (model: Model) =
    { SourceCode = sourceCode
      Defines = splitDefines model.Defines
      FileName = if model.IsFsi then "script.fsi" else "script.fsx" }

let init isActive =
    let model =
        if isActive then
            UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel
        else
            initialModel

    let cmd =
        Cmd.OfPromise.either fetchFSCVersion () FSCVersionReceived (fun ex -> Error ex.Message)

    model, cmd

let private updateUrl code (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code model)

    UrlTools.updateUrlWithData json

let update code msg model =
    match msg with
    | SelectTab tab -> { model with ActiveTab = tab }, Cmd.none
    | GetTrivia ->
        let parseRequest = modelToParseRequest code model

        let cmd =
            Cmd.batch [ Cmd.ofSub (fetchTrivia parseRequest)
                        Cmd.ofSub (updateUrl code model) ]

        { model with IsLoading = true }, cmd
    | TriviaReceived result ->
        { model with
            IsLoading = false
            Trivia = result.Trivia
            TriviaNodeCandidates = result.TriviaNodeCandidates
            TriviaNodes = result.TriviaNodes
            ActiveByTriviaIndex = 0
            ActiveByTriviaNodeIndex = 0 },
        Cmd.none
    | Error err ->
        { initialModel with
            Error = Some err
            IsLoading = false },
        Cmd.none
    | ActiveItemChange (tab, index) ->
        let model, range =
            match tab with
            | ByTriviaNodeCandidates -> model, None
            | ByTriviaNodes ->
                let range =
                    List.tryItem index model.TriviaNodes
                    |> Option.map (fun t -> t.Range)

                { model with ActiveByTriviaNodeIndex = index }, range
            | ByTrivia ->
                let range =
                    List.tryItem index model.Trivia
                    |> Option.map (fun tv -> tv.Range)

                { model with ActiveByTriviaIndex = index }, range

        let cmd =
            range
            |> Option.map (fun r ->
                let highLightRange: Editor.HighLightRange =
                    { StartLine = r.StartLine
                      StartColumn = r.StartColumn
                      EndLine = r.EndLine
                      EndColumn = r.EndColumn }

                Cmd.ofMsg (HighLight highLightRange))
            |> Option.defaultValue Cmd.none

        model, cmd
    | DefinesUpdated d -> { model with Defines = d }, Cmd.none
    | FSCVersionReceived version ->
        { model with
            Version = version
            IsLoading = false },
        Cmd.none
    | SetFsiFile v -> { model with IsFsi = v }, Cmd.none
    | HighLight hlr -> model, Cmd.ofSub (FantomasTools.Client.Editor.selectRange hlr)
