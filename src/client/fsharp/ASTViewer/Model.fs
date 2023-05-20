module FantomasTools.Client.ASTViewer.Model

open FantomasTools.Client

type Msg =
    | Bubble of BubbleMessage
    | VersionFound of string
    | DoParse
    | ASTParsed of ASTViewer.Shared.Response
    | Error of string
    | SetExpand of value: bool

[<RequireQualifiedAccess>]
type AstViewerTabState =
    | Loading
    | Result of ASTViewer.Shared.Response
    | Error of string

type Model =
    { State: AstViewerTabState
      Version: string
      Expand: bool }
