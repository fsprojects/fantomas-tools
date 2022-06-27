module TriviaViewer.Shared

type Range =
    { StartLine: int
      StartColumn: int
      EndLine: int
      EndColumn: int }

type Comment =
    | LineCommentAfterSourceCode of comment: string
    | LineCommentOnSingleLine of comment: string
    | BlockComment of string * newlineBefore: bool * newlineAfter: bool

type TriviaContent =
    | Comment of Comment
    | Newline
    | Directive of directive: string

type TriviaNode =
    { Type: string
      Range: Range
      Children: TriviaNode array }

type Trivia = { Item: TriviaContent; Range: Range }

type TriviaInstruction =
    { Trivia: Trivia
      Type: string
      Range: Range
      AddBefore: bool }

type ParseResult =
    { Trivia: Trivia list
      RootNode: TriviaNode
      TriviaInstructions: TriviaInstruction list }

type ParseRequest =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
