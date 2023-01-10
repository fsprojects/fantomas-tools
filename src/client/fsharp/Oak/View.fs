module FantomasTools.Client.OakViewer.View

open Browser.Types
open Fable.Core
open FantomasTools.Client.Editor
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView
open Reactstrap

[<RequireQualifiedAccess>]
module Continuation =
    let rec sequence<'a, 'ret> (recursions: (('a -> 'ret) -> 'ret) list) (finalContinuation: 'a list -> 'ret) : 'ret =
        match recursions with
        | [] -> [] |> finalContinuation
        | recurse :: recurses -> recurse (fun ret -> sequence recurses (fun rets -> ret :: rets |> finalContinuation))

let mkResultDivContent level range text =
    let range =
        $"({range.StartLine},{range.StartColumn}-{range.EndLine},{range.EndColumn})"

    sprintf "%s%s %s" (String.replicate level "  ") text range

let mkTriviaResultDiv dispatch level (key: string) (triviaNode: TriviaNode) : ReactElement =
    let className =
        match triviaNode.Type with
        | "commentOnSingleLine"
        | "lineCommentAfterSourceCode"
        | "blockComment" -> "comment"
        | "newline" -> "newline"
        | "directive" -> "directive"
        | _ -> ""

    let content =
        mkResultDivContent level triviaNode.Range (Option.defaultValue "Newline" triviaNode.Content)

    div [
        Key key
        OnClick(fun ev ->
            ev.stopPropagation ()
            let div = (ev.target :?> Element)
            div.classList.add "highlight"
            JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore
            dispatch (HighLight triviaNode.Range))
    ] [ pre [ ClassName className ] [ str content ] ]

let rec mkResultDiv
    dispatch
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

    let current =
        let content =
            mkResultDivContent level node.Range (Option.defaultValue node.Type node.Text)

        div [
            Key key
            OnClick(fun ev ->
                ev.stopPropagation ()
                let div = (ev.target :?> Element)
                div.classList.add "highlight"
                JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore

                dispatch (HighLight node.Range))
        ] [ pre [] [ str content ] ]

    let contentBefore =
        Array.mapi (fun idx tn -> mkTriviaResultDiv dispatch level $"{key}_cb_{idx}" tn) node.ContentBefore

    let contentAfter =
        Array.mapi (fun idx tn -> mkTriviaResultDiv dispatch level $"{key}_ca_{idx}" tn) node.ContentAfter

    let finalContinuation (elements: ReactElement array list) =
        continuation [|
            yield! contentBefore
            yield current
            yield! (Seq.collect id elements)
            yield! contentAfter
        |]

    Continuation.sequence continuations finalContinuation

let private results (model: Model) dispatch =
    match model.Oak with
    | None -> div [ Id "oakResult" ] []
    | Some oak ->
        let lines = mkResultDiv dispatch 0 "root" oak id
        div [ Id "oakResult" ] [ ofArray lines ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        match model.Error, model.IsGraphView with
        | None, false -> results model dispatch
        | None, true -> Oak.GraphView.view (model, dispatch)
        | Some errors, _ -> Editor true [ MonacoEditorProp.DefaultValue errors ]

let commands dispatch =
    fragment [] [
        Button.button [
            Button.Color Primary
            Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch GetOak) ]
        ] [ i [ ClassName "fas fa-code mr-1" ] []; str "Get oak" ]
    ]

let settings isFsi (model: Model) dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "trivia-defines"
            (DefinesUpdated >> dispatch)
            (str "Defines")
            "Enter your defines separated with a space"
            model.Defines
        SettingControls.toggleButton
            (fun _ -> dispatch (SetFsiFile true))
            (fun _ -> dispatch (SetFsiFile false))
            "*.fsi"
            "*.fs"
            (str "File extension")
            isFsi
        SettingControls.toggleButton
            (fun _ -> dispatch (SetStroustrup true))
            (fun _ -> dispatch (SetStroustrup false))
            "Stroustrup enabled"
            "Stroustrup disabled"
            (str "Is stroustrup?")
            model.IsStroustrup
        SettingControls.toggleButton
            (fun _ -> dispatch (SetGraphView true))
            (fun _ -> dispatch (SetGraphView false))
            "Graph view enabled"
            "Graph view disabled"
            (str "Show AST in interactive graph view")
            model.IsGraphView
        if model.IsGraphView then
            yield! [
                SettingControls.multiButton "Graph view layout" [
                    {
                        Label = "Top-down layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout TopDown))
                        IsActive = model.GraphViewOptions.Layout = TopDown
                    }
                    {
                        Label = "Left-right layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout LeftRight))
                        IsActive = model.GraphViewOptions.Layout = LeftRight
                    }
                    {
                        Label = "Free layout"
                        OnClick = (fun _ -> dispatch (SetGraphViewLayout Free))
                        IsActive = model.GraphViewOptions.Layout = Free
                    }
                ]
                SettingControls.input
                    "graph-view-node-limit"
                    (int >> SetGraphViewNodeLimit >> dispatch)
                    (str "Graph view node limit")
                    "Max nodes in graph view"
                    model.GraphViewOptions.NodeLimit
                SettingControls.multiButton "Graph view scaling by subtree size" [
                    {
                        Label = "No scaling"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale NoScale))
                        IsActive = model.GraphViewOptions.Scale = NoScale
                    }
                    {
                        Label = "Scale only collapsed subtree nodes"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale SubTreeNodes))
                        IsActive = model.GraphViewOptions.Scale = SubTreeNodes
                    }
                    {
                        Label = "Scale all nodes"
                        OnClick = (fun _ -> dispatch (SetGraphViewScale AllNodes))
                        IsActive = model.GraphViewOptions.Scale = AllNodes
                    }
                ]
                SettingControls.input
                    "graph-view-scale-max-size"
                    (int >> SetGraphViewScaleMax >> dispatch)
                    (str "Graph view scale max size limit")
                    "Max size of scaled node"
                    model.GraphViewOptions.NodeLimit
            ]
    ]
