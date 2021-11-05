module ASTViewer.GetAST

open FSharp.Compiler.CodeAnalysis

let getVersion () =

    let assembly = typeof<FSharpChecker>.Assembly

    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision
