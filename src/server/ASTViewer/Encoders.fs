module ASTViewer.Server.Encoders

open Thoth.Json.Net
open FSharp.Compiler.SourceCodeServices

let private rangeEncoder (range: FSharp.Compiler.Text.Range) =
    Encode.object
        [ "startLine", Encode.int range.StartLine
          "startCol", Encode.int range.StartColumn
          "endLine", Encode.int range.EndLine
          "endCol", Encode.int range.EndColumn ]

let private idEncoder (id: Fantomas.AstTransformer.Id) =
    Encode.object
        [ "Ident", Encode.string id.Ident
          "Range", Encode.option rangeEncoder id.Range ]

let private encodeKeyValue (k, v: obj) =
    let (|IsList|_|) (candidate: obj) =
        let t = candidate.GetType()

        if t.IsGenericType
           && t.GetGenericTypeDefinition() = typedefof<list<_>> then
            Some(candidate :?> System.Collections.IEnumerable)
        else
            None

    let rec encodeValue (v: obj) =
        match v with
        | null -> Encode.nil
        | :? bool as b -> Encode.bool b
        | :? string as s -> Encode.string s
        | :? int as i -> Encode.int i
        | :? uint16 as ui -> Encode.uint16 ui
        | :? FSharp.Compiler.Text.Range as r -> rangeEncoder r
        | :? Fantomas.AstTransformer.Id as id -> idEncoder id
        | :? FSharp.Compiler.SyntaxTree.SynModuleOrNamespaceKind as kind ->
            match kind with
            | FSharp.Compiler.SyntaxTree.SynModuleOrNamespaceKind.AnonModule -> Encode.string "AnonModule"
            | FSharp.Compiler.SyntaxTree.SynModuleOrNamespaceKind.DeclaredNamespace -> Encode.string "DeclaredNamespace"
            | FSharp.Compiler.SyntaxTree.SynModuleOrNamespaceKind.GlobalNamespace -> Encode.string "GlobalNamespace"
            | FSharp.Compiler.SyntaxTree.SynModuleOrNamespaceKind.NamedModule -> Encode.string "NamedModule"
        | :? (ref<bool>) as r -> encodeValue r.Value
        | :? (option<FSharp.Compiler.Text.Range>) as o -> Encode.option encodeValue o
        | IsList l ->
            l
            |> Seq.cast
            |> Seq.toList
            |> List.map encodeValue
            |> Encode.list
        | meh ->
            printfn "unsupported typeof %A" (meh.GetType())
            Encode.nil

    encodeValue v |> fun ev -> (k, ev)

let rec astNodeEncoder (node: Fantomas.AstTransformer.Node) =
    let properties =
        Map.toList node.Properties
        |> List.map encodeKeyValue
        |> Encode.object

    Encode.object
        [ "type", Encode.string (node.Type.ToString())
          "range", Encode.option rangeEncoder node.Range
          "properties", properties
          "childs", Encode.array (Array.map astNodeEncoder (Array.ofList node.Childs)) ]

let rec tastNodeEncoder (node: TastTransformer.Tast.Node) =
    let properties =
        Map.toList node.Properties
        |> List.map encodeKeyValue
        |> Encode.object

    Encode.object
        [ "type", Encode.string node.Type
          "range", Encode.option rangeEncoder node.Range
          "properties", properties
          "childs", Encode.array (Array.map tastNodeEncoder (Array.ofList node.Childs)) ]

let private encodeFSharpErrorInfoSeverity =
    function
    | FSharpDiagnosticSeverity.Warning -> Encode.string "warning"
    | FSharpDiagnosticSeverity.Error -> Encode.string "error"
    | FSharpDiagnosticSeverity.Hidden -> Encode.string "hidden"
    | FSharpDiagnosticSeverity.Info -> Encode.string "info"

let private encodeFSharpErrorInfo (info: FSharpDiagnostic) =
    Encode.object
        [ "subcategory", Encode.string info.Subcategory
          "range", rangeEncoder info.Range
          "severity", encodeFSharpErrorInfoSeverity info.Severity
          "errorNumber", Encode.int info.ErrorNumber
          "message", Encode.string info.Message ]

let encodeResponse nodeJson string (errors: FSharpDiagnostic array) =
    let errors =
        Array.map encodeFSharpErrorInfo errors
        |> Encode.array

    Encode.object
        [ "node", nodeJson
          "string", Encode.string string
          "errors", errors ]
