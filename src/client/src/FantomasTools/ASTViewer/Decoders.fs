module FantomasTools.Client.ASTViewer.Decoders

open ASTViewer.Shared
open FantomasTools.Client.ASTViewer.Model
open Thoth.Json

let decodeUrlModel initialModel: Decoder<Model> =
    Decode.object
        (fun get ->
            { initialModel with
                  Defines = get.Required.Field "defines" Decode.string
                  IsFsi = get.Required.Field "isFsi" Decode.bool })

let private rangeDecoder: Decoder<Range> =
    Decode.object
        (fun get ->
            { StartLine = get.Required.Field "startLine" Decode.int
              StartCol = get.Required.Field "startCol" Decode.int
              EndLine = get.Required.Field "endLine" Decode.int
              EndCol = get.Required.Field "endCol" Decode.int })

let decodeKeyValue: Decoder<obj> = fun _ -> Ok

#nowarn "40"

let rec private nodeDecoder: Decoder<Node> =
    Decode.object
        (fun get ->
            { Type = get.Required.Field "type" Decode.string
              Range = get.Optional.Field "range" rangeDecoder
              Properties = get.Required.Field "properties" decodeKeyValue
              Childs = get.Required.Field "childs" (Decode.array nodeDecoder) })


let responseDecoder: Decoder<Dto> =
    Decode.object
        (fun get ->
            { Node = get.Required.Field "node" nodeDecoder
              String = get.Required.Field "string" Decode.string })

let decodeResult json = Decode.fromString responseDecoder json
