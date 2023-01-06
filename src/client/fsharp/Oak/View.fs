module FantomasTools.Client.OakViewer.View

open System
open Browser.Types
open Fable.Core
open FantomasTools.Client.Editor
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Graph
open Feliz
open Reactstrap

type NodeType =
    | Std
    | Comment
    | Newline
    | Directive

type OakLine = {
    Id: int
    Node: string
    Type: NodeType
    Coords: HighLightRange
}

let memoizeBy (g: 'a -> 'c) (f: 'a -> 'b) =
    let cache = System.Collections.Generic.Dictionary<_, _>()

    fun x ->
        let key = g x

        if cache.ContainsKey key then
            cache[key]
        else
            let y = f x
            cache.Add(key, y)
            y

let inline memoize f = memoizeBy id f

let private parseResults =
    memoize
    <| fun (model: Model) ->
        let lines =
            model.Oak.Split([| '\n' |])
            |> Array.mapi (fun idx line ->
                let nodeType =
                    let trimmedLine = line.TrimStart()

                    if trimmedLine.StartsWith("//") || trimmedLine.StartsWith("(*") then
                        Comment
                    elif trimmedLine.StartsWith("Newline") then
                        Newline
                    elif trimmedLine.StartsWith("#") then
                        Directive
                    else
                        Std

                let range =
                    if line.Trim() = "" then
                        None
                    else
                        let coords =
                            line
                            |> Seq.filter (fun (c: char) -> Char.IsDigit c || c = '-' || c = ',')
                            |> Seq.skipWhile (fun (c: char) -> not (Char.IsDigit c))
                            |> fun chars ->
                                Seq.foldBack
                                    (fun (c: char) (acc: string list) ->
                                        match acc with
                                        | [] -> acc
                                        | current :: rest ->
                                            if Char.IsDigit c then
                                                $"{c}{current}" :: rest
                                            else
                                                "" :: acc)
                                    chars
                                    [ "" ]
                            |> Seq.map int
                            |> Seq.toList

                        match coords with
                        | [ startLine; startColumn; endLine; endColumn ] ->
                            let highlightRange: HighLightRange = {
                                StartLine = startLine
                                StartColumn = startColumn
                                EndLine = endLine
                                EndColumn = endColumn
                            }

                            Some highlightRange
                        | _ ->
                            JS.console.log $"Could not construct highlight range, got %A{coords}"
                            None

                let nodeString =
                    line |> Seq.takeWhile ((<>) '(') |> Seq.map string |> String.concat ""

                range
                |> Option.map (fun r -> {
                    Id = idx
                    Node = nodeString
                    Coords = r
                    Type = nodeType
                }))
            |> Array.choose id
            |> Array.toList

        // create edges in graph based on indentation of lines
        let edges =
            let getLevel s = s |> Seq.takeWhile ((=) ' ') |> Seq.length

            match lines with
            | [] -> []
            | hd :: tl ->
                (([ hd, getLevel hd.Node ], []), tl)
                ||> Seq.fold (fun (parents, acc) n ->
                    match parents with
                    | [] -> [], acc
                    | (parent, level) :: otherParents ->
                        let l = getLevel n.Node

                        if l > level then
                            ((n, l) :: parents), ((parent.Id, n.Id) :: acc)
                        else
                            match otherParents |> List.skipWhile (fun (_, lp) -> l < lp) with
                            | [] -> (otherParents, acc)
                            | (p, _) :: _ as filteredParents -> ((n, l) :: filteredParents), ((p.Id, n.Id) :: acc))
                |> snd

        lines, edges

let createGraph =
    memoizeBy fst
    <| fun (model, dispatch: Msg -> unit) ->
        let lines, parsedEdges = parseResults model
        let lines = lines |> Seq.map (fun n -> NodeId n.Id, n) |> Map.ofSeq
        let Nodes = lines |> Map.map (fun _ n -> NodeLabel(n.Node.Trim()))
        let Edges = parsedEdges |> Seq.map (fun (i, j) -> NodeId i, NodeId j) |> set

        VisReact.graph Nodes Edges (fun nId -> Fable.Core.JS.console.log lines[nId]) (fun nId ->
            dispatch (HighLight lines[nId].Coords))

let private results (model: Model) dispatch =
    let lines =
        parseResults model
        |> fst
        |> Seq.map (fun n ->
            let className =
                match n.Type with
                | Comment -> "comment"
                | Newline -> "newline"
                | Directive -> "directive"
                | Std -> ""

            div [
                Key !!n.Id
                OnClick(fun ev ->
                    ev.stopPropagation ()
                    let div = (ev.target :?> Element)
                    div.classList.add "highlight"
                    JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore

                    dispatch (HighLight n.Coords))
            ] [ pre [ ClassName className ] [ str n.Node ] ])
        |> Seq.toArray

    div [ Id "oakResult" ] [ ofArray lines ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        match model.Error, model.ShowGraph with
        | None, false -> results model dispatch
        | None, true -> createGraph (model, dispatch)
        | Some errors, _ -> Editor true [ MonacoEditorProp.DefaultValue errors ]

let commands dispatch =
    fragment [] [
        Button.button [
            Button.Color Primary
            Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch GetOak) ]
        ] [ i [ ClassName "fas fa-code mr-1" ] []; str "Get oak" ]
        Button.button [
            Button.Color Secondary
            Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch ShowGraph) ]
        ] [ i [ ClassName "fas fa-code mr-1" ] []; str "Show graph" ]
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
    ]
