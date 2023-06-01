module FantomasOnline.Server.Shared.Encoders

open Thoth.Json.Net
open FantomasOnline.Shared
open FantomasTools.Client

let encodeOptions options =
    options
    |> List.toArray
    |> Array.map (fun option ->
        match option with
        | IntOption(o, k, i) ->
            Encode.object
                [ "$type", Encode.string "int"
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.int (o, k, i) ]
        | BoolOption(o, k, b) ->
            Encode.object
                [ "$type", Encode.string "bool"
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.bool (o, k, b) ]
        | MultilineFormatterTypeOption(o, k, v) ->
            Encode.object
                [ "$type", Encode.string "multilineFormatterType"
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v) ]
        | EndOfLineStyleOption(o, k, v) ->
            Encode.object
                [ "$type", Encode.string "endOfLineStyle"
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v) ]
        | MultilineBracketStyleOption(o, k, v) ->
            Encode.object
                [ "$type", Encode.string "multilineBracketStyle"
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v) ])
    |> Encode.array
    |> Encode.toString 4

let encodeFormatResponse (formatResponse: FormatResponse) =
    Encode.object
        [ "firstFormat", Encode.string formatResponse.FirstFormat
          "firstValidation", (formatResponse.FirstValidation |> Array.map Diagnostic.Encode |> Encode.array)
          "secondFormat", Encode.option Encode.string formatResponse.SecondFormat
          "secondValidation", (formatResponse.SecondValidation |> Array.map Diagnostic.Encode |> Encode.array) ]
