module FantomasTools.Client.ASTViewer.State

open System
open System.Text.RegularExpressions
open Elmish
open Thoth.Json
open Fable.Core
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
            | Result.Error err -> Error $"failed to decode response: %A{err}"
        | 400 -> Error body
        | 413 -> Error "the input was too large to process"
        | _ -> Error body
        |> dispatch)

let private fetchUntypedAST (payload: Shared.Request) dispatch =
    let url = $"%s{backend}/untyped-ast"
    fetchNodeRequest url payload dispatch

let private fetchTypedAst (payload: Shared.Request) dispatch =
    let url = $"%s{backend}/typed-ast"
    fetchNodeRequest url payload dispatch

let private initialModel =
    { Parsed = None
      IsLoading = false
      Version = ""
      FSharpEditorState = Loading }

let private getMessageFromError (ex: exn) = Error ex.Message

// defines the initial state and initial command (= side-effect) of the application
let init isActive : Model * Cmd<Msg> =
    let cmd =
        Cmd.batch
            [ if isActive then
                  yield! UrlTools.restoreModelFromUrl decodeUrlModel []
              yield Cmd.OfPromise.either getVersion () VersionFound getMessageFromError ]

    initialModel, cmd

let private getDefines (bubble: BubbleModel) =
    bubble.Defines.Split([| ' '; ','; ';' |], StringSplitOptions.RemoveEmptyEntries)

let private modelToParseRequest (bubble: BubbleModel) : Shared.Request =
    { SourceCode = bubble.SourceCode
      Defines = getDefines bubble
      IsFsi = bubble.IsFsi }

let private updateUrl (bubble: BubbleModel) _ =
    let json = Encode.toString 2 (encodeUrlModel bubble)
    UrlTools.updateUrlWithData json

// Enter 'localStorage.setItem("debugASTRangeHighlight", "true");' in your browser console to enable.
let debugASTRangeHighlight: bool =
    not (String.IsNullOrWhiteSpace(Browser.WebStorage.localStorage.getItem "debugASTRangeHighlight"))

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (bubble: BubbleModel) (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Msg.Bubble _ -> model, Cmd.none // handle in upper update function
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
        let parseRequest = modelToParseRequest bubble

        let cmd =
            Cmd.batch [ Cmd.ofEffect (fetchUntypedAST parseRequest); Cmd.ofEffect (updateUrl bubble) ]

        { model with IsLoading = true }, cmd

    | VersionFound version -> { model with Version = version }, Cmd.none
// TODO: fix AST highlighting
// | HighLight(line, column) ->
//     match model.Parsed with
//     | Some(Ok { Shared.Response.String = astText }) ->
//         let lines = astText.Split([| "\r\n"; "\n" |], StringSplitOptions.None)
//         // Try and get the line where the cursor clicked in the AST editor
//         match Array.tryItem (line - 1) lines with
//         | None -> model, Cmd.none
//         | Some sourceLine ->
//             if debugASTRangeHighlight then
//                 JS.console.log (sourceLine.Trim())
//
//             let pattern = @"\(\d+,\d+--\d+,\d+\)"
//
//             let rangeDigits =
//                 Regex.Matches(sourceLine, pattern)
//                 |> Seq.cast<Match>
//                 |> fun matches ->
//                     if debugASTRangeHighlight then
//                         JS.console.log matches
//
//                     matches
//                 |> Seq.tryPick (fun m ->
//                     if debugASTRangeHighlight then
//                         JS.console.log m.Value
//
//                     let startIndex = m.Index
//                     let endIndex = m.Index + m.Value.Length
//                     // Verify the match contains the cursor column.
//                     if startIndex <= column && column <= endIndex then
//                         m.Value.Split([| ','; '-'; '('; ')' |], StringSplitOptions.RemoveEmptyEntries)
//                         |> Array.map int
//                         |> Array.toList
//                         |> Some
//                     else
//                         None)
//
//             match rangeDigits with
//             | Some [ startLine; startColumn; endLine; endColumn ] ->
//                 let highlight =
//                     { StartLine = startLine
//                       StartColumn = startColumn
//                       EndLine = endLine
//                       EndColumn = endColumn }
//                     : HighLightRange
//
//                 model, Cmd.ofEffect (selectRange highlight)
//             | _ -> model, Cmd.none
