module FantomasTools.Client.OakViewer.Decoders

open Thoth.Json
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model

let decodeUrlModel: Decoder<bool> =
    Decode.object (fun get -> get.Optional.Field "isGraphView" Decode.bool |> Option.defaultValue false)

let decodeTriviaNode: Decoder<TriviaNode> =
    Decode.object (fun get ->
        { Type = get.Required.Field "type" Decode.string
          Range = get.Required.Field "range" Range.Decode
          Content = get.Optional.Field "content" Decode.string })

let rec private decodeNode (name: string) (value: JsonValue) =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Text = get.Optional.Field "text" Decode.string
              Range = get.Required.Field "range" Range.Decode
              ContentBefore = get.Required.Field "contentBefore" (Decode.array decodeTriviaNode)
              Children = get.Required.Field "children" (Decode.array decodeNode)
              ContentAfter = get.Required.Field "contentAfter" (Decode.array decodeTriviaNode) })
        name
        value

let decodeOak: string -> obj -> Result<OakNode * Diagnostic array, DecoderError> =
    Decode.object (fun get ->
        let oak = get.Required.Field "oak" decodeNode
        let diagnostics = get.Required.Field "diagnostics" (Decode.array Diagnostic.Decode)
        oak, diagnostics)
