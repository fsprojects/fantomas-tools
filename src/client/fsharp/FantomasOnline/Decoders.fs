module FantomasTools.Client.FantomasOnline.Decoders

open System
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

let rec decodeEditorConfigLine (currentSettings: Map<string, FantomasOption>) i (line: string) =
    let parts =
        line.Split([| '='; ' ' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.toList

    let toPascalCase (str: string) =
        let rec buildStr acc (split: string list) =
            match split with
            | [] -> acc
            | h :: t ->
                let first = h[0] |> Char.ToUpper |> string
                buildStr (acc + first + h[1..]) t

        str.Split([| '_' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.toList
        |> buildStr ""

    match parts with
    | name :: value :: _ ->
        let name = name.Trim().Replace("fsharp_", "")
        let value = value.Trim()
        let isInt, intValue = Int32.TryParse value
        let isBool, boolValue = Boolean.TryParse value

        let pascalName = toPascalCase name
        let existing = currentSettings.TryFind pascalName

        let optionValue =
            match existing with
            | Some(FantomasOption.IntOption(order, pascalName, _)) when isInt ->
                FantomasOption.IntOption(order, pascalName, intValue)
            | Some(FantomasOption.BoolOption(order, pascalName, _)) when isBool ->
                FantomasOption.BoolOption(order, pascalName, boolValue)
            | Some(FantomasOption.MultilineFormatterTypeOption(order, pascalName, _)) ->
                FantomasOption.MultilineFormatterTypeOption(order, pascalName, value)
            | Some(FantomasOption.EndOfLineStyleOption(order, pascalName, _)) ->
                FantomasOption.EndOfLineStyleOption(order, pascalName, value)
            | Some(FantomasOption.MultilineBracketStyleOption(order, pascalName, _)) ->
                FantomasOption.MultilineBracketStyleOption(order, pascalName, value)
            | _ ->
                let pascalName = toPascalCase name

                match name with
                | _ when isInt -> FantomasOption.IntOption(i, pascalName, intValue)
                | _ when isBool -> FantomasOption.BoolOption(i, pascalName, boolValue)
                | "record_multiline_formatter"
                | "array_or_list_multiline_formatter" ->
                    FantomasOption.MultilineFormatterTypeOption(i, pascalName, value)
                | "end_of_line" -> FantomasOption.EndOfLineStyleOption(i, pascalName, value)
                | "multiline_bracket_style" -> FantomasOption.MultilineBracketStyleOption(i, pascalName, value)
                | name -> failwithf $"Cannot decode %s{name}"

        pascalName, optionValue
    | _ -> failwithf $"Cannot decode `{line}`"

let decodeOptionsFromEditorConfigFile (currentSettings: Map<string, FantomasOption>) (fileText: string) =
    fileText.Split([| Environment.NewLine |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (fun line ->
        line.StartsWith("fsharp_", StringComparison.OrdinalIgnoreCase)
        || [ "max_line_length"; "indent_size"; "end_of_line"; "insert_final_newline" ]
           |> List.exists (fun prop -> line.StartsWith(prop, StringComparison.OrdinalIgnoreCase)))
    |> Array.mapi (decodeEditorConfigLine currentSettings)
    |> Array.toList

let decodeFormatResponse: Decoder<FormatResponse> =
    Decode.object (fun get ->
        { FirstFormat = get.Required.Field "firstFormat" Decode.string
          FirstValidation = get.Required.Field "firstValidation" (Decode.array Diagnostic.Decode)
          SecondFormat = get.Optional.Field "secondFormat" Decode.string
          SecondValidation = get.Required.Field "secondValidation" (Decode.array Diagnostic.Decode) })
