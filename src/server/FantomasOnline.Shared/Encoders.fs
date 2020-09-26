module FantomasOnline.Server.Shared.Encoders

open FantomasOnline.Shared
open Thoth.Json.Net

let encodeOptions options =
    options
    |> List.toArray
    |> Array.map (fun option ->
        match option with
        | IntOption (o, k, i) ->
            Encode.object [ "$type", Encode.string "int"
                            "$value", Encode.tuple3 Encode.int Encode.string Encode.int (o, k, i) ]
        | BoolOption (o, k, b) ->
            Encode.object [ "$type", Encode.string "bool"
                            "$value", Encode.tuple3 Encode.int Encode.string Encode.bool (o, k, b) ]
        | MultilineFormatterTypeOption (o, k, v) ->
            Encode.object [ "$type", Encode.string "multilineFormatterType"
                            "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v) ])
    |> Encode.array
    |> Encode.toString 4
