module FantomasTools.Client.OakViewer.Decoders

open FantomasTools.Client.Editor
open Thoth.Json
open FantomasTools.Client.OakViewer.Model

let decodeUrlModel (initialModel: Model) : Decoder<Model> =
    Decode.object (fun get ->
        let defines = get.Optional.Field "defines" Decode.string |> Option.defaultValue ""

        let isGraphView =
            get.Optional.Field "isGraphView" Decode.bool |> Option.defaultValue false

        { initialModel with
            Defines = defines
            IsGraphView = isGraphView })

let decodeRange: Decoder<HighLightRange> =
    Decode.object (fun get ->
        { StartLine = get.Required.Field "startLine" Decode.int
          StartColumn = get.Required.Field "startColumn" Decode.int
          EndLine = get.Required.Field "endLine" Decode.int
          EndColumn = get.Required.Field "endColumn" Decode.int })

let decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" Decode.string
          Range = get.Required.Field "range" decodeRange
          Content = get.Optional.Field "content" Decode.string })

let rec decodeOak (name: string) (value: JsonValue) : Result<OakNode, DecoderError> =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Text = get.Optional.Field "text" Decode.string
              Range = get.Required.Field "range" decodeRange
              ContentBefore = get.Required.Field "contentBefore" (Decode.array decodeTriviaNode)
              Children = get.Required.Field "children" (Decode.array decodeOak)
              ContentAfter = get.Required.Field "contentAfter" (Decode.array decodeTriviaNode) })
        name
        value
