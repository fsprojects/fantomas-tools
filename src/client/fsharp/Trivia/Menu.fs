module FantomasTools.Client.Trivia.Menu

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap
open TriviaViewer.Shared

let rangeToText (r: Range) =
    sprintf "(%i,%i - %i,%i)" r.StartLine r.StartColumn r.EndLine r.EndColumn

let private rangeToBadge (r: Range) =
    Badge.badge [ Badge.Color Dark
                  Badge.Custom [ ClassName "px-2 py-1 ml-auto" ] ] [
        (rangeToText r |> str)
    ]

let private triviaContentToDetail tc =
    let wrap outer inner =
        [ str (sprintf "%s(" outer)
          code [] [ str inner ]
          str ")" ]

    match tc with
    | Newline -> str "Newline"
    | StringContent sc -> fragment [] (wrap "StringContent" sc)
    | CharContent cc -> fragment [] (wrap "CharContent" cc)
    | Comment (c) ->
        match c with
        | BlockComment (bc, _, _) -> (wrap "BlockComment" bc)
        | LineCommentOnSingleLine (lc) -> (wrap "LineCommentOnSingleLine" lc)
        | LineCommentAfterSourceCode (lc) -> (wrap "LineCommentAfterSourceCode" lc)
        |> fun inner ->
            fragment [] [
                str "Comment("
                yield! inner
                str ")"
            ]
    | Directive d -> fragment [] (wrap "Directive" d)
    | IdentOperatorAsWord ioaw -> fragment [] (wrap "IdentOperatorAsWord" ioaw)
    | IdentBetweenTicks ibt -> fragment [] (wrap "IdentBetweenTicks" ibt)
    | Number n -> fragment [] (wrap "Number" n)
    | NewlineAfter -> str "NewlineAfter"
    | Keyword kw -> fragment [] (wrap "Keyword" kw)
    | EmbeddedIL eil -> fragment [] (wrap "EmbeddedIL" eil)
    | KeywordString ks -> fragment [] (wrap "KeywordString" ks)

type MenuItem =
    { ClassName: string
      Label: string
      Title: string
      Range: Range }

let menu onItemClick activeIndex items =
    let navItems =
        items
        |> List.mapi (fun idx mi ->
            let className =
                mi.ClassName
                |> sprintf "d-flex %s %s" (if idx = activeIndex then "active" else "")

            NavItem.navItem [ NavItem.Custom [ Key !!idx
                                               Title mi.Title
                                               OnClick (fun ev ->
                                                   ev.preventDefault ()
                                                   onItemClick idx) ] ] [
                NavLink.navLink [ NavLink.Custom [ Href "#"
                                                   ClassName className ] ] [
                    span [ ClassName "mr-4" ] [
                        str mi.Label
                    ]
                    rangeToBadge mi.Range
                ]
            ])

    Nav.nav [ Nav.Pills true
              Nav.Custom [ ClassName "flex-column" ] ] [
        ofList navItems
    ]
