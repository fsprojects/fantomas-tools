module FantomasTools.Client.ASTViewer.Decoders

open ASTViewer.Shared
open FantomasTools.Client.ASTViewer.Model
open Thoth.Json

let decodeUrlModel initialModel : Decoder<Model> =
    Decode.object (fun get ->
        { initialModel with
              Defines = get.Required.Field "defines" Decode.string
              IsFsi = get.Required.Field "isFsi" Decode.bool })

let private rangeDecoder: Decoder<Range> =
    Decode.object (fun get ->
        { StartLine = get.Required.Field "startLine" Decode.int
          StartCol = get.Required.Field "startCol" Decode.int
          EndLine = get.Required.Field "endLine" Decode.int
          EndCol = get.Required.Field "endCol" Decode.int })

let decodeKeyValue: Decoder<obj> = fun _ -> Ok

#nowarn "40"

let private decodeASTError: Decoder<ASTError> =
    Decode.object (fun get ->
        { SubCategory = get.Required.Field "subcategory" Decode.string
          Range = get.Required.Field "range" rangeDecoder
          Severity = get.Required.Field "severity" Decode.string
          ErrorNumber = get.Required.Field "errorNumber" Decode.int
          Message = get.Required.Field "message" Decode.string })

let responseDecoder: Decoder<Response> =
    Decode.object (fun get ->
        { String = get.Required.Field "string" Decode.string
          Errors = get.Required.Field "errors" (Decode.array decodeASTError) })

let decodeResult json = Decode.fromString responseDecoder json
