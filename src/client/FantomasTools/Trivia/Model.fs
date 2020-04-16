module FantomasTools.Client.Trivia.Model

open TriviaViewer.Shared

type ActiveTab =
    | ByTriviaNodes
    | ByTrivia

type Model =
    { ActiveTab: ActiveTab
      Trivia: Trivia list
      TriviaNodes: TriviaNode list
      Exception: exn option
      IsLoading: bool
      ActiveByTriviaNodeIndex: int
      ActiveByTriviaIndex: int
      Defines: string
      FSCVersion: string
      IsFsi: bool }

type UrlModel = { IsFsi: bool; Defines: string }

type Msg =
    | SelectTab of ActiveTab
    | GetTrivia
    | TriviaReceived of ParseResult
    | ActiveItemChange of ActiveTab * int
    | UpdateDefines of string
    | FSCVersionReceived of string
    | SetFsiFile of bool
    | NetworkError of exn
