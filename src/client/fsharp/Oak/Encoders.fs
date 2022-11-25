module FantomasTools.Client.OakViewer.Encoders

open Thoth.Json
open OakViewer
open FantomasTools.Client.OakViewer.Model

let encodeParseRequest (pr: ParseRequest) =
    Encode.object
        [ "sourceCode", Encode.string pr.SourceCode
          "defines", Array.map Encode.string pr.Defines |> Encode.array
          "isFsi", Encode.bool pr.IsFsi
          "isStroustrup", Encode.bool pr.IsStroustrup ]
    |> Encode.toString 4

let encodeUrlModel code isFsi (model: Model) =
    Encode.object
        [ "code", Encode.string code // the "code" key is a convention
          "defines", Encode.string model.Defines
          "isFsi", Encode.bool isFsi
          "isStroustrup", Encode.bool model.IsStroustrup ]
