module FantomasTools.Client.ASTViewer.Model

open FantomasTools.Client

type Msg =
    | Bubble of BubbleMessage
    | VersionFound of string
    | DoParse
    | ASTParsed of ASTViewer.Shared.Response
    | Error of string

type Model =
    { Parsed: Result<ASTViewer.Shared.Response, string> option
      Version: string }
