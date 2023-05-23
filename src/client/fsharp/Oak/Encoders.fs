module FantomasTools.Client.OakViewer.Encoders

open FantomasTools.Client
open Thoth.Json
open FantomasTools.Client.OakViewer.Model

let encodeParseRequest (pr: OakViewer.ParseRequest) =
    Encode.object
        [ "sourceCode", Encode.string pr.SourceCode
          "defines", Array.map Encode.string pr.Defines |> Encode.array
          "isFsi", Encode.bool pr.IsFsi ]
    |> Encode.toString 4

let encodeUrlModel (bubble: BubbleModel) (model: Model) =
    Encode.object
        [ "code", Encode.string bubble.SourceCode // the "code" key is a convention
          "defines", Encode.string bubble.Defines
          "isFsi", Encode.bool bubble.IsFsi
          "isGraphView", Encode.bool model.IsGraphView ]
