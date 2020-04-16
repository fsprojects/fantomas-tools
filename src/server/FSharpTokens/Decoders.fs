module FSharpTokens.Server.Decoders

open FSharp.Compiler.SourceCodeServices

open Thoth.Json.Net
open FSharpTokens.Shared

let decodeTokenRequest: Decoder<GetTokensRequest> =
    Decode.object (fun get ->
        { Defines = get.Required.Field "defines" (Decode.list Decode.string)
          SourceCode = get.Required.Field "sourceCode" Decode.string })

let private decodeEnum<'t> (path: string) (token: JsonValue) =
    let v = token.Value<string>()
    match System.Enum.Parse(typeof<'t>, v, true) with
    | :? 't as t -> Ok t
    | _ ->
        let typeName = typeof<'t>.Name
        Error(DecoderError(sprintf "Cannot decode to %s" typeName, ErrorReason.BadField(path, token)))
