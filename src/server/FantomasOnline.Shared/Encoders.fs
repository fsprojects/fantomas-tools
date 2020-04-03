module FantomasOnline.Server.Shared.Encoders

open FantomasOnline.Shared
open Thoth.Json.Net

let encodeOptions options =
    options
    |> List.toArray
    |> Array.map (fun option ->
        match option with
        | IntOption (k, i) ->
            Encode.object
                [ "$type", Encode.string "int"
                  "$value", Encode.tuple2 Encode.string Encode.int (k, i) ]
        | BoolOption (k, b) ->
            Encode.object
                [ "$type", Encode.string "bool"
                  "$value", Encode.tuple2 Encode.string Encode.bool (k, b) ])
    |> Encode.array
    |> Encode.toString 4