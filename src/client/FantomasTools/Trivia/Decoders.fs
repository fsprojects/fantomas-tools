module FantomasTools.Client.Trivia.Decoders

open Thoth.Json
open TriviaViewer.Shared

let private decodeRange =
    Decode.object (fun get ->
        { StartLine = get.Required.Field "startLine" Decode.int
          StartColumn = get.Required.Field "startColumn" Decode.int
          EndLine = get.Required.Field "endLine" Decode.int
          EndColumn = get.Required.Field "endColumn" Decode.int })

let private decodeTriviaContent = Decode.Auto.generateDecoderCached<TriviaContent>()

let private decodeTrivia: Decoder<Trivia> =
    Decode.object (fun get ->
        { Item = get.Required.Field "item" decodeTriviaContent
          Range = get.Required.Field "range" decodeRange })

let private decodeTriviaNodeType = Decode.Auto.generateDecoderCached<TriviaNodeType>()

let private decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" decodeTriviaNodeType
          ContentBefore = get.Required.Field "contentBefore" (Decode.list decodeTriviaContent)
          ContentItself = get.Required.Field "contentItself" (Decode.option decodeTriviaContent)
          ContentAfter = get.Required.Field "contentAfter" (Decode.list decodeTriviaContent)
          Range = get.Required.Field "range" decodeRange })

let private decodeParseResult: Decoder<ParseResult> =
    Decode.object (fun get ->
        { Trivia = get.Required.Field "trivia" (Decode.list decodeTrivia)
          TriviaNodes = get.Required.Field "triviaNodes" (Decode.list decodeTriviaNode) })

let decodeResult json =
    Decode.fromString decodeParseResult json

let decodeParseRequest: Decoder<ParseRequest> =
    Decode.object (fun get ->
        let source =
            get.Optional.Field "sourceCode" Decode.string |> Option.defaultValue System.String.Empty
        let defines =
            get.Optional.Field "defines" (Decode.list Decode.string) |> Option.defaultValue []
        let fileName =
            get.Optional.Field "fileName" (Decode.string) |> Option.defaultValue "script.fsx"
        let keepNewlineAfter =
            get.Optional.Field "keepNewlineAfter" (Decode.bool) |> Option.defaultValue false
        { SourceCode = source
          Defines = defines
          FileName = fileName
          KeepNewlineAfter = keepNewlineAfter })

let decodeVersion json = Decode.fromString Decode.string json