module FantomasTools.Client.Trivia.ByTrivia

open Fable.React
open Fable.React.Props
open FantomasTools.Client.Trivia.Model
open FantomasTools.Client.Trivia.Menu
open TriviaViewer.Shared

let private contentToClassName c =
    match c with
    | Keyword _ -> "keyword"
    | Number _ -> "number-content"
    | StringContent _ -> "string-content"
    | CharContent _ -> "char-content"
    | IdentOperatorAsWord _ -> "ident-operator-keyword"
    | IdentBetweenTicks _ -> "ident-between-ticks"
    | Comment _ -> "comment"
    | Newline -> "newline"
    | Directive _ -> "directive"
    | NewlineAfter -> "newline-after"
    | EmbeddedIL _ -> "embedded-il"
    | KeywordString _ -> "keyword-string"
    |> sprintf "nav-link-%s"

let private typeName c =
    match c with
    | Keyword _ -> "Keyword"
    | Number _ -> "Number"
    | StringContent _ -> "StringContent"
    | CharContent _ -> "CharContent"
    | IdentOperatorAsWord _ -> "IdentOperatorAsWord"
    | IdentBetweenTicks _ -> "IdentBetweenTicks"
    | Comment c ->
        match c with
        | LineCommentAfterSourceCode _ -> "LineCommentAfterSourceCode"
        | LineCommentOnSingleLine _ -> "LineCommentOnSingleLine"
        | BlockComment _ -> "BlockComment"
        |> sprintf "Comment(%s)"
    | Newline -> "Newline"
    | Directive _ -> "Directive"
    | NewlineAfter -> "Newline-after"
    | EmbeddedIL _ -> "EmbeddedIL"
    | KeywordString _ -> "KeywordString"

let private activeTrivia trivia =
    let title = sprintf "%s %s" (typeName trivia.Item) (rangeToText trivia.Range)

    let content =
        match trivia.Item with
        | Number i
        | StringContent i
        | CharContent i
        | IdentOperatorAsWord i
        | IdentBetweenTicks i
        | Directive i
        | EmbeddedIL i
        | KeywordString i -> Some i
        | Comment c ->
            match c with
            | LineCommentAfterSourceCode c
            | LineCommentOnSingleLine c -> Some c
            | BlockComment (c, nb, na) ->
                sprintf "%s (newline before: %b) (newline after: %b)" c nb na
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

            { Label = label
              ClassName = className
              Title = label
              Range = t.Range })

    let onClick idx =
        dispatch (Msg.ActiveItemChange(ActiveTab.ByTrivia, idx))

    let activeTrivia =
        List.tryItem model.ActiveByTriviaIndex model.Trivia
        |> Option.map activeTrivia

    div [ ClassName "d-flex h-100" ] [
        menu onClick model.ActiveByTriviaIndex navItems
        div [ ClassName "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto" ] [
            ofOption activeTrivia
        ]
    ]
