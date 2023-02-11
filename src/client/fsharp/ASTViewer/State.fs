module FantomasTools.Client.ASTViewer.State

open System
open Elmish
open Thoth.Json
open Fable.Core
open Fable.Core.JsInterop
open ASTViewer
open FantomasTools.Client.Editor
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
    | HighLight(line, column) ->
        match model.Parsed with
        | Some(Ok { Shared.Response.String = astText }) ->
            let lines = astText.Split([| "\r\n"; "\n" |], StringSplitOptions.None)
            // Try and get the line where the cursor clicked in the AST editor
            match Array.tryItem (line - 1) lines with
            | None -> model, Cmd.none
            | Some sourceLine ->
                let matches: string array option =
                    emitJsExpr sourceLine @"$0.match(/\d+,\d+\-\-\d+,\d+/g)"

                match matches with
                | None -> model, Cmd.none
                | Some matches ->
                    // Find the range text that matches our cursor column.
                    let highlightRange =
                        matches
                        |> Array.tryPick (fun m ->
                            let startIndex = sourceLine.IndexOf(m)
                            let endIndex = startIndex + m.Length

                            if startIndex <= column && column <= endIndex then
                                let parts = m.Split("--")
                                let startPos = parts.[0]

                                let endPos = parts.[1]

                                { StartLine = int (startPos.Split(",").[0])
                                  StartColumn = int (startPos.Split(",").[1])
                                  EndLine = int (endPos.Split(",").[0])
                                  EndColumn = int (endPos.Split(",").[1]) }
                                : HighLightRange
                                |> Some
                            else
                                None)

                    let cmd =
                        match highlightRange with
                        | None -> Cmd.none
                        | Some hlr -> Cmd.ofSub (selectRange hlr)

                    model, cmd
        | _ -> model, Cmd.none
