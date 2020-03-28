module FantomasTools.Client.Trivia.Encoders

open Thoth.Json
open TriviaViewer.Shared

let encodeParseRequest pr =
    Encode.object
        [ "sourceCode", Encode.string pr.SourceCode
          "defines", List.map Encode.string pr.Defines |> Encode.list
          "fileName", Encode.string pr.FileName
          "keepNewlineAfter", Encode.bool pr.KeepNewlineAfter ]
    |> Encode.toString 4