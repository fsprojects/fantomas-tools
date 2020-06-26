module FantomasTools.Client.FSharpTokens.Encoders

open FantomasTools.Client.FSharpTokens.Model
open Thoth.Json

let encodeGetTokensRequest (value: FSharpTokens.Shared.GetTokensRequest): JsonValue =
    Encode.object [ "defines",
                    (value.Defines
                     |> List.map Encode.string
                     |> List.toArray
                     |> Encode.array)
                    "sourceCode", Encode.string value.SourceCode ]

let encodeUrlModel code model: JsonValue =
    Encode.object [ "defines", Encode.string model.Defines
                    "code", Encode.string code ]
