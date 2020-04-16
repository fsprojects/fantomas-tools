module FantomasOnline.Server.Shared.Decoders

open FantomasOnline.Shared
open Thoth.Json.Net

let optionDecoder: Decoder<FantomasOption> =
    Decode.object (fun get ->
        let t = get.Required.Field "$type" Decode.string
        if t = "int" then
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.int)
            |> FantomasOption.IntOption
        else
            get.Required.Field "$value" (Decode.tuple3 Decode.int Decode.string Decode.bool)
            |> FantomasOption.BoolOption)

let requestDecoder: Decoder<FormatRequest> =
    Decode.object (fun get ->
        { SourceCode = get.Required.Field "sourceCode" Decode.string
          Options =
              get.Required.Field "options"
                  (Decode.list optionDecoder
                   |> Decode.map (List.sortBy sortByOption))
          IsFsi = get.Required.Field "isFsi" Decode.bool })

let decodeRequest json = Decode.fromString requestDecoder json
