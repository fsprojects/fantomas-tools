module FantomasTools.Client.Editor

open Fable.Core.JsInterop
open Fable.Core
open Fable.React
open Browser.Types
open Browser
open Feliz
open System

[<AllowNullLiteral>]
type IMonacoEditor =
    abstract setSelection: obj -> unit
    abstract revealRangeInCenter: obj * int -> unit

[<RequireQualifiedAccess>]
type MonacoEditorProp =
    | Height of string
    | DefaultLanguage of string
    | DefaultValue of string
    | OnChange of (string -> unit)
    | OnMount of Action<IMonacoEditor, obj>
    | Options of obj

let inline private MonacoEditor (props: MonacoEditorProp list) : ReactElement =
    ofImport "default" "@monaco-editor/react" (keyValueList CaseRules.LowerFirst props) []

let private useEventListener (target: Element, ``type``: string, listener: Event -> unit) =
    let subscribe () =
        target.addEventListener (``type``, listener)

        { new IDisposable with
            member this.Dispose() = target.removeEventListener (``type``, listener) }

    React.useEffect (
        subscribe,
        [| box target
           box ``type``
           box listener |]
    )

[<ReactComponent>]
let Editor (isReadOnly: bool) (props: MonacoEditorProp list) =
    let editorRef = React.useRef<IMonacoEditor> (Unchecked.defaultof<IMonacoEditor>)

    let selectRange (ev: Event) =
        let ev = ev :?> CustomEvent

        if (emitJsExpr (ev, editorRef.current) "$0 && $0.detail && $1") then
            let range = ev.detail
            let editor = editorRef.current
            editor.setSelection (range)
            editor.revealRangeInCenter (range, 0)

    useEventListener (window :?> Element, "select_range", selectRange)
    let handleEditorDidMount = Action<_, _>(fun editor _ -> editorRef.current <- editor)

    let options =
        createObj [ "readOnly" ==> isReadOnly
                    "selectOnLineNumbers" ==> true
                    "lineNumbers" ==> true
                    "theme" ==> "vs-light"
                    "renderWhitespace" ==> "all"
                    "minimap" ==> createObj [ "enabled" ==> false ] ]

    let defaultProps: MonacoEditorProp list =
        [ MonacoEditorProp.Height "100%"
          MonacoEditorProp.DefaultLanguage "fsharp"
          MonacoEditorProp.Options options
          MonacoEditorProp.OnMount handleEditorDidMount ]

    MonacoEditor(defaultProps @ props)

type EditorProp =
    | OnChange of (string -> unit)
    | Value of string
    | Language of string
    | IsReadOnly of bool
    | GetEditor of (obj -> unit)

type HighLightRange =
    { StartLine: int
      StartColumn: int
      EndLine: int
      EndColumn: int }

let selectRange (range: HighLightRange) _ =
    let data =
        jsOptions<CustomEventInit> (fun o ->
            o.detail <-
                {| startColumn = range.StartColumn + 1
                   startLineNumber = range.StartLine
                   endLineNumber = range.EndLine
                   endColumn = range.EndColumn + 1 |})

    let event = CustomEvent.Create("select_range", data)
    Dom.window.dispatchEvent (event) |> ignore
