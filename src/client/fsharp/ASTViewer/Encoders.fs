module FantomasTools.Client.ASTViewer.Encoders

open ASTViewer.Shared
open FantomasTools.Client
open Thoth.Json

let encodeUrlModel (bubble: BubbleModel) : JsonValue =
    Encode.object
        [ "defines", Encode.string bubble.Defines
          "isFsi", Encode.bool bubble.IsFsi
          "code", Encode.string bubble.SourceCode ]

let encodeInput (input: Request) =
    Encode.object
        [ "sourceCode", Encode.string input.SourceCode
          "defines", (Array.map Encode.string input.Defines |> Encode.array)
          "isFsi", Encode.bool input.IsFsi ]
    |> Encode.toString 2
