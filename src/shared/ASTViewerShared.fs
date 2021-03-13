module ASTViewer.Shared

type Range =
    { StartLine: int
      StartCol: int
      EndLine: int
      EndCol: int }

type ASTError =
    { SubCategory: string
      Range: Range
      Severity: string
      ErrorNumber: int
      Message: string }

type Response =
    { String: string
      Errors: ASTError array }

type Request =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
