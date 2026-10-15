namespace ASTViewer

open System
open System.Globalization
open Fantomas.FCS.Syntax
open Fantomas.FCS.Text
open Fantomas.FCS.Xml

/// The untyped syntax tree as plain data, so it can be printed without F# reflection.
/// The generated SyntaxReader builds it, PrintAST prints it.
[<RequireQualifiedAccess>]
type SyntaxNode =
    /// A union case with its fields. A field without a name in SyntaxTree.fsi gets the name
    /// reflection would give it: `Item` for a single field, `Item1`, `Item2`, ... otherwise.
    | Case of typeName: string * caseName: string * fields: (string * SyntaxNode) list
    | Record of fields: (string * SyntaxNode) list
    | Tuple of items: SyntaxNode list
    | Option of value: SyntaxNode option
    | List of items: SyntaxNode list
    | Array of items: SyntaxNode list
    | Set of items: SyntaxNode list
    | Ident of ident: Ident
    | Range of range: range
    | Position of pos: pos
    | XmlDoc of xmlDoc: PreXmlDoc
    /// A value that is already F# source text, for example `true`, `1L` or `"a"`.
    | Literal of text: string

module FSharpText =
    /// The value as an F# string literal.
    let quote (value: string) : string =
        let escaped =
            value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r")

        "\"" + escaped + "\""

/// The leaves of the tree: the types the generated reader does not look inside.
/// GenerateSyntaxReader.fsx maps a type it does not find in the syntax files to the function here with the
/// same name, starting with a lowercase letter: `Ident` to `Leaf.ident`.
module Leaf =
    let private invariant (value: #IFormattable) (format: string) : string =
        value.ToString(format, CultureInfo.InvariantCulture)

    let private withDecimalPoint (text: string) : string =
        if text |> Seq.exists (fun c -> not (Char.IsDigit c || c = '-')) then
            text
        else
            text + ".0"

    let ident (value: Ident) : SyntaxNode = SyntaxNode.Ident value
    let range (value: range) : SyntaxNode = SyntaxNode.Range value
    let pos (value: pos) : SyntaxNode = SyntaxNode.Position value
    let preXmlDoc (value: PreXmlDoc) : SyntaxNode = SyntaxNode.XmlDoc value

    let string (value: string) : SyntaxNode = SyntaxNode.Literal(FSharpText.quote value)

    let bool (value: bool) : SyntaxNode =
        SyntaxNode.Literal(if value then "true" else "false")

    let char (value: char) : SyntaxNode =
        let text =
            match value with
            | '\'' -> "\\'"
            | '\\' -> "\\\\"
            | '\n' -> "\\n"
            | '\r' -> "\\r"
            | '\t' -> "\\t"
            | c when Char.IsControl c -> "\\u" + (int c).ToString("x4")
            | c -> String(c, 1)

        SyntaxNode.Literal("'" + text + "'")

    let sbyte (value: sbyte) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "y")
    let byte (value: byte) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "uy")
    let int16 (value: int16) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "s")
    let uint16 (value: uint16) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "us")
    let int (value: int) : SyntaxNode = SyntaxNode.Literal(invariant value "D")
    let int32 (value: int32) : SyntaxNode = int value
    let uint32 (value: uint32) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "u")
    let int64 (value: int64) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "L")
    let uint64 (value: uint64) : SyntaxNode = SyntaxNode.Literal(invariant value "D" + "UL")

    let single (value: single) : SyntaxNode =
        SyntaxNode.Literal(withDecimalPoint (invariant value "R") + "f")

    let double (value: double) : SyntaxNode =
        SyntaxNode.Literal(withDecimalPoint (invariant value "R"))

    let decimal (value: decimal) : SyntaxNode = SyntaxNode.Literal(invariant value "G" + "M")

    /// Only `SynExpr.LibraryOnlyILAssembly` holds an `obj`: IL instructions the syntax tree keeps hidden.
    let obj (_: obj) : SyntaxNode = SyntaxNode.Literal "<obj>"
