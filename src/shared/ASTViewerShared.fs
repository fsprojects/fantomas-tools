module ASTViewer.Shared

open FantomasTools.Client

type Response =
    { String: string
      Errors: Diagnostic array }

type Request =
    { SourceCode: string
      Defines: string array
      IsFsi: bool }
