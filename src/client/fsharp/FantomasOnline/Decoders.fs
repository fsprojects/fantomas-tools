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

let decodeOptionsFromEditorConfigFile (currentSettings: Map<string, FantomasOption>) (fileText: string) =
    fileText.Split([| Environment.NewLine |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (fun line -> line.StartsWith("fsharp_", StringComparison.OrdinalIgnoreCase))
    |> Array.map (fun line ->
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

            match existing with
            | Some(FantomasOption.IntOption(order, pascalName, _)) when isInt ->
                pascalName, FantomasOption.IntOption(order, pascalName, intValue)
            | Some(FantomasOption.BoolOption(order, pascalName, _)) when isBool ->
                pascalName, FantomasOption.BoolOption(order, pascalName, boolValue)
            | Some(FantomasOption.MultilineFormatterTypeOption(order, pascalName, _)) ->
                pascalName, FantomasOption.MultilineFormatterTypeOption(order, pascalName, value)
            | Some(FantomasOption.EndOfLineStyleOption(order, pascalName, _)) ->
                pascalName, FantomasOption.EndOfLineStyleOption(order, pascalName, value)
            | Some(FantomasOption.MultilineBracketStyleOption(order, pascalName, _)) ->
               pascalName, FantomasOption.MultilineBracketStyleOption(order, pascalName, value)
            | _ -> failwithf $"Cannot decode `{line}`"
        //
        // match name with
        // | _ when isInt -> FantomasOption.IntOption(i, pascalName, intValue)
        // | _ when isBool -> FantomasOption.BoolOption(i, pascalName, boolValue)
        // | "record_multiline_formatter"
        // | "array_or_list_multiline_formatter" -> FantomasOption.MultilineFormatterTypeOption(i, pascalName, value)
        // | "end_of_line" -> FantomasOption.EndOfLineStyleOption(i, pascalName, value)
        // | "multiline_bracket_style" -> FantomasOption.MultilineBracketStyleOption(i, pascalName, value)
        // | name -> failwithf $"Cannot decode %s{name}"
        | _ -> failwithf $"Cannot decode `{line}`")
    |> Array.toList

let private decodeRange: Decoder<Range> =
    Decode.object (fun get ->
        { StartLine = get.Required.Field "startLine" Decode.int
          StartCol = get.Required.Field "startCol" Decode.int
          EndLine = get.Required.Field "endLine" Decode.int
          EndCol = get.Required.Field "endCol" Decode.int })

let private decoderASTErrorSeverity: Decoder<ASTErrorSeverity> =
    Decode.string
    |> Decode.map (fun s ->
        match s with
        | "error" -> ASTErrorSeverity.Error
        | "warning" -> ASTErrorSeverity.Warning
        | "info" -> ASTErrorSeverity.Info
        | _ -> ASTErrorSeverity.Hidden)

let private decodeASTError: Decoder<ASTError> =
    Decode.object (fun get ->
        { SubCategory = get.Required.Field "subCategory" Decode.string
          Range = get.Required.Field "range" decodeRange
          Severity = get.Required.Field "severity" decoderASTErrorSeverity
          ErrorNumber = get.Required.Field "errorNumber" Decode.int
          Message = get.Required.Field "message" Decode.string })

let decodeFormatResponse: Decoder<FormatResponse> =
    Decode.object (fun get ->
        { FirstFormat = get.Required.Field "firstFormat" Decode.string
          FirstValidation = get.Required.Field "firstValidation" (Decode.array Diagnostic.Decode)
          SecondFormat = get.Optional.Field "secondFormat" Decode.string
          SecondValidation = get.Required.Field "secondValidation" (Decode.array Diagnostic.Decode) })
