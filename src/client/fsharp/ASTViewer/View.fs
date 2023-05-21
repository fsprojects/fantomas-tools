module FantomasTools.Client.ASTViewer.View

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model

// Enter 'localStorage.setItem("debugASTRangeHighlight", "true");' in your browser console to enable.
let debugASTRangeHighlight: bool =
    not (String.IsNullOrWhiteSpace(Browser.WebStorage.localStorage.getItem "debugASTRangeHighlight"))

let cursorChanged (bubbleMsg: BubbleMessage -> unit) (model: Model) (e: obj) : unit =
    let lineNumber: int = e?position?lineNumber
    let column: int = e?position?column

    match model.State with
    | AstViewerTabState.Result { Ast = astText } ->
        let lines = astText.Split([| "\r\n"; "\n" |], StringSplitOptions.None)
        // Try and get the line where the cursor clicked in the AST editor
        match Array.tryItem (lineNumber - 1) lines with
        | None -> ()
        | Some sourceLine ->

        if debugASTRangeHighlight then
            JS.console.log (sourceLine.Trim())

        let pattern = @"\(\d+,\d+--\d+,\d+\)"

        let rangeDigits =
            Regex.Matches(sourceLine, pattern)
            |> Seq.cast<Match>
            |> fun matches ->
                if debugASTRangeHighlight then
                    JS.console.log matches

                matches
            |> Seq.tryPick (fun m ->
                if debugASTRangeHighlight then
                    JS.console.log m.Value

                let startIndex = m.Index
                let endIndex = m.Index + m.Value.Length
                // Verify the match contains the cursor column.
                if startIndex <= column && column <= endIndex then
                    m.Value.Split([| ','; '-'; '('; ')' |], StringSplitOptions.RemoveEmptyEntries)
                    |> Array.map int
                    |> Array.toList
                    |> Some
                else
                    None)

        match rangeDigits with
        | Some [ startLine; startColumn; endLine; endColumn ] ->
            let range =
                { StartLine = startLine
                  StartColumn = startColumn
                  EndLine = endLine
                  EndColumn = endColumn }

            bubbleMsg (BubbleMessage.HighLight range)
        | _ -> bubbleMsg (BubbleMessage.HighLight Range.Zero)

    | _ -> ()

let commands dispatch =
    button [ ClassName Style.Primary; OnClick(fun _ -> dispatch DoParse) ] [ str "Show Untyped AST" ]

let settings (bubble: BubbleModel) (model: Model) dispatch =
    fragment [] [
        VersionBar.versionBar $"FSC - %s{model.Version}"
        SettingControls.input
            "ast-defines"
            (BubbleMessage.SetDefines >> Bubble >> dispatch)
            (str "Defines")
            "Enter your defines separated with a space"
            bubble.Defines
        SettingControls.toggleButton
            (fun _ -> BubbleMessage.SetFsi true |> Bubble |> dispatch)
            (fun _ -> BubbleMessage.SetFsi false |> Bubble |> dispatch)
            "*.fsi"
            "*.fs"
            (str "File extension")
            bubble.IsFsi
        SettingControls.toggleButton
            (fun _ -> dispatch (SetExpand false))
            (fun _ -> dispatch (SetExpand true))
            "Regular"
            "Expanded"
            (str "Mode")
            (not model.Expand)
    ]

let view (model: Model) =
    match model.State with
    | AstViewerTabState.Loading -> Loader.tabLoading
    | _ -> null
