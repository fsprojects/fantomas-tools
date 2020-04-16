module ASTViewer.Shared

type Range =
    { StartLine: int
      StartCol: int
      EndLine: int
      EndCol: int }

type Id = { Ident: string; Range: Range option }

type Node =
    { Type: string
      Range: Range option
      Properties: obj
      Childs: Node array }

type Dto = { Node: Node; String: string }

type Input =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
