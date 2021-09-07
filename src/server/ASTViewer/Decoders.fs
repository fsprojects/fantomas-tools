module ASTViewer.Server.Decoders

open Thoth.Json.Net
open ASTViewer.Shared

let private decodeInput =
    Decode.object (fun get ->
        { SourceCode = get.Required.Field "sourceCode" Decode.string
          Defines = get.Required.Field "defines" (Decode.array Decode.string)
          IsFsi = get.Required.Field "isFsi" Decode.bool })

let decodeInputRequest json = Decode.fromString decodeInput json
