module FantomasTools.Client.FantomasOnline.Decoders

open FantomasTools.Client
open Thoth.Json
open FantomasOnline.Shared

let private optionDecoder: Decoder<FantomasOption> =
    Decode.object (fun get ->
        let t = get.Required.Field "$type" Decode.string

        if t = "int" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.int)
            |> FantomasOption.IntOption
        elif t = "bool" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.bool)
            |> FantomasOption.BoolOption
        elif t = "multilineFormatterType" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.string)
            |> FantomasOption.MultilineFormatterTypeOption
        elif t = "endOfLineStyle" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.string)
            |> FantomasOption.EndOfLineStyleOption
        elif t = "multilineBracketStyle" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.string)
            |> FantomasOption.MultilineBracketStyleOption
        else
            failwithf $"Cannot decode %s{t}")

let decodeOptions json =
    Decode.fromString (Decode.array optionDecoder) json
    |> Result.map (Array.sortBy sortByOption >> List.ofArray)

let decodeOptionsFromUrl: Decoder<FantomasOption list> =
    Decode.object (fun get -> get.Required.Field "settings" (Decode.list optionDecoder))

let decodeFormatResponse: Decoder<FormatResponse> =
    Decode.object (fun get ->
        { FirstFormat = get.Required.Field "firstFormat" Decode.string
          FirstValidation = get.Required.Field "firstValidation" (Decode.list Diagnostic.Decode)
          SecondFormat = get.Optional.Field "secondFormat" Decode.string
          SecondValidation = get.Required.Field "secondValidation" (Decode.list Diagnostic.Decode) })
