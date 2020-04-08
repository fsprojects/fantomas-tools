module FantomasTools.Client.FantomasOnline.Decoders

open Thoth.Json
open FantomasOnline.Shared

let private optionDecoder : Decoder<FantomasOption> =
    Decode.object (fun get ->
        let t = get.Required.Field "$type" Decode.string
        if t = "int" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.int)
            |> FantomasOption.IntOption
        else
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.bool)
            |> FantomasOption.BoolOption)

let decodeOptions json =
    Decode.fromString (Decode.array optionDecoder) json
    |> Result.map (Array.sortBy sortByOption >> List.ofArray)