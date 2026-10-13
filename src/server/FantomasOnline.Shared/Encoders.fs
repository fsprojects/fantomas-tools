module FantomasOnline.Server.Shared.Encoders

open Thoth.Json.Core
open Thoth.Json.System.Text.Json
open FantomasOnline.Shared
open FantomasTools.Client

let encodeOptions options =
    options
    |> List.toArray
    |> Array.map (fun option ->
        match option with
        | IntOption(o, k, i) ->
            Encode.object
                [
                    "$type", Encode.string "int"
                    "$value", Encode.tuple3 Encode.int Encode.string Encode.int (o, k, i)
                ]
        | BoolOption(o, k, b) ->
            Encode.object
                [
                    "$type", Encode.string "bool"
                    "$value", Encode.tuple3 Encode.int Encode.string Encode.bool (o, k, b)
                ]
        | MultilineFormatterTypeOption(o, k, v) ->
            Encode.object
                [
                    "$type", Encode.string "multilineFormatterType"
                    "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v)
                ]
        | EndOfLineStyleOption(o, k, v) ->
            Encode.object
                [
                    "$type", Encode.string "endOfLineStyle"
                    "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v)
                ]
        | MultilineBracketStyleOption(o, k, v) ->
            Encode.object
                [
                    "$type", Encode.string "multilineBracketStyle"
                    "$value", Encode.tuple3 Encode.int Encode.string Encode.string (o, k, v)
                ]
    )
    |> Encode.array
    |> Encode.toString 4

let private encodeSourceComment (comment: SourceComment) : IEncodable =
    Encode.object [ "range", Range.Encode comment.Range; "text", Encode.string comment.Text ]

let private encodeStrings (values: string seq) : IEncodable =
    values |> Seq.map Encode.string |> Seq.toArray |> Encode.array

let private encodeFormatIssue (issue: FormatIssue) : IEncodable =
    let issueObject (kind: string) (fields: (string * IEncodable) list) =
        Encode.object [ yield "kind", Encode.string kind; yield! fields ]

    match issue with
    | FormatIssue.MissingComment comment -> issueObject "missingComment" [ "comment", encodeSourceComment comment ]
    | FormatIssue.NotValidFSharp(defines, diagnostics) ->
        issueObject
            "notValidFSharp"
            [
                "defines", encodeStrings defines
                "diagnostics", (diagnostics |> Array.map Diagnostic.Encode |> Encode.array)
            ]
    | FormatIssue.CommentsChanged(defines, missing, added) ->
        issueObject
            "commentsChanged"
            [
                "defines", encodeStrings defines
                "missing", (missing |> Array.map encodeSourceComment |> Encode.array)
                "added", encodeStrings added
            ]
    | FormatIssue.DirectivesChanged(defines, missing, added) ->
        issueObject
            "directivesChanged"
            [
                "defines", encodeStrings defines
                "missing", encodeStrings missing
                "added", encodeStrings added
            ]
    | FormatIssue.NotIdempotent formattedAgain ->
        issueObject "notIdempotent" [ "formattedAgain", Encode.string formattedAgain ]
    | FormatIssue.CheckFailed(check, message) ->
        issueObject "checkFailed" [ "check", Encode.string check; "message", Encode.string message ]

let encodeFormatResponse (formatResponse: FormatResponse) =
    Encode.object
        [
            "code", Encode.string formatResponse.Code
            "issues", (formatResponse.Issues |> Array.map encodeFormatIssue |> Encode.array)
        ]
