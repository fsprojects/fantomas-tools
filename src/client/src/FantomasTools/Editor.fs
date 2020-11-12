module FantomasTools.Client.Editor

open Fable.Core.JsInterop
open Fable.Core
open Fable.React
open Browser.Types
open Browser

type Editor =
    class
    end

type EditorProp =
    | OnChange of (string -> unit)
    | Value of string
    | Language of string
    | IsReadOnly of bool
    | GetEditor of (obj -> unit)

let inline editor (props: EditorProp list): ReactElement =
    ofImport "default" "../js/Editor.jsx" (keyValueList CaseRules.LowerFirst props) []

let inline editorInTab (props: EditorProp list): ReactElement =
    ofImport "default" "../../js/Editor.jsx" (keyValueList CaseRules.LowerFirst props) []

type HighLightRange =
    { StartLine: int
      StartColumn: int
      EndLine: int
      EndColumn: int }

let selectRange (range: HighLightRange) _ =
    printfn "highlight range: %A" range

    let data =
        jsOptions<CustomEventInit> (fun o ->
            o.detail <-
                {| startColumn = range.StartColumn + 1
                   startLineNumber = range.StartLine
                   endLineNumber = range.EndLine
                   endColumn = range.EndColumn + 1 |})

    let event = CustomEvent.Create("select_range", data)
    Dom.window.dispatchEvent (event) |> ignore
