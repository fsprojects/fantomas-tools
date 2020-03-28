module FantomasTools.Client.Editor

open Fable.Core.JsInterop
open Fable.Core
open Fable.React

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