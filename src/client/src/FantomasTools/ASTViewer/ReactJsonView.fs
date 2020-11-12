module FantomasTools.Client.ASTViewer.ReactJsonView

open Fable.Core.JsInterop
open Fable.Core
open Fable.React

type LookupData = { id: string; key: string; value: obj }

type Props =
    | Src of obj
    | Name of string
    | DisplayObjectSize of bool
    | DisplayDataTypes of bool
    | IndentWidth of int
    | ShouldCollapse of obj
    | OnLookup of (LookupData -> unit)
    | ShouldLookup of (LookupData -> bool)

let inline viewer (props: Props list): ReactElement =
    ofImport "default" "react-json-view" (keyValueList CaseRules.LowerFirst props) []
