module FantomasOnline.Server.Shared.Encoders

open FantomasOnline.Shared
open Thoth.Json.Net

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
                  "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v) ])
    |> Encode.array
    |> Encode.toString 4

let private encodeRange (range: Range) =
    Encode.object
        [ "startLine", Encode.int range.StartLine
          "startCol", Encode.int range.StartCol
          "endLine", Encode.int range.EndLine
          "endCol", Encode.int range.EndCol ]

let private encodeASTErrorSeverity =
    function
    | ASTErrorSeverity.Error -> Encode.string "error"
    | ASTErrorSeverity.Warning -> Encode.string "warning"
    | ASTErrorSeverity.Info -> Encode.string "info"
    | ASTErrorSeverity.Hidden -> Encode.string "hidden"

let private encodeASTError (astError: ASTError) =
    Encode.object
        [ "subCategory", Encode.string astError.SubCategory
          "range", encodeRange astError.Range
          "severity", encodeASTErrorSeverity astError.Severity
          "errorNumber", Encode.int astError.ErrorNumber
          "message", Encode.string astError.Message ]

let encodeFormatResponse (formatResponse: FormatResponse) =
    Encode.object
        [ "firstFormat", Encode.string formatResponse.FirstFormat
          "firstValidation", (formatResponse.FirstValidation |> List.map encodeASTError |> Encode.list)
          "secondFormat", Encode.option Encode.string formatResponse.SecondFormat
          "secondValidation", (formatResponse.SecondValidation |> List.map encodeASTError |> Encode.list) ]
