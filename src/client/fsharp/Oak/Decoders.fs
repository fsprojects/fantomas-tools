module FantomasTools.Client.OakViewer.Decoders

open Thoth.Json
open FantomasTools.Client.OakViewer.Model

let decodeUrlModel (initialModel: Model) : Decoder<Model> =
    Decode.object (fun get ->
        let defines = get.Optional.Field "defines" Decode.string |> Option.defaultValue ""

        let isStroustrup =
            get.Optional.Field "isStroustrup" Decode.bool |> Option.defaultValue false

        { initialModel with
            Defines = defines
            IsStroustrup = isStroustrup })

let decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" Decode.string
          Range = get.Required.Field "range" Decode.string
          Content = get.Optional.Field "content" Decode.string })

let rec decodeOak (name: string) (value: JsonValue) : Result<OakNode, DecoderError> =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Range = get.Required.Field "range" Decode.string
              ContentBefore = get.Required.Field "contentBefore" (Decode.array decodeTriviaNode)
              Children = get.Required.Field "children" (Decode.array decodeOak)
              ContentAfter = get.Required.Field "contentAfter" (Decode.array decodeTriviaNode) })
        name
        value
