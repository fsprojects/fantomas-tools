module OakViewer.GetOak

open FSharp.Compiler.Text
open Fantomas.Core
open Fantomas.Core.FormatConfig
open Fantomas.Core.SyntaxOak
open OakViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private parseAST source defines isFsi = Fantomas.FCS.Parse.parseFile isFsi source defines

[<RequireQualifiedAccess>]
type GetOakResponse =
    | Ok of text: string
    | BadRequest of body: string

let getOak json : GetOakResponse =
    let parseRequest = Decoders.decodeParseRequest json

    match parseRequest with
    | Ok pr ->
        let { SourceCode = content
              Defines = defines
              IsFsi = isFsi
              IsStroustrup = isStroustrup } =
            pr

        let source = SourceText.ofString content
        let ast, _diags = parseAST source (List.ofArray defines) isFsi

        let config =
            if not isStroustrup then
                FormatConfig.Default
            else
                { FormatConfig.Default with MultilineBracketStyle = MultilineBracketStyle.ExperimentalStroustrup }

        let oak =
            Fangorn.mkOak config (Some source) ast |> Flowering.enrichTree config source ast

        let responseText =
            let rangeToText (m: range) =
                $"(%i{m.StartLine},%i{m.StartColumn}-%i{m.EndLine},%i{m.EndColumn})"

            let triviaLines level (trivia: TriviaNode seq) =
                if Seq.isEmpty trivia then
                    Array.empty
                else
                    let levelPadding = "".PadRight(level * 2)

                    Seq.map
                        (fun (tn: TriviaNode) ->
                            let rangeText = rangeToText tn.Range

                            match tn.Content with
                            | Newline -> sprintf "%sNewline %s" levelPadding rangeText
                            | LineCommentAfterSourceCode comment
                            | CommentOnSingleLine comment -> sprintf "%s%s %s" levelPadding comment rangeText
                            | BlockComment(comment, _, _) -> sprintf "%s%s %s" levelPadding comment rangeText
                            | Directive directive -> sprintf "%s%s %s" levelPadding directive rangeText)
                        trivia
                    |> Seq.toArray

            let rec visit (level: int) (node: Node) (continuation: string array -> string array) : string array =
                let continuations = node.Children |> Array.map (visit (level + 1)) |> Array.toList

                let contentBefore, currentNode, contentAfter =
                    let name = node.GetType().Name

                    let nodeText =
                        match node with
                        | :? SingleTextNode as stn ->
                            let text =
                                if stn.Text.Length > 13 then
                                    sprintf "%s..." (stn.Text.Substring(0, 10))
                                else
                                    stn.Text

                            sprintf "%s\"%s\" %s" ("".PadRight(level * 2)) text (rangeToText node.Range)
                        | _ -> sprintf "%s%s %s" ("".PadRight(level * 2)) name (rangeToText node.Range)

                    triviaLines level node.ContentBefore, nodeText, triviaLines level node.ContentAfter

                let finalContinuation (lines: string array list) =
                    [| yield! contentBefore
                       yield currentNode
                       yield! Seq.collect id lines
                       yield! contentAfter |]
                    |> continuation

                Continuation.sequence continuations finalContinuation

            visit 0 oak id |> String.concat "\n"

        GetOakResponse.Ok responseText

    | Error err -> GetOakResponse.BadRequest(string err)
