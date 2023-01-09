module internal OakViewer.Encoders

open Thoth.Json.Net
open FSharp.Compiler.Text
open Fantomas.Core
open Fantomas.Core.SyntaxOak

// Todo: we could encode the range as object if that appears to be useful.
let private rangeToText (m: range) =
    Encode.string $"(%i{m.StartLine},%i{m.StartColumn}-%i{m.EndLine},%i{m.EndColumn})"

let private encodeTriviaNode (triviaNode: TriviaNode) : JsonValue =
    let contentType, content =
        match triviaNode.Content with
        | CommentOnSingleLine comment -> "commentOnSingleLine", Some comment
        | LineCommentAfterSourceCode comment -> "lineCommentAfterSourceCode", Some comment
        | BlockComment(comment, _, _) -> "blockComment", Some comment
        | Newline -> "newline", None
        | Directive directive -> "directive", Some directive

    Encode.object
        [ "range", rangeToText triviaNode.Range
          "type", Encode.string contentType
          "content", Encode.option Encode.string content ]

let rec encodeNode (node: Node) (continuation: JsonValue -> JsonValue) : JsonValue =
    let continuations = List.map encodeNode (Array.toList node.Children)

    let finalContinuation (children: JsonValue list) =
        Encode.object
            [ "type", Encode.string (node.GetType().Name)
              "range", rangeToText node.Range
              "contentBefore", Encode.seq (Seq.map encodeTriviaNode node.ContentBefore)
              "children", Encode.list children
              "contentAfter", Encode.seq (Seq.map encodeTriviaNode node.ContentAfter) ]
        |> continuation

    Continuation.sequence continuations finalContinuation
