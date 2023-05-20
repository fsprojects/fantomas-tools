module FantomasTools.Client.ASTViewer.Encoders

open ASTViewer.Shared
open FantomasTools.Client
open Thoth.Json

let encodeUrlModel expand (bubble: BubbleModel) : JsonValue =
    Encode.object
        [ "defines", Encode.string bubble.Defines
          "isFsi", Encode.bool bubble.IsFsi
          "code", Encode.string bubble.SourceCode
          "expand", Encode.bool expand ]

let encodeInput (input: Request) =
    Encode.object
        [ "sourceCode", Encode.string input.SourceCode
          "defines", (Array.map Encode.string input.Defines |> Encode.array)
          "isFsi", Encode.bool input.IsFsi
          "expand", Encode.bool input.Expand ]
    |> Encode.toString 2
