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

let rec private decodeTriviaNode path jsonValue : Result<TriviaNode, DecoderError> =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Range = get.Required.Field "range" decodeRange
              Children = get.Required.Field "children" (Decode.array decodeTriviaNode) })
        path
        jsonValue

let private decodeTriviaInstruction: Decoder<TriviaInstruction> =
    Decode.object (fun get ->
        { Trivia = get.Required.Field "trivia" decodeTrivia
          Type = get.Required.Field "type" Decode.string
          Range = get.Required.Field "range" decodeRange
          AddBefore = get.Required.Field "addBefore" Decode.bool })

let private decodeParseResult: Decoder<ParseResult> =
    Decode.object (fun get ->
        { Trivia = get.Required.Field "trivia" (Decode.list decodeTrivia)
          RootNode = get.Required.Field "rootNode" decodeTriviaNode
          TriviaInstructions = get.Required.Field "triviaInstructions" (Decode.list decodeTriviaInstruction) })

let decodeResult json = Decode.fromString decodeParseResult json

let decodeVersion json = Decode.fromString Decode.string json

let decodeUrlModel (initialModel: Model) : Decoder<Model> =
    Decode.object (fun get ->
        let defines = get.Optional.Field "defines" Decode.string |> Option.defaultValue ""

        { initialModel with
            Defines = defines })
