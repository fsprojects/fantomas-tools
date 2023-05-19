module internal OakViewer.Encoders

open Thoth.Json.Net
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open Fantomas.FCS.Parse
open Fantomas.Core
open Fantomas.Core.SyntaxOak

let private encodeRange (m: range) =
    Encode.object
        [ "startLine", Encode.int m.StartLine
          "startColumn", Encode.int m.StartColumn
          "endLine", Encode.int m.EndLine
          "endColumn", Encode.int m.EndColumn ]

let private encodeTriviaNode (triviaNode: TriviaNode) : JsonValue =
    let contentType, content =
        match triviaNode.Content with
        | CommentOnSingleLine comment -> "commentOnSingleLine", Some comment
        | LineCommentAfterSourceCode comment -> "lineCommentAfterSourceCode", Some comment
        | BlockComment(comment, _, _) -> "blockComment", Some comment
        | Newline -> "newline", None
        | Directive directive -> "directive", Some directive
        | Cursor -> "cursor", None

    Encode.object
        [ "range", encodeRange triviaNode.Range
          "type", Encode.string contentType
          "content", Encode.option Encode.string content ]

let rec private encodeNode (node: Node) (continuation: JsonValue -> JsonValue) : JsonValue =
    let continuations = List.map encodeNode (Array.toList node.Children)

    let text =
        match node with
        | :? SingleTextNode as stn ->
            if stn.Text.Length < 13 then
                stn.Text
            else
                sprintf "%s.." (stn.Text.Substring(0, 10))
            |> Some
        | _ -> None

    let finalContinuation (children: JsonValue list) =
        Encode.object
            [ "type", Encode.string (node.GetType().Name)
              "text", Encode.option Encode.string text
              "range", encodeRange node.Range
              "contentBefore", Encode.seq (Seq.map encodeTriviaNode node.ContentBefore)
              "children", Encode.list children
              "contentAfter", Encode.seq (Seq.map encodeTriviaNode node.ContentAfter) ]
        |> continuation

    Continuation.sequence continuations finalContinuation

let private mkRange (range: Range) : FantomasTools.Client.Range =
    { StartLine = range.StartLine
      StartColumn = range.StartColumn
      EndLine = range.EndLine
      EndColumn = range.EndColumn }

let private fsharpErrorInfoSeverity =
    function
    | FSharpDiagnosticSeverity.Warning -> "warning"
    | FSharpDiagnosticSeverity.Error -> "error"
    | FSharpDiagnosticSeverity.Hidden -> "hidden"
    | FSharpDiagnosticSeverity.Info -> "info"

let private encodeFSharpErrorInfo (info: FSharpParserDiagnostic) =
    ({ SubCategory = info.SubCategory
       Range =
         match info.Range with
         | None -> mkRange Range.Zero
         | Some r -> mkRange r
       Severity = fsharpErrorInfoSeverity info.Severity
       ErrorNumber = Option.defaultValue 0 info.ErrorNumber
       Message = info.Message }
    : FantomasTools.Client.Diagnostic)
    |> FantomasTools.Client.Diagnostic.Encode

let encode (root: Node) (diagnostics: FSharpParserDiagnostic list) =
    Encode.object
        [ "oak", encodeNode root id
          "diagnostics", Encode.list (List.map encodeFSharpErrorInfo diagnostics) ]
