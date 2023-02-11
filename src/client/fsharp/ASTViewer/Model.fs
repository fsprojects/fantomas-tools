module FantomasTools.Client.ASTViewer.Model

type Msg =
    | VersionFound of string
    | SetSourceText of string
    | DoParse
    | ASTParsed of ASTViewer.Shared.Response
    | Error of string
    | DefinesUpdated of string
    | SetFsiFile of bool
    | HighLight of line: int * column: int

type EditorState =
    | Loading
    | Loaded

type Model =
    { Source: string
      Defines: string
      Parsed: Result<ASTViewer.Shared.Response, string> option
      IsLoading: bool
      Version: string
      FSharpEditorState: EditorState }
