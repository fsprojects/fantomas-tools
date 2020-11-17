module FantomasTools.Client.FantomasOnline.Decoders

open Thoth.Json
open FantomasOnline.Shared

let private optionDecoder: Decoder<FantomasOption> =
    Decode.object
        (fun get ->
            let t = get.Required.Field "$type" Decode.string

            if t = "int" then
                get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.int)
                |> FantomasOption.IntOption
            elif t = "bool" then
                get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.bool)
                |> FantomasOption.BoolOption
            else
                get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.string)
                |> FantomasOption.MultilineFormatterTypeOption)

let decodeOptions json =
    Decode.fromString (Decode.array optionDecoder) json
    |> Result.map (Array.sortBy sortByOption >> List.ofArray)

let decodeOptionsFromUrl: Decoder<FantomasOption list * bool> =
    Decode.object
        (fun get ->
            let settings =
                get.Required.Field "settings" (Decode.list optionDecoder)

            let isFSI = get.Required.Field "isFsi" Decode.bool
            settings, isFSI)
