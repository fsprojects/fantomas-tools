module FantomasTools.Client.Trivia.Model

open TriviaViewer.Shared

type ActiveTab =
    | ByTriviaNodes
    | ByTrivia
    | ByTriviaNodeCandidates

type Model =
    { ActiveTab: ActiveTab
      Trivia: Trivia list
      TriviaNodeCandidates: TriviaNodeCandidate list
      TriviaNodes: TriviaNode list
      Error: string option
      IsLoading: bool
      ActiveByTriviaNodeIndex: int
      ActiveByTriviaIndex: int
      Defines: string
      Version: string
      IsFsi: bool }

type UrlModel = { IsFsi: bool; Defines: string }

type Msg =
    | SelectTab of ActiveTab
    | GetTrivia
    | TriviaReceived of ParseResult
    | ActiveItemChange of ActiveTab * int
    | DefinesUpdated of string
    | FSCVersionReceived of string
    | SetFsiFile of bool
    | Error of string
    | HighLight of FantomasTools.Client.Editor.HighLightRange
