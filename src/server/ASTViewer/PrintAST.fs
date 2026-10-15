module ASTViewer.PrintAST

open System
open Fantomas.FCS.Syntax
open Fantomas.FCS.Xml
open Fantomas.FCS.Text
open Fantomas.Core
open Fantomas.Core.SyntaxOak

let zeroRange = Range.range0
let stn v = SingleTextNode(v, zeroRange)

let identList values =
    match values with
    | [] -> IdentListNode([], zeroRange)
    | [ singleValue ] -> IdentListNode([ IdentifierOrDot.Ident(stn singleValue) ], zeroRange)
    | head :: tail ->
        let content =
            [
                yield IdentifierOrDot.Ident(stn head)
                for t in tail do
                    yield IdentifierOrDot.UnknownDot
                    yield IdentifierOrDot.Ident(stn t)
            ]

        IdentListNode(content, zeroRange)

let mkConstExpr v =
    SingleTextNode(v, zeroRange) |> Constant.FromText |> Expr.Constant

let mkStringExpr v = FSharpText.quote v |> mkConstExpr

let mkExprTuple xs =
    match xs with
    | []
    | [ _ ] -> failwith "No point in making a tuple out of less than two items"
    | head :: tail ->
        let content =
            [
                yield Choice1Of2 head
                for t in tail do
                    yield Choice2Of2(stn ",")
                    yield Choice1Of2 t
            ]

        ExprTupleNode(content, zeroRange) |> Expr.Tuple

let mkExprParen e =
    ExprParenNode(stn "(", e, stn ")", zeroRange) |> Expr.Paren

let mkInfix leftExpr operatorName rightExpr =
    ExprInfixAppNode(leftExpr, stn operatorName, rightExpr, zeroRange)
    |> Expr.InfixApp

let mkArrayExpr elements =
    ExprArrayOrListNode(stn "[|", elements, stn "|]", zeroRange) |> Expr.ArrayOrList

let mkListExpr elements =
    ExprArrayOrListNode(stn "[", elements, stn "]", zeroRange) |> Expr.ArrayOrList

let mkExprOptVarNode name =
    ExprOptVarNode(false, name, zeroRange) |> Expr.OptVar

let mkExprIdent v = Expr.Ident(stn v)

let mkExprAppSingleParenArgNode functionName argExpr =
    ExprAppSingleParenArgNode(mkExprIdent functionName, argExpr, zeroRange)
    |> Expr.AppSingleParenArg

let mkRangeExpr (r: range) : Expr =
    String.Concat("(", r.StartLine, ",", r.StartColumn, "--", r.EndLine, ",", r.EndColumn, ")")
    |> mkStringExpr
    |> mkExprParen
    |> mkExprAppSingleParenArgNode "R"

let mkPositionExpr (p: Position) : Expr =
    String.Concat(p.Line, ",", p.Column)
    |> mkStringExpr
    |> mkExprParen
    |> mkExprAppSingleParenArgNode "P"

let mkIdent (ident: Ident) : Expr =
    ExprTupleNode(
        [
            Choice1Of2(mkStringExpr ident.idText)
            Choice2Of2(stn ",")
            Choice1Of2(mkRangeExpr ident.idRange)
        ],
        zeroRange
    )
    |> Expr.Tuple
    |> mkExprParen
    |> mkExprAppSingleParenArgNode "Ident"

let mkXmlDoc (preXmlDoc: PreXmlDoc) : Expr =
    if preXmlDoc.IsEmpty then
        mkExprOptVarNode (identList [ "PreXmlDoc"; "Empty" ])
    else
        let xmlDoc = preXmlDoc.ToXmlDoc(false, None)

        let linesExpr =
            xmlDoc.UnprocessedLines
            |> Array.map (fun line -> mkStringExpr ("///" + line))
            |> Array.toList
            |> mkArrayExpr

        let rangeExpr = mkRangeExpr preXmlDoc.Range
        let argExpr = mkExprTuple [ linesExpr; rangeExpr ] |> mkExprParen

        ExprAppSingleParenArgNode(mkExprOptVarNode (identList [ "PreXmlDoc"; "Create" ]), argExpr, zeroRange)
        |> Expr.AppSingleParenArg

let mkCaseExpr (caseName: IdentListNode) (arg: Expr) : Expr =
    ExprAppSingleParenArgNode(mkExprOptVarNode caseName, mkExprParen arg, zeroRange)
    |> Expr.AppSingleParenArg

let rec map (expand: bool) (node: SyntaxNode) : Expr =
    match node with
    | SyntaxNode.Case(typeName, caseName, fields) ->
        let caseName =
            identList
                [
                    if expand then
                        yield typeName
                    yield caseName
                ]

        match fields with
        | [] -> mkExprOptVarNode caseName
        | [ _, value ] -> mkCaseExpr caseName (map expand value)
        | fields ->
            fields
            |> List.map (fun (fieldName, value) ->
                if expand then
                    mkInfix (mkConstExpr fieldName) "=" (map expand value)
                else
                    map expand value
            )
            |> mkExprTuple
            |> mkCaseExpr caseName
    | SyntaxNode.Record fields ->
        let recordFields =
            fields
            |> List.map (fun (fieldName, value) ->
                RecordFieldNode(identList [ fieldName ], stn "=", map expand value, zeroRange)
                |> ExprRecordFieldOrSpread.Field
            )

        ExprRecordNode(stn "{", None, recordFields, stn "}", zeroRange) |> Expr.Record
    | SyntaxNode.Tuple items -> items |> List.map (map expand) |> mkExprTuple |> mkExprParen
    | SyntaxNode.Option None -> Expr.Ident(stn "None")
    | SyntaxNode.Option(Some value) -> mkCaseExpr (identList [ "Some" ]) (map expand value)
    | SyntaxNode.List items -> items |> List.map (map expand) |> mkListExpr
    | SyntaxNode.Array items -> items |> List.map (map expand) |> mkArrayExpr
    | SyntaxNode.Set items ->
        ExprAppNode(mkConstExpr "set", [ items |> List.map (map expand) |> mkListExpr ], zeroRange)
        |> Expr.App
    | SyntaxNode.Ident ident -> mkIdent ident
    | SyntaxNode.Range range -> mkRangeExpr range
    | SyntaxNode.Position pos -> mkPositionExpr pos
    | SyntaxNode.XmlDoc preXmlDoc -> mkXmlDoc preXmlDoc
    | SyntaxNode.Literal text -> mkConstExpr text

let printAST (expand: bool) (ast: ParsedInput) : string =
    let expr = SyntaxReader.readParsedInput ast |> map expand

    let oak =
        Oak([], [ ModuleOrNamespaceNode(None, [ ModuleDecl.DeclExpr expr ], zeroRange) ], zeroRange)

    CodeFormatter.FormatOakAsync(
        oak,
        { FormatConfig.Default with
            MultilineBracketStyle = Stroustrup
            MaxLineLength = 200
        }
    )
    |> Async.RunSynchronously
