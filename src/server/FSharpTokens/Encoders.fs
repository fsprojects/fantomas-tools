module FSharpTokens.Server.Encoders

open Thoth.Json.Net
open FSharp.Compiler.Tokenization
open Fantomas.TriviaTypes

let private encodeEnum<'t> (value: 't) = value.ToString() |> Encode.string

let private encodeTokenInfo (token: FSharpTokenInfo) =
    Encode.object
        [ "colorClass", encodeEnum token.ColorClass
          "charClass", encodeEnum token.CharClass
          "fsharpTokenTriggerClass", encodeEnum token.FSharpTokenTriggerClass
          "tokenName", Encode.string token.TokenName
          "leftColumn", Encode.int token.LeftColumn
          "rightColumn", Encode.int token.RightColumn
          "tag", Encode.int token.Tag
          "fullMatchedLength", Encode.int token.FullMatchedLength ]

let private encodeToken (token: Token) =
    Encode.object
        [ "tokenInfo", encodeTokenInfo token.TokenInfo
          "lineNumber", Encode.int token.LineNumber
          "content", Encode.string token.Content ]

let toJson (tokens: Token list) =
    tokens
    |> List.map encodeToken
    |> Encode.list
    |> Encode.toString 4
