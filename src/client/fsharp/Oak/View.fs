﻿module FantomasTools.Client.OakViewer.View

open Browser.Types
open Fable.Core
open FantomasTools.Client.Editor
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView

[<RequireQualifiedAccess>]
module Continuation =
    let rec sequence<'a, 'ret> (recursions: (('a -> 'ret) -> 'ret) list) (finalContinuation: 'a list -> 'ret) : 'ret =
        match recursions with
        | [] -> [] |> finalContinuation
        | recurse :: recurses -> recurse (fun ret -> sequence recurses (fun rets -> ret :: rets |> finalContinuation))

let mkResultDivContent range text =
    let range =
        $"({range.StartLine},{range.StartColumn}-{range.EndLine},{range.EndColumn})"

    sprintf "%s %s" text range

let mkTriviaResultDiv (dispatch: Msg -> unit) (isBefore: bool) (key: string) (triviaNode: TriviaNode) : ReactElement =
    let className =
        match triviaNode.Type with
        | "commentOnSingleLine"
        | "lineCommentAfterSourceCode"
        | "blockComment" -> "comment"
        | "newline" -> "newline"
        | "directive" -> "directive"
        | _ -> ""

    let title =
        sprintf "%c%s" (System.Char.ToUpper triviaNode.Type[0]) (triviaNode.Type[1..])

    let iconClassName =
        if isBefore then
            "fa-solid fa-arrow-turn-up fa-flip-both"
        else
            "fa-solid fa-arrow-turn-up fa-flip-horizontal"

    let content =
        mkResultDivContent triviaNode.Range (Option.defaultValue "Newline" triviaNode.Content)

    div [
        Title title
        Key key
        ClassName className
        OnClick(fun ev ->
            ev.stopPropagation ()
            let div = (ev.target :?> Element)
            div.classList.add "highlight"
            JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore
            dispatch (Bubble(BubbleMessage.HighLight triviaNode.Range)))
    ] [ i [ ClassName iconClassName ] []; str content ]

let rec mkResultDiv
    (dispatch: Msg -> unit)
    (level: int)
    (key: string)
    (node: OakNode)
    (continuation: ReactElement array -> ReactElement array)
    =
    let continuations =
        node.Children
        |> Array.mapi (fun idx child ->
            let key = $"{key}_{idx}"
            mkResultDiv dispatch (level + 1) key child)
        |> Array.toList

    let contentBefore =
        Array.mapi (fun idx -> mkTriviaResultDiv dispatch true $"{key}_cb_{idx}") node.ContentBefore

    let contentAfter =
        Array.mapi (fun idx -> mkTriviaResultDiv dispatch false $"{key}_ca_{idx}") node.ContentAfter

    let current =
        let content =
            mkResultDivContent node.Range (Option.defaultValue node.Type node.Text)

        div [
            Title node.Type
            Key key
            Props.Style [ MarginLeft $"{level * 10}px" ]
            OnClick(fun ev ->
                ev.stopPropagation ()
                let div = (ev.target :?> Element)
                div.classList.add "highlight"
                JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore

                dispatch (Bubble(BubbleMessage.HighLight node.Range)))
        ] [ yield! contentBefore; yield str content; yield! contentAfter ]

    let finalContinuation (elements: ReactElement array list) =
        continuation [| yield current; yield! (Seq.collect id elements) |]

    Continuation.sequence continuations finalContinuation

let results (oak: OakNode) dispatch =
    div [ Id "oak-tab"; ClassName Style.TabContent ] [
        let lines = mkResultDiv dispatch 0 "root" oak id
        ofArray lines
    ]

let view (model: Model) dispatch =
    match model.State with
    | OakViewerTabState.Result oakNode ->
        if model.IsGraphView then
            Oak.GraphView.view (oakNode, model, dispatch)
        else
            results oakNode dispatch
    | OakViewerTabState.Error errors -> ReadOnlyEditor errors
    | OakViewerTabState.Loading -> Loader.tabLoading

let commands dispatch =
    button [ ClassName Style.Primary; OnClick(fun _ -> dispatch GetOak) ] [
        i [ ClassName $"fas fa-code" ] []
        str "Get oak"
    ]

let settings (bubble: BubbleModel) (model: Model) dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "trivia-defines"
            (BubbleMessage.SetDefines >> Bubble >> dispatch)
            (str "Defines")
            "Enter your defines separated with a space"
            bubble.Defines
        SettingControls.toggleButton
            (fun _ -> dispatch (Bubble(BubbleMessage.SetFsi true)))
            (fun _ -> dispatch (Bubble(BubbleMessage.SetFsi false)))
            "*.fsi"
            "*.fs"
            (str "File extension")
            bubble.IsFsi
        SettingControls.toggleButton
            (fun _ -> dispatch (SetGraphView true))
            (fun _ -> dispatch (SetGraphView false))
            "Graph view enabled"
            "Graph view disabled"
            (str "Show AST in interactive graph view")
            model.IsGraphView
        if model.IsGraphView then
            yield!
                [ SettingControls.multiButton "Graph view layout" [
                      { Label = "Top-down layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout TopDown))
                        IsActive = model.GraphViewOptions.Layout = TopDown }
                      { Label = "Left-right layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout LeftRight))
                        IsActive = model.GraphViewOptions.Layout = LeftRight }
                      { Label = "Free layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout Free))
                        IsActive = model.GraphViewOptions.Layout = Free }
                  ]
                  SettingControls.input
                      "graph-view-node-limit"
                      (int >> SetGraphViewNodeLimit >> dispatch)
                      (str "Graph view node limit")
                      "Max nodes in graph view"
                      model.GraphViewOptions.NodeLimit
                  SettingControls.multiButton "Graph view scaling by subtree size" [
                      { Label = "No scaling"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale NoScale))
                        IsActive = model.GraphViewOptions.Scale = NoScale }
                      { Label = "Scale only collapsed subtree nodes"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale SubTreeNodes))
                        IsActive = model.GraphViewOptions.Scale = SubTreeNodes }
                      { Label = "Scale all nodes"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale AllNodes))
                        IsActive = model.GraphViewOptions.Scale = AllNodes }
                  ]
                  SettingControls.input
                      "graph-view-scale-max-size"
                      (int >> SetGraphViewScaleMax >> dispatch)
                      (str "Graph view scale max size limit")
                      "Max size of scaled node"
                      model.GraphViewOptions.ScaleMaxSize ]
    ]
