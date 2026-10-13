module FantomasTools.Client.FantomasOnline.Decoders

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
            failwithf $"Cannot decode %s{t}"
    )

let decodeOptions json =
    Decode.fromString (Decode.array optionDecoder) json
    |> Result.map (Array.sortBy sortByOption >> List.ofArray)

let decodeOptionsFromUrl: Decoder<FantomasOption list> =
    Decode.object (fun get -> get.Required.Field "settings" (Decode.list optionDecoder))

let private sourceCommentDecoder: Decoder<SourceComment> =
    Decode.object (fun get ->
        {
            Range = get.Required.Field "range" Range.Decode
            Text = get.Required.Field "text" Decode.string
        }
    )

let private formatIssueDecoder: Decoder<FormatIssue> =
    Decode.object (fun get ->
        let defines () =
            get.Required.Field "defines" (Decode.list Decode.string)

        match get.Required.Field "kind" Decode.string with
        | "missingComment" -> FormatIssue.MissingComment(get.Required.Field "comment" sourceCommentDecoder)
        | "notValidFSharp" ->
            FormatIssue.NotValidFSharp(defines (), get.Required.Field "diagnostics" (Decode.array Diagnostic.Decode))
        | "commentsChanged" ->
            FormatIssue.CommentsChanged(
                defines (),
                get.Required.Field "missing" (Decode.array sourceCommentDecoder),
                get.Required.Field "added" (Decode.array Decode.string)
            )
        | "directivesChanged" ->
            FormatIssue.DirectivesChanged(
                defines (),
                get.Required.Field "missing" (Decode.array Decode.string),
                get.Required.Field "added" (Decode.array Decode.string)
            )
        | "notIdempotent" -> FormatIssue.NotIdempotent(get.Required.Field "formattedAgain" Decode.string)
        | "checkFailed" ->
            FormatIssue.CheckFailed(
                get.Required.Field "check" Decode.string,
                get.Required.Field "message" Decode.string
            )
        | kind -> failwithf $"Cannot decode issue %s{kind}"
    )

let decodeFormatResponse: Decoder<FormatResponse> =
    Decode.object (fun get ->
        {
            Code = get.Required.Field "code" Decode.string
            Issues = get.Required.Field "issues" (Decode.array formatIssueDecoder)
        }
    )
