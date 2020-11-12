module FantomasTools.Client.Trivia.Decoders

open Thoth.Json
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model

let private decodeRange =
    Decode.object (fun get ->
        { StartLine = get.Required.Field "startLine" Decode.int
          StartColumn = get.Required.Field "startColumn" Decode.int
          EndLine = get.Required.Field "endLine" Decode.int
          EndColumn = get.Required.Field "endColumn" Decode.int })

let private decodeTriviaContent =
    Decode.Auto.generateDecoderCached<TriviaContent> ()

let private decodeTrivia: Decoder<Trivia> =
    Decode.object (fun get ->
        { Item = get.Required.Field "item" decodeTriviaContent
          Range = get.Required.Field "range" decodeRange })

let private decodeTriviaNodeType =
    Decode.Auto.generateDecoderCached<TriviaNodeType> ()

let private decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" decodeTriviaNodeType
          ContentBefore = get.Required.Field "contentBefore" (Decode.list decodeTriviaContent)
          ContentItself = get.Required.Field "contentItself" (Decode.option decodeTriviaContent)
          ContentAfter = get.Required.Field "contentAfter" (Decode.list decodeTriviaContent)
          Range = get.Required.Field "range" decodeRange })

let private decodeTriviaNodeCandidate: Decoder<TriviaNodeCandidate> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" Decode.string
          Name = get.Required.Field "name" Decode.string
          Range = get.Required.Field "range" decodeRange })

let private decodeParseResult: Decoder<ParseResult> =
    Decode.object (fun get ->
        { Trivia = get.Required.Field "trivia" (Decode.list decodeTrivia)
          TriviaNodeCandidates = get.Required.Field "triviaNodeCandidates" (Decode.list decodeTriviaNodeCandidate)
          TriviaNodes = get.Required.Field "triviaNodes" (Decode.list decodeTriviaNode) })

let decodeResult json = Decode.fromString decodeParseResult json

let decodeVersion json = Decode.fromString Decode.string json

let decodeUrlModel (initialModel: Model): Decoder<Model> =
    Decode.object (fun get ->
        let defines =
            get.Optional.Field "defines" (Decode.string)
            |> Option.defaultValue ""

        let isFsi =
            get.Optional.Field "isFsi" (Decode.bool)
            |> Option.defaultValue initialModel.IsFsi

        { initialModel with
              Defines = defines
              IsFsi = isFsi })
