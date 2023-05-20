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
        // Optional to make old urls still work.
        let expand = get.Optional.Field "expand" Decode.bool

        [ yield Cmd.ofMsg (Bubble(BubbleMessage.SetSourceCode source))
          yield Cmd.ofMsg (Bubble(BubbleMessage.SetDefines defines))
          match expand with
          | Some expand -> yield Cmd.ofMsg (SetExpand expand)
          | None -> () ])

let decodeKeyValue: Decoder<obj> = fun _ -> Ok

#nowarn "40"

let responseDecoder: Decoder<Response> =
    Decode.object (fun get ->
        { Ast = get.Required.Field "ast" Decode.string
          Diagnostics = get.Required.Field "diagnostics" (Decode.array Diagnostic.Decode) })

let decodeResult json = Decode.fromString responseDecoder json
