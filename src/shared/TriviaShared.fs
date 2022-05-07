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
      ContentBefore: TriviaContent list
      ContentItself: TriviaContent option
      ContentAfter: TriviaContent list
      Range: Range }

type TriviaNodeCandidate =
    { Type: string
      Name: string
      Range: Range }

type Trivia = { Item: TriviaContent; Range: Range }

type ParseResult =
    { Trivia: Trivia list
      TriviaNodeCandidates: TriviaNodeCandidate list
      TriviaNodes: TriviaNode list }

type ParseRequest =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
