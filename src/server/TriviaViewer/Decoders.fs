module TriviaViewer.Server.Decoders

open Thoth.Json.Net
open TriviaViewer.Shared

let private parseRequestDecoder: Decoder<ParseRequest> =
    Decode.object (fun get ->
        { SourceCode = get.Required.Field "sourceCode" Decode.string
          Defines = get.Required.Field "defines" (Decode.array Decode.string)
          IsFsi = get.Required.Field "isFsi" Decode.bool })

let decodeParseRequest value = Decode.fromString parseRequestDecoder value
