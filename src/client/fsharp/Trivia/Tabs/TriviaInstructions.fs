module FantomasTools.Client.Trivia.Tabs.TriviaInstructions

open FantomasTools.Client.Trivia.Model
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client.Trivia.Tabs
open Reactstrap
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Menu

// let private rangeToText (r: Range) =
//     sprintf "(%i,%i - %i,%i)" r.StartLine r.StartColumn r.EndLine r.EndColumn
//
// let private rangeToBadge (r: Range) =
//     Badge.badge [
//         Badge.Color Dark
//         Badge.Custom [ ClassName "px-2 py-1 ml-auto" ]
//     ] [ (rangeToText r |> str) ]
//
// let private isNotAnEmptyList = List.isEmpty >> not
//
let private triviaContentToDetail tc =
    let wrap outer inner = [
        str (sprintf "%s(" outer)
        code [] [ str inner ]
        str ")"
    ]

    match tc with
    | Newline -> str "Newline"
    | Comment c ->
        match c with
        | BlockComment (bc, _, _) -> (wrap "BlockComment" bc)
        | LineCommentOnSingleLine lc -> (wrap "LineCommentOnSingleLine" lc)
        | LineCommentAfterSourceCode lc -> (wrap "LineCommentAfterSourceCode" lc)
        |> fun inner ->
            fragment [] [
                str "Comment("
                yield! inner
                str ")"
            ]
    | Directive d -> fragment [] (wrap "Directive" d)

let private activeTriviaNode (ti: TriviaInstruction) =
    let title = $"%s{ti.Type} %s{rangeToText ti.Range}"

    div [ ClassName "tab-pane active" ] [
        h2 [ ClassName "mb-4" ] [ str title ]
        h4 [] [ str title ]
        triviaContentToDetail ti.Trivia.Item
    ]

let view (model: Model) dispatch =
    let navItems =
        model.TriviaInstructions
        |> List.map (fun tn -> {
            Label = tn.Type
            ClassName = ""
            Title = "MainNode"
            Range = tn.Range
        })

    let onClick idx =
        dispatch (Msg.ActiveItemChange(ActiveTab.TriviaInstructions, idx))

    let activeNode =
        List.tryItem model.ActiveByTriviaInstructionIndex model.TriviaInstructions
        |> Option.map activeTriviaNode

    div [ ClassName "d-flex h-100" ] [
        menu onClick model.ActiveByTriviaInstructionIndex navItems
        div [
            ClassName "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto"
        ] [ ofOption activeNode ]
    ]
