module internal OakViewer.Encoders

open Thoth.Json.Net
open FSharp.Compiler.Text
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

let rec encodeNode (node: Node) (continuation: JsonValue -> JsonValue) : JsonValue =
    let continuations = List.map encodeNode (Array.toList node.Children)

    let text =
        match node with
        | :? SingleTextNode as stn ->
            if stn.Text.Length > 13 then
                sprintf "\"%s...\"" (stn.Text.Substring(0, 10))
            else
                $"\"{stn.Text}\""
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
