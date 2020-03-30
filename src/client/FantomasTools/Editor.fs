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
    ofImport "default" "../js/Editor.js" (keyValueList CaseRules.LowerFirst props) []

let selectRange startLine startColumn endLine endColumn _ =
    let data =
        jsOptions<CustomEventInit> (fun o ->
            o.detail <-
                {| startColumn = startColumn + 1
                   startLineNumber = startLine
                   endColumn = endColumn + 1
                   endLineNumber = endLine |})

    let event = CustomEvent.Create("select_range", data)
    Dom.window.dispatchEvent (event) |> ignore