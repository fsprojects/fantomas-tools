module FantomasTools.Client.Trivia.Model

open TriviaViewer.Shared

[<RequireQualifiedAccess>]
type ActiveTab =
    | TriviaInstructions
    | Trivia
    | RootNode

type Model =
    { ActiveTab: ActiveTab
      Trivia: Trivia list
      RootNode: TriviaNode
      TriviaInstructions: TriviaInstruction list
      Error: string option
      IsLoading: bool
      ActiveByTriviaInstructionIndex: int
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
