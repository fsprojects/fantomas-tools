module FantomasTools.Client.ASTViewer.Model

open ASTViewer.Shared

type Msg =
    | VersionFound of string
    | SetSourceText of string
    | DoParse
    | DoTypeCheck
    | ASTParsed of Dto
    | Error of string
    | ShowEditor
    | ShowRaw
    | DefinesUpdated of string
    | SetFsiFile of bool
    | HighLight of FantomasTools.Client.Editor.HighLightRange

type EditorState =
    | Loading
    | Loaded

type View =
    | Editor
    | Raw

type Model =
    { Source: string
      Defines: string
      IsFsi: bool
      Parsed: Result<Dto option, string>
      IsLoading: bool
      Version: string
      View: View
      FSharpEditorState: EditorState }
