module FantomasTools.Client.ASTViewer.Model

type Msg =
    | VersionFound of string
    | SetSourceText of string
    | DoParse
    | ASTParsed of ASTViewer.Shared.Response
    | Error of string
    | DefinesUpdated of string
    | SetFsiFile of bool
    | HighLight of FantomasTools.Client.Editor.HighLightRange

type EditorState =
    | Loading
    | Loaded

type Model =
    { Source: string
      Defines: string
      IsFsi: bool
      Parsed: Result<ASTViewer.Shared.Response, string> option
      IsLoading: bool
      Version: string
      FSharpEditorState: EditorState }
