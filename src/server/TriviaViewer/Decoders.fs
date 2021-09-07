module TriviaViewer.Server.Decoders

open Thoth.Json.Net
open TriviaViewer.Shared

let private parseRequestDecoder: Decoder<ParseRequest> =
    Decode.object (fun get ->
        { SourceCode = get.Required.Field "sourceCode" Decode.string
          Defines = get.Required.Field "defines" (Decode.list Decode.string)
          FileName = get.Required.Field "fileName" Decode.string })

let decodeParseRequest value = Decode.fromString parseRequestDecoder value
