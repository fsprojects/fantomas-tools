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

type ASTError =
    { SubCategory: string
      Range: Range
      Severity: string
      ErrorNumber: int
      Message: string }

type Dto =
    { Node: Node
      String: string
      Errors: ASTError array }

type Input =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
