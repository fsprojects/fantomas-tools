module FantomasTools.Client.Trivia.ByTriviaNodes

open FantomasTools.Client.Trivia.Model
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Menu

let private rangeToText (r: Range) =
    sprintf "(%i,%i - %i,%i)" r.StartLine r.StartColumn r.EndLine r.EndColumn

let private rangeToBadge (r: Range) =
    Badge.badge [
        Badge.Color Dark
        Badge.Custom [ ClassName "px-2 py-1 ml-auto" ]
    ] [ (rangeToText r |> str) ]

let private isNotAnEmptyList = List.isEmpty >> not

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

let private activeTriviaNode (tn: TriviaNode) =
    let title = $"%s{tn.Type} %s{rangeToText tn.Range}"

    let contentInfo title items =
        if (isNotAnEmptyList items) then
            let listItems =
                items
                |> List.mapi (fun idx item -> li [ Key !!idx ] [ triviaContentToDetail item ])

            fragment [] [
                h4 [] [ str title ]
                ul [ ClassName "list-unstyled" ] [ ofList listItems ]
            ]
        else
            ofOption None

    div [ ClassName "tab-pane active" ] [
        h2 [ ClassName "mb-4" ] [ str title ]
        contentInfo "Content before" tn.ContentBefore
        contentInfo "Content itself" (Option.toList tn.ContentItself)
        contentInfo "Content after" tn.ContentAfter
    ]

let view (model: Model) dispatch =
    let navItems =
        model.TriviaNodes
        |> List.map (fun tn ->
            let className = "nav-link-main-node"

            {
                Label = tn.Type
                ClassName = className
                Title = "MainNode"
                Range = tn.Range
            })

    let onClick idx =
        dispatch (Msg.ActiveItemChange(ActiveTab.ByTriviaNodes, idx))

    let activeNode =
        List.tryItem model.ActiveByTriviaNodeIndex model.TriviaNodes
        |> Option.map activeTriviaNode

    div [ ClassName "d-flex h-100" ] [
        menu onClick model.ActiveByTriviaNodeIndex navItems
        div [
            ClassName "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto"
        ] [ ofOption activeNode ]
    ]
