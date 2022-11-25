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
