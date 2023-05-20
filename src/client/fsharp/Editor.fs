module FantomasTools.Client.Editor

open System
open Fable.Core.JsInterop
open Fable.Core
open Fable.React
open Browser.Types
open Feliz
open FantomasTools.Client

[<AllowNullLiteral>]
type MonacoIDisposable =
    abstract dispose: unit -> unit

type IDimension =
    abstract width: int
    abstract height: int

[<AllowNullLiteral>]
type IMonacoEditor =
    abstract setSelection: obj -> unit
    abstract revealRangeInCenter: obj * int -> unit
    abstract onDidChangeCursorPosition: listener: (obj -> unit) -> MonacoIDisposable
    abstract layout: IDimension -> unit

[<RequireQualifiedAccess>]
type MonacoEditorProp =
    | Height of string
    | DefaultLanguage of string
    | Value of string
    | OnChange of (string -> unit)
    | OnMount of Action<IMonacoEditor, obj>
    | Options of obj
    | Theme of string
    | ClassName of string

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

let private theme =
    emitJsExpr<string> () "(window.matchMedia(\"(prefers-color-scheme: dark)\").matches ? \"vs-dark\" : \"vs-light\")"

let private editorOptions =
    {| selectionHighlight = false
       occurrencesHighlight = false
       selectOnLineNumbers = true
       lineNumbers = true
       renderWhitespace = "all"
       minimap = {| enabled = false |}
       automaticLayout = true |}

/// The main editor where the user will input the code
[<ReactComponent>]
let InputEditor (onChange: string -> unit) (value: string) (maxLineLength: int) (highlight: Range) =
    let editorRef = React.useRef<IMonacoEditor> Unchecked.defaultof<IMonacoEditor>

    let isEditorMounted, setIsEditorMounted = React.useState false

    let handleEditorDidMount =
        Action<_, _>(fun editor _ ->
            editorRef.current <- editor
            setIsEditorMounted true)

    let options =
        {| editorOptions with
            rulers =
                [| {| column = maxLineLength
                      color = "#2FBADC" |} |] |}

    useEffect (
        fun () ->
            if not (isNullOrUndefined editorRef.current) then
                let selection =
                    {| startColumn = highlight.StartColumn + 1
                       startLineNumber = highlight.StartLine
                       endLineNumber = highlight.EndLine
                       endColumn = highlight.EndColumn + 1 |}

                editorRef.current.setSelection selection
                editorRef.current.revealRangeInCenter (selection, 0)
        , [| isEditorMounted; highlight |]
    )

    MonacoEditor
        [ MonacoEditorProp.Theme theme
          MonacoEditorProp.Height "100%"
          MonacoEditorProp.DefaultLanguage "fsharp"
          MonacoEditorProp.OnChange onChange
          MonacoEditorProp.Value value
          MonacoEditorProp.Options options
          MonacoEditorProp.OnMount handleEditorDidMount ]

/// The hidden editor.
/// We always want React to render this editor for performance reasons.
/// We hide it by settings the height to 0.
[<ReactComponent>]
let HiddenEditor () =
    MonacoEditor [ MonacoEditorProp.Height "0%"; MonacoEditorProp.ClassName "hidden-editor" ]

[<ReactComponent>]
let ReadOnlyEditor (value: string) =
    let options =
        {| editorOptions with
            readOnly = true
            domReadOnly = true |}

    MonacoEditor
        [ MonacoEditorProp.Theme theme
          MonacoEditorProp.Height "100%"
          MonacoEditorProp.Value value
          MonacoEditorProp.Options options ]

[<ReactComponent>]
let AstResultEditor onCursorChanged value =
    let editorRef = React.useRef<IMonacoEditor> Unchecked.defaultof<IMonacoEditor>

    let isEditorMounted, setIsEditorMounted = React.useState false

    let changeCursorPosition, setChangeCursorPosition =
        React.useState<MonacoIDisposable> null

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
        {| editorOptions with
            readOnly = true
            domReadOnly = true |}

    MonacoEditor
        [ MonacoEditorProp.Theme theme
          MonacoEditorProp.Height "100%"
          MonacoEditorProp.DefaultLanguage "fsharp"
          MonacoEditorProp.Value value
          MonacoEditorProp.Options options
          MonacoEditorProp.OnMount handleEditorDidMount ]

[<ReactComponent>]
let FantomasResultEditor (value: string) =
    let options =
        {| editorOptions with
            readOnly = true
            domReadOnly = true |}

    MonacoEditor
        [ MonacoEditorProp.Theme theme
          MonacoEditorProp.Height "100%"
          MonacoEditorProp.Value value
          MonacoEditorProp.Options options
          MonacoEditorProp.DefaultLanguage "fsharp" ]
