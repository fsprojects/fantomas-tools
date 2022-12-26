module FantomasTools.Client.FantomasOnline.Decoders

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

let decodeOptionsFromUrl: Decoder<FantomasOption list * bool> =
    Decode.object (fun get ->
        let settings = get.Required.Field "settings" (Decode.list optionDecoder)
        let isFSI = get.Required.Field "isFsi" Decode.bool
        settings, isFSI)

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
          FirstValidation = get.Required.Field "firstValidation" (Decode.list decodeASTError)
          SecondFormat = get.Optional.Field "secondFormat" Decode.string
          SecondValidation = get.Required.Field "secondValidation" (Decode.list decodeASTError) })
