module TriviaViewer.Server.Encoders

open FSharp.Compiler.Text
open Fantomas.Core
open Thoth.Json.Net
open Fantomas.Core.TriviaTypes
open TriviaViewer

let private mapToComment comment =
    match comment with
    | LineCommentAfterSourceCode c -> Shared.LineCommentAfterSourceCode c
    | CommentOnSingleLine c -> Shared.LineCommentOnSingleLine c
    | BlockComment(c, nb, na) -> Shared.BlockComment(c, nb, na)

let private mapToTriviaContent (tc: TriviaContent) =
    match tc with
    | Comment c -> Shared.Comment(mapToComment c)
    | Newline -> Shared.Newline
    | Directive d -> Shared.Directive d

let private triviaContentEncoder =
    Encode.Auto.generateEncoder<Shared.TriviaContent> ()

let private encodeTriviaContent = mapToTriviaContent >> triviaContentEncoder

let private encodeRange (range: Range) =
    Encode.object
        [ "startLine", Encode.int range.Start.Line
          "startColumn", Encode.int range.Start.Column
          "endLine", Encode.int range.End.Line
          "endColumn", Encode.int range.End.Column ]

let rec private encodeTriviaNode (tn: TriviaNode) =
    Encode.object
        [ "type", Encode.string (string tn.Type)
          "range", encodeRange tn.Range
          "children", Encode.array (Array.map encodeTriviaNode tn.Children) ]

let private encodeTrivia (t: Trivia) =
    Encode.object [ "item", encodeTriviaContent t.Item; "range", encodeRange t.Range ]

let private encodeTriviaInstruction (ti: TriviaInstruction) =
    Encode.object
        [ "trivia", encodeTrivia ti.Trivia
          "type", Encode.string (string ti.Type)
          "range", encodeRange ti.Range
          "addBefore", Encode.bool ti.AddBefore ]

let internal encodeParseResult
    (trivia: Trivia list)
    (rootNode: TriviaNode)
    (triviaInstructions: TriviaInstruction list)
    =
    Encode.object
        [ "trivia", List.map encodeTrivia trivia |> Encode.list
          "rootNode", encodeTriviaNode rootNode
          "triviaInstructions", List.map encodeTriviaInstruction triviaInstructions |> Encode.list ]
    |> Encode.toString 4
