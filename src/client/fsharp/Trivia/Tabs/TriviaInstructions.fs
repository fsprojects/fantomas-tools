module FantomasTools.Client.Trivia.Tabs.TriviaInstructions

open FantomasTools.Client.Trivia.Model
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Menu

let private isNotAnEmptyList = List.isEmpty >> not

let private triviaContentToDetail tc =
    let wrap outer inner = [ str $"%s{outer}("; code [] [ str inner ]; str ")" ]

    match tc with
    | Newline -> str "Newline"
    | Comment c ->
        match c with
        | BlockComment(bc, _, _) -> (wrap "BlockComment" bc)
        | LineCommentOnSingleLine lc -> (wrap "LineCommentOnSingleLine" lc)
        | LineCommentAfterSourceCode lc -> (wrap "LineCommentAfterSourceCode" lc)
        |> fun inner -> fragment [] [ str "Comment("; yield! inner; str ")" ]
    | Directive d -> fragment [] (wrap "Directive" d)

let private activeTriviaNode (instructions: TriviaInstruction list) =
    let ti = List.head instructions
    let title = $"%s{ti.Type} %s{rangeToText ti.Range}"

    let before, after =
        List.partition (fun (ti: TriviaInstruction) -> ti.AddBefore) instructions

    let contentInfo title items =
        if (isNotAnEmptyList items) then
            let listItems =
                items
                |> List.mapi (fun idx (instruction: TriviaInstruction) ->
                    li [ Key !!idx ] [ triviaContentToDetail instruction.Trivia.Item ])

            fragment [] [ h4 [] [ str title ]; ul [ ClassName "list-unstyled" ] [ ofList listItems ] ]
        else
            ofOption None

    div [ ClassName "tab-pane active" ] [
        h2 [ ClassName "mb-4" ] [ str title ]
        contentInfo "Content before" before
        contentInfo "Content after" after
    ]

let view (model: Model) dispatch =
    let groupedInstructions =
        model.TriviaInstructions |> List.groupBy (fun ti -> ti.Type, ti.Range)

    let navItems =
        groupedInstructions
        |> List.map (fun (_, g) ->
            let ti = List.head g

            {
                Label = ti.Type
                ClassName = ""
                Title = "MainNode"
                Range = ti.Range
            })

    let onClick idx =
        dispatch (Msg.ActiveItemChange(ActiveTab.TriviaInstructions, idx))

    let activeNode =
        List.tryItem model.ActiveByTriviaInstructionIndex groupedInstructions
        |> Option.map (snd >> activeTriviaNode)

    div [ ClassName "d-flex h-100" ] [
        menu onClick model.ActiveByTriviaInstructionIndex navItems
        div [ ClassName "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto" ] [ ofOption activeNode ]
    ]
