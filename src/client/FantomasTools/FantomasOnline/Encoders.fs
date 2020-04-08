module FantomasTools.Client.FantomasOnline.Encoders

open FantomasOnline.Shared
open FantomasTools.Client.FantomasOnline.Model
open Thoth.Json

let private encodeOption fantomasOption =
    let key, value =
        match fantomasOption with
        | IntOption (k, v) -> "int", Encode.tuple2 Encode.string Encode.int (k, v)
        | BoolOption (k, v) -> "bool", Encode.tuple2 Encode.string Encode.bool (k, v)
    Encode.object [
        "$type", Encode.string key
        "$value", value
    ]


let encodeRequest code (model:Model) =
    Encode.object
        [ "sourceCode", Encode.string code
          "options", List.map encodeOption model.DefaultOptions |> Encode.list
          "isFsi", Encode.bool model.IsFsi ]
    |> Encode.toString 2