module ASTViewer.ExpandedAST

open FSharp.Compiler.Syntax

/// Process the ParsedInput tree using reflection to produce a rich to string representation.
/// This string comes very close to usable input if one were to manually construct the AST.
/// <remarks>Could be slow for large trees and might have some tail recursion problems.</remarks>
val getExpandedAST: ast: ParsedInput -> string
