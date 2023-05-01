module FantomasTools.Client.ASTViewer.Model

open FantomasTools.Client

type Msg =
    | Bubble of BubbleMessage
    | VersionFound of string
    | DoParse
    | ASTParsed of ASTViewer.Shared.Response
    | Error of string

[<RequireQualifiedAccess>]
type AstViewerTabState =
    | Initial
    | Result of ASTViewer.Shared.Response
    | Error of string

type Model =
    { State: AstViewerTabState
      Version: string }
