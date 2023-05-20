module ASTViewer.Shared

open FantomasTools.Client

type Response =
    { Ast: string
      Diagnostics: Diagnostic array }

type Request =
    { SourceCode: string
      Defines: string array
      IsFsi: bool
      Expand: bool }
