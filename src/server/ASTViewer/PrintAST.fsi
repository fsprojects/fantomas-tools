module ASTViewer.PrintAST

open Fantomas.FCS.Syntax

/// Print the ParsedInput tree as F# code, read with the generated SyntaxReader instead of reflection.
/// The expanded form adds the type name to each union case and the name to each field of a case.
/// It comes very close to usable input if one were to construct the AST manually.
/// <remarks>Could be slow for large trees and might have some tail recursion problems.</remarks>
val printAST: expand: bool -> ast: ParsedInput -> string
