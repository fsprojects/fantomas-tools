module FantomasTools.Client.FSharpTokens.Decoders

open FantomasTools.Client.FSharpTokens.Model
open Thoth.Json

let private decodeTokenInfo: Decoder<TokenInfo> =
    Decode.object (fun get ->
        { ColorClass = get.Required.Field "colorClass" Decode.string
          CharClass = get.Required.Field "charClass" Decode.string
          FSharpTokenTriggerClass = get.Required.Field "fsharpTokenTriggerClass" Decode.string
          TokenName = get.Required.Field "tokenName" Decode.string
          LeftColumn = get.Required.Field "leftColumn" Decode.int
          RightColumn = get.Required.Field "rightColumn" Decode.int
          Tag = get.Required.Field "tag" Decode.int
          FullMatchedLength = get.Required.Field "fullMatchedLength" Decode.int })

let private decodeToken: Decoder<Token> =
    Decode.object (fun get ->
        { TokenInfo = get.Required.Field "tokenInfo" decodeTokenInfo
          LineNumber = get.Required.Field "lineNumber" Decode.int
          Content = get.Required.Field "content" Decode.string })

let decodeTokens json =
    Decode.fromString (Decode.array decodeToken) json

let decodeUrlModel initialModel: Decoder<Model> =
    Decode.object (fun get ->
        { initialModel with
              Defines = get.Required.Field "defines" Decode.string })
