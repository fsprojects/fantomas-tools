module FantomasTools.Client.ASTViewer.Decoders

open ASTViewer.Shared
open Elmish
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open Thoth.Json

let decodeUrlModel: Decoder<Cmd<Msg> list> =
    Decode.object (fun get ->
        let defines = get.Required.Field "defines" Decode.string
        let source = get.Required.Field "code" Decode.string

        [ Cmd.ofMsg (Bubble(BubbleMessage.SetSourceCode source))
          Cmd.ofMsg (Bubble(BubbleMessage.SetDefines defines)) ])

let decodeKeyValue: Decoder<obj> = fun _ -> Ok

#nowarn "40"

let responseDecoder: Decoder<Response> =
    Decode.object (fun get ->
        { String = get.Required.Field "string" Decode.string
          Errors = get.Required.Field "errors" (Decode.array Diagnostic.Decode) })

let decodeResult json = Decode.fromString responseDecoder json
