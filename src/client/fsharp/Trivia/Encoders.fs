module FantomasTools.Client.Trivia.Encoders

open Thoth.Json
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model

let encodeParseRequest (pr: ParseRequest) =
    Encode.object
        [ "sourceCode", Encode.string pr.SourceCode
          "defines", Array.map Encode.string pr.Defines |> Encode.array
          "isFsi", Encode.bool pr.IsFsi ]
    |> Encode.toString 4

let encodeUrlModel code (model: Model) =
    Encode.object
        [ "code", Encode.string code // the "code" key is a convention
          "defines", Encode.string model.Defines
          "isFsi", Encode.bool model.IsFsi ]
