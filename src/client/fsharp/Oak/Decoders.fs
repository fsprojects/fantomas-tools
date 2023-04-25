module FantomasTools.Client.OakViewer.Decoders

open Elmish
open Thoth.Json
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model

let decodeUrlModel: Decoder<bool * Cmd<Msg>> =
    Decode.object (fun get ->
        let defines = get.Optional.Field "defines" Decode.string |> Option.defaultValue ""

        let isGraphView =
            get.Optional.Field "isGraphView" Decode.bool |> Option.defaultValue false

        isGraphView, Cmd.ofMsg (BubbleMessage.SetDefines defines |> Msg.Bubble))

let decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" Decode.string
          Range = get.Required.Field "range" Range.Decode
          Content = get.Optional.Field "content" Decode.string })

let rec decodeOak (name: string) (value: JsonValue) : Result<OakNode, DecoderError> =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Text = get.Optional.Field "text" Decode.string
              Range = get.Required.Field "range" Range.Decode
              ContentBefore = get.Required.Field "contentBefore" (Decode.array decodeTriviaNode)
              Children = get.Required.Field "children" (Decode.array decodeOak)
              ContentAfter = get.Required.Field "contentAfter" (Decode.array decodeTriviaNode) })
        name
        value
