module FantomasTools.Client.Editor

open System
open Fable.Core.JsInterop
open Fable.Core
open Fable.React
open Browser.Types
open Browser
open Feliz
open FantomasTools.Client

[<AllowNullLiteral>]
type MonacoIDisposable =
    abstract dispose: unit -> unit

[<AllowNullLiteral>]
type IMonacoEditor =
    abstract setSelection: obj -> unit
    abstract revealRangeInCenter: obj * int -> unit
    abstract onDidChangeCursorPosition: listener: (obj -> unit) -> MonacoIDisposable

[<RequireQualifiedAccess>]
type MonacoEditorProp =
    | Height of string
    | DefaultLanguage of string
    | Value of string
    | OnChange of (string -> unit)
    | OnMount of Action<IMonacoEditor, obj>
    | Options of obj

    static member rulerOption column =
        {| rulers = [| {| column = column; color = "#2FBADC" |} |] |} :> obj

let inline private MonacoEditor (props: MonacoEditorProp list) : ReactElement =
    ofImport "default" "@monaco-editor/react" (keyValueList CaseRules.LowerFirst props) []

let private useEventListener (target: Element, ``type``: string, listener: Event -> unit) =
    let subscribe () =
        target.addEventListener (``type``, listener)

        { new IDisposable with
            member this.Dispose() = target.removeEventListener (``type``, listener) }

    React.useEffect (subscribe, [| box target; box ``type``; box listener |])

let useEffect (_action: unit -> unit, _dependencies: obj array) : unit = import "useEffect" "react"

[<ReactComponent>]
let EditorAux (onCursorChanged: obj -> unit) (isReadOnly: bool) (props: MonacoEditorProp list) =
    let editorRef = React.useRef<IMonacoEditor> Unchecked.defaultof<IMonacoEditor>

    let isEditorMounted, setIsEditorMounted = React.useState false

    let changeCursorPosition, setChangeCursorPosition =
        React.useState<MonacoIDisposable> null

    let selectRange (ev: Event) =
        if not isReadOnly then
            let ev = ev :?> CustomEvent

            if (emitJsExpr (ev, editorRef.current) "$0 && $0.detail && $1") then
                let range = ev.detail
                let editor = editorRef.current
                editor.setSelection range
                editor.revealRangeInCenter (range, 0)

    useEventListener (window :?> Element, "select_range", selectRange)

    let handleEditorDidMount =
        Action<_, _>(fun editor _ ->
            editorRef.current <- editor
            setIsEditorMounted true)

    useEffect (
        fun () ->
            if not (isNullOrUndefined changeCursorPosition) then
                changeCursorPosition.dispose ()

            if not (isNullOrUndefined editorRef.current) then
                editorRef.current.onDidChangeCursorPosition onCursorChanged
                |> setChangeCursorPosition
        , [| isEditorMounted |]
    )

    let options =
        createObj
            [ "readOnly" ==> isReadOnly
              "domReadOnly" ==> isReadOnly
              "selectionHighlight" ==> false
              "occurrencesHighlight" ==> false
              "selectOnLineNumbers" ==> true
              "lineNumbers" ==> true
              "theme" ==> "vs-light"
              "renderWhitespace" ==> "all"
              "minimap" ==> createObj [ "enabled" ==> false ]
              "automaticLayout" ==> true ]

    let defaultProps: MonacoEditorProp list =
        [ MonacoEditorProp.Height "100%"
          MonacoEditorProp.DefaultLanguage "fsharp"
          MonacoEditorProp.Options options
          MonacoEditorProp.OnMount handleEditorDidMount ]

    MonacoEditor(defaultProps @ props)

let ReadOnlyEditor props = EditorAux ignore true props
let Editor props = EditorAux ignore false props

type EditorProp =
    | OnChange of (string -> unit)
    | Value of string
    | Language of string
    | IsReadOnly of bool
    | GetEditor of (obj -> unit)

let selectRange (range: Range) _ =
    let data =
        jsOptions<CustomEventInit> (fun o ->
            o.detail <-
                {| startColumn = range.StartColumn + 1
                   startLineNumber = range.StartLine
                   endLineNumber = range.EndLine
                   endColumn = range.EndColumn + 1 |})

    let event = CustomEvent.Create("select_range", data)
    Dom.window.dispatchEvent event |> ignore
