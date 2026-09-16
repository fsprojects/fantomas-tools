module OakViewer.Server.Decoders

open Thoth.Json.Core
open Thoth.Json.System.Text.Json
open OakViewer

let private parseRequestDecoder: Decoder<ParseRequest> =
    Decode.object (fun get ->
        {
            SourceCode = get.Required.Field "sourceCode" Decode.string
            Defines = get.Required.Field "defines" (Decode.array Decode.string)
            IsFsi = get.Required.Field "isFsi" Decode.bool
        }
    )

let decodeParseRequest value = Decode.fromString parseRequestDecoder value
