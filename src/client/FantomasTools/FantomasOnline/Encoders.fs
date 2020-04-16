module FantomasTools.Client.FantomasOnline.Encoders

open FantomasOnline.Shared
open FantomasTools.Client.FantomasOnline.Model
open Thoth.Json

let private encodeOption fantomasOption =
    let key, value =
        match fantomasOption with
        | IntOption (o, k, v) -> "int", Encode.tuple3 Encode.int Encode.string Encode.int (o, k, v)
        | BoolOption (o, k, v) -> "bool", Encode.tuple3 Encode.int Encode.string Encode.bool (o, k, v)

    Encode.object
        [ "$type", Encode.string key
          "$value", value ]

let private encodeUserSettings model =
    model.UserOptions
    |> Map.toList
    |> List.sortBy (snd >> sortByOption)
    |> List.map (snd >> encodeOption)
    |> Encode.list

let encodeRequest code (model: Model) =
    Encode.object
        [ "sourceCode", Encode.string code
          "options", encodeUserSettings model
          "isFsi", Encode.bool model.IsFsi ]
    |> Encode.toString 2

let encodeUrlModel code model =
    Encode.object
        [ "code", Encode.string code
          "settings", encodeUserSettings model
          "isFsi", Encode.bool model.IsFsi ]
