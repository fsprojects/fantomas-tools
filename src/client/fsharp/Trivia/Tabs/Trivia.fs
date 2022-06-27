module FantomasTools.Client.Trivia.Tabs.Trivia

open Fable.React
open Fable.React.Props
open FantomasTools.Client.Trivia.Model
open FantomasTools.Client.Trivia.Menu
open TriviaViewer.Shared

let private contentToClassName c =
    match c with
    | Comment _ -> "comment"
    | Newline -> "newline"
    | Directive _ -> "directive"
    |> sprintf "nav-link-%s"

let private typeName c =
    match c with
    | Comment c ->
        match c with
        | LineCommentAfterSourceCode _ -> "LineCommentAfterSourceCode"
        | LineCommentOnSingleLine _ -> "LineCommentOnSingleLine"
        | BlockComment _ -> "BlockComment"
        |> sprintf "Comment(%s)"
    | Newline -> "Newline"
    | Directive _ -> "Directive"

let private activeTrivia trivia =
    let title = $"%s{typeName trivia.Item} %s{rangeToText trivia.Range}"

    let content =
        match trivia.Item with
        | Directive d -> Some d
        | Comment c ->
            match c with
            | LineCommentAfterSourceCode c
            | LineCommentOnSingleLine c -> Some c
            | BlockComment (c, nb, na) ->
                $"%s{c} (newline before: %b{nb}) (newline after: %b{na})"
                |> Some
        | _ -> None
        |> Option.map (fun c -> code [] [ str c ])

    div [ ClassName "tab-pane active" ] [
        h2 [ ClassName "mb-4" ] [ str title ]
        ofOption content
    ]

let view (model: Model) dispatch =
    let navItems =
        model.Trivia
        |> List.map (fun t ->
            let className = contentToClassName t.Item
            let label = typeName t.Item

            {
                Label = label
                ClassName = className
                Title = label
                Range = t.Range
            })

    let onClick idx =
        dispatch (Msg.ActiveItemChange(ActiveTab.Trivia, idx))

    let activeTrivia =
        List.tryItem model.ActiveByTriviaIndex model.Trivia
        |> Option.map activeTrivia

    div [ ClassName "d-flex h-100" ] [
        menu onClick model.ActiveByTriviaIndex navItems
        div [
            ClassName "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto"
        ] [ ofOption activeTrivia ]
    ]
