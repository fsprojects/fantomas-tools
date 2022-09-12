module FantomasTools.Client.ASTViewer.Encoders

open ASTViewer.Shared
open FantomasTools.Client.ASTViewer.Model
open Thoth.Json

let encodeUrlModel code isFsi model : JsonValue =
    Encode.object
        [ "defines", Encode.string model.Defines
          "isFsi", Encode.bool isFsi
          "code", Encode.string code ]

let encodeInput (input: Request) =
    Encode.object
        [ "sourceCode", Encode.string input.SourceCode
          "defines", (Array.map Encode.string input.Defines |> Encode.array)
          "isFsi", Encode.bool input.IsFsi ]
    |> Encode.toString 2
