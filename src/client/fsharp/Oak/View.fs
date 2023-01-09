module FantomasTools.Client.OakViewer.View

open System
open System.Collections.Generic
open Browser.Types
open Fable.Core
open FantomasTools.Client.Editor
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Utils
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Graph
open FantomasTools.Client.OakViewer.Model.GraphView
open Reactstrap

type NodeType =
    | Standard
    | Comment
    | Newline
    | Directive

type OakNode = {
    Id: int
    Node: string
    Type: NodeType
    Coords: HighLightRange
    CoordsUnion: HighLightRange
    Childs: OakNode list
    Limited: bool
    Size: int
}

let nodesFromRoot root =
    let rec getChilds acc n =
        n.Childs |> Seq.fold (fun s x -> getChilds s x |> Set.add x) (acc |> Set.add n)

    let oakNodes = getChilds Set.empty root
    let nodeMap = oakNodes |> Seq.map (fun n -> NodeId n.Id, n) |> Map.ofSeq
    nodeMap

let private parseResults =
    memoize (fun (model: Model) ->
        let nodes =
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
                        Standard

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
                    CoordsUnion = r
                    Type = nodeType
                    Childs = []
                    Limited = false
                    Size = 1
                }))
            |> Array.choose id
            |> Array.toList

        if nodes = [] then
            Map.empty
        else
            let nodesMap = nodes |> Seq.map (fun n -> n.Id, n) |> Map.ofSeq

            // create edges in graph based on indentation of lines
            let edges =
                let getLevel s = s |> Seq.takeWhile ((=) ' ') |> Seq.length

                match nodes with
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
                |> List.groupBy fst
                |> Seq.map (fun (k, g) -> k, g |> List.map snd)
                |> Map.ofSeq

            /// Recursively update nodes with links to childs, range info and size
            let rec setChilds (n: OakNode) =
                let childs =
                    edges
                    |> Map.tryFind n.Id
                    |> Option.defaultValue []
                    |> List.map (fun idx -> nodesMap[idx] |> setChilds)

                let ranges = n.Coords :: (childs |> List.map (fun x -> x.CoordsUnion))

                let unionRange = {
                    StartLine = ranges |> Seq.map (fun n -> n.StartLine) |> Seq.min
                    StartColumn = ranges |> Seq.map (fun n -> n.StartColumn) |> Seq.min
                    EndLine = ranges |> Seq.map (fun n -> n.EndLine) |> Seq.max
                    EndColumn = ranges |> Seq.map (fun n -> n.EndColumn) |> Seq.max
                }

                { n with
                    Childs = childs
                    CoordsUnion = unionRange
                    Size = n.Size + (childs |> Seq.sumBy (fun x -> x.Size))
                }

            let root = nodes |> List.head |> setChilds
            nodesFromRoot root)

let fullGraph = memoize (fun model -> parseResults model)

let limitTree =
    memoize2 (fun allowedSet n ->
        let rec f n =
            let childs = n.Childs |> List.filter (fun c -> List.contains c allowedSet)
            let limit = not (List.isEmpty n.Childs) && List.isEmpty childs

            { n with
                Childs = childs |> List.map f
                Limited = limit
            }

        f n)

let limitTreeByNodes =
    memoize2 (fun maxNodes n ->
        let q = Queue<OakNode>()
        q.Enqueue n

        let rec loop acc i =
            if i >= maxNodes then
                acc
            else
                match q.TryDequeue() with
                | false, _ -> acc
                | true, x ->
                    x.Childs |> List.iter q.Enqueue
                    loop (x :: acc) (i + 1)

        let allowedNodes = loop [] 1
        limitTree allowedNodes n)

let createGraph =
    // copied from variables.sass
    let colors = {|
        dark = "#222222"
        primary = "#338CBB"
        secondary = "#2FBADC"
        danger = "#C74910"
        warning = "#C7901B"
        success = "#88D1A6"
        white = "#FFF"
    |}

    let getColor =
        function
        | Standard -> colors.secondary
        | Comment -> colors.success
        | Newline -> colors.warning
        | Directive -> colors.primary

    memoizeBy fst (fun (model, dispatch: Msg -> unit) ->
        let nodeMap = fullGraph model

        let root =
            if Map.isEmpty nodeMap then
                None
            else
                model.GraphViewRootNodes
                |> List.tryHead
                |> Option.bind (fun nId -> Map.tryFind nId nodeMap)
                |> Option.orElse (nodeMap |> Map.tryFind (NodeId 0))

        match root with
        | Some root ->
            let root = limitTreeByNodes model.GraphViewOptions.NodeLimit root
            let oakNodes = nodesFromRoot root

            let nodes =
                oakNodes
                |> Map.map (fun _ n -> {
                    Label = NodeLabel(n.Node.Trim())
                    Color = NodeColor(getColor n.Type)
                    Shape = if n.Limited then Box else Ellipse
                    ScaleValue =
                        match model.GraphViewOptions.Scale with
                        | NoScale -> 1
                        | SubTreeNodes -> if not n.Limited then 1 else n.Size
                        | AllNodes -> n.Size
                })

            let edges =
                oakNodes
                |> Map.values
                |> Seq.collect (fun n -> n.Childs |> Seq.map (fun m -> NodeId n.Id, NodeId m.Id))
                |> set

            let graph =
                VisReact.graph
                    model.GraphViewOptions
                    "tab-content"
                    nodes
                    edges
                    (fun nId -> dispatch (GraphViewSetRoot nId))
                    (fun nId -> dispatch (HighLight nodeMap[nId].CoordsUnion))

            fragment [] [
                graph
                div [ Id "graph-view-commands" ] [
                    if model.GraphViewRootNodes <> [] then
                        Button.button [
                            Button.Color Primary
                            Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch GraphViewGoBack) ]
                        ] [ str $"<- back({model.GraphViewRootNodes.Length})" ]
                ]
            ]
        | None -> div [] [])

let private results (model: Model) dispatch =
    let lines =
        parseResults model
        |> Map.toSeq
        |> Seq.sortBy fst
        |> Seq.map (fun (_, n) ->
            let className =
                match n.Type with
                | Comment -> "comment"
                | Newline -> "newline"
                | Directive -> "directive"
                | Standard -> ""

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
        match model.Error, model.IsGraphView with
        | None, false -> results model dispatch
        | None, true -> createGraph (model, dispatch)
        | Some errors, _ -> Editor true [ MonacoEditorProp.DefaultValue errors ]

let commands model dispatch =
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
