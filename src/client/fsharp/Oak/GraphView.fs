module FantomasTools.Client.Oak.GraphView

open System.Collections.Generic
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Editor
open FantomasTools.Client.OakViewer.Graph
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView
open FantomasTools.Client.Utils

type NodeType =
    | Standard
    | Comment
    | Newline
    | Directive

type GraphOakNode =
    {
        Id: int
        /// The clr type of the node
        Title: string
        /// The text of the node, could be the type name or the text of the node
        Node: string
        Type: NodeType
        Coords: HighLightRange
        Children: GraphOakNode list
        Limited: bool
        Level: int
        Size: int
    }

let nodesFromRoot root =
    let rec getChildren acc n =
        n.Children
        |> Seq.fold (fun s x -> getChildren s x |> Set.add x) (acc |> Set.add n)

    let oakNodes = getChildren Set.empty root
    let nodeMap = oakNodes |> Seq.map (fun n -> NodeId n.Id, n) |> Map.ofSeq
    nodeMap

let private parseResults =
    let mutable nodeIdCounter = 0

    let rec parseNode level (n: OakNode) =
        let mkNode (level: int) (title: string) (text: string) (range: HighLightRange) (t: NodeType) : GraphOakNode =
            let n =
                { Id = nodeIdCounter
                  Node = text
                  Title = title
                  Coords = range
                  Type = t
                  Children = []
                  Limited = false
                  Level = level
                  Size = 1 }

            nodeIdCounter <- nodeIdCounter + 1
            n

        let parseTriviaNode level (t: TriviaNode) =
            let typ =
                match t.Type with
                | "directive" -> Directive
                | "newline" -> Newline
                | s when s.ToLower().Contains "comment" -> Comment
                | _ -> Standard

            let title = sprintf "%c%s" (System.Char.ToUpper t.Type.[0]) (t.Type[1..])
            mkNode level title (t.Content |> Option.defaultValue t.Type) t.Range typ

        let parseNodeOrTriviaNode level n =
            match n with
            | Choice1Of2 x -> parseNode level x
            | Choice2Of2 x -> parseTriviaNode level x

        let children =
            [ yield! n.ContentBefore |> Array.map Choice2Of2
              yield! n.Children |> Array.map Choice1Of2
              yield! n.ContentAfter |> Array.map Choice2Of2 ]
            |> List.map (parseNodeOrTriviaNode (level + 1))

        let node =
            let n =
                let text = Option.defaultValue n.Type n.Text
                mkNode level n.Type text n.Range Standard

            { n with
                Children = children
                Size = n.Size + (children |> Seq.sumBy (fun x -> x.Size)) }

        node

    memoize (fun (model: Model) ->
        Option.map
            (fun oak ->
                nodeIdCounter <- 0
                parseNode 0 oak)
            model.Oak)

let fullGraph =
    memoize (fun model -> parseResults model |> Option.map (fun r -> r, nodesFromRoot r))

let limitTree =
    memoize2 (fun allowedSet n ->
        let rec f n =
            let children = n.Children |> List.filter (fun c -> List.contains c allowedSet)
            let limit = not (List.isEmpty n.Children) && List.isEmpty children

            { n with
                Children = children |> List.map f
                Limited = limit }

        f n)

let limitTreeByNodes =
    memoize2 (fun maxNodes n ->
        let q = Queue<GraphOakNode>()
        q.Enqueue n

        let rec loop acc i =
            if i >= maxNodes then
                acc
            else
                match q.TryDequeue() with
                | false, _ -> acc
                | true, x ->
                    x.Children |> List.iter q.Enqueue
                    loop (x :: acc) (i + 1)

        let allowedNodes = loop [] 1
        limitTree allowedNodes n)

let view =
    // copied from variables.sass
    let colors =
        {| dark = "#222222"
           primary = "#338CBB"
           secondary = "#2d94b0"
           danger = "#C74910"
           warning = "#C7901B"
           success = "#88D1A6"
           white = "#FFF"
           grey = "#DDD"
           purple300 = "#a98eda" |}

    let getColor =
        function
        | Standard -> colors.primary
        | Comment -> colors.success
        | Newline -> colors.grey
        | Directive -> colors.secondary

    let getFontColor =
        function
        | Newline -> colors.dark
        | _ -> colors.white

    let minScaling = 10

    memoizeBy fst (fun (model, dispatch: Msg -> unit) ->
        let root =
            fullGraph model
            |> Option.map (fun (root, nodeMap) ->
                model.GraphViewRootNodes
                |> List.tryHead
                |> Option.bind (fun nId -> Map.tryFind nId nodeMap)
                |> Option.defaultValue root)

        match root with
        | Some root ->
            let root = limitTreeByNodes model.GraphViewOptions.NodeLimit root
            let oakNodes = nodesFromRoot root

            let scalingLabel =
                let opt =
                    {| enabled = true
                       min = minScaling
                       max = model.GraphViewOptions.ScaleMaxSize |}

                match model.GraphViewOptions.Scale with
                | NoScale -> {| opt with enabled = false |}
                | SubTreeNodes
                | AllNodes -> opt

            let nodes: VisNetwork.node array =
                oakNodes
                |> Map.toArray
                |> Array.map (fun (_, graphOakNode) ->
                    let scaleValue =
                        match model.GraphViewOptions.Scale with
                        | NoScale -> minScaling
                        | SubTreeNodes ->
                            if not graphOakNode.Limited then
                                minScaling
                            else
                                graphOakNode.Size
                        | AllNodes -> minScaling + graphOakNode.Size - 1

                    {| id = !!graphOakNode.Id
                       label = graphOakNode.Node.Trim()
                       title = graphOakNode.Title
                       level = graphOakNode.Level
                       color = getColor graphOakNode.Type
                       shape = if graphOakNode.Limited then "box" else "ellipse"
                       value = scaleValue
                       font = {| color = getFontColor graphOakNode.Type |} |})

            let edges: VisNetwork.edge array =
                oakNodes
                |> Map.values
                |> Seq.collect (fun n ->
                    n.Children
                    |> Seq.map (fun m ->
                        if m.Type = Standard then
                            {| from = !!n.Id
                               ``to`` = !!m.Id
                               dashes = false |}
                            : VisNetwork.edge
                        else
                            {| from = !!m.Id
                               ``to`` = !!n.Id
                               dashes = true |}))
                |> Seq.toArray

            let layout =
                let hier =
                    {| enabled = true
                       direction = "UD"
                       levelSeparation = 75 |}

                match model.GraphViewOptions.Layout with
                | TopDown -> {| hierarchical = hier |}
                | LeftRight -> {| hierarchical = {| hier with direction = "LR" |} |}
                | Free -> {| hierarchical = {| hier with enabled = false |} |}

            let parentElement = Browser.Dom.document.getElementById "tab-content"

            let options: VisNetwork.options =
                {| layout = layout
                   interaction = {| hover = true |}
                   width = $"{parentElement.clientWidth}"
                   height = $"{parentElement.clientHeight}"
                   nodes = {| scaling = {| label = scalingLabel |} |} |}

            let graph =
                Graph
                    {| options = options
                       data =
                        {| nodes = VisNetwork.DataSet(!!nodes)
                           edges = VisNetwork.DataSet(!!edges) |}
                       selectNode =
                        (fun ev ->
                            for nodeId in ev.nodes do
                                dispatch (GraphViewSetRoot(NodeId nodeId)))
                       hoverNode = (fun ev -> dispatch (HighLight oakNodes.[NodeId ev.node].Coords)) |}

            fragment [] [
                graph
                div [ Id "graph-view-commands" ] [
                    if model.GraphViewRootNodes <> [] then
                        button [
                            ClassName $"{Style.Btn} {Style.BtnPrimary} {Style.TextWhite}"
                            OnClick(fun _ -> dispatch GraphViewGoBack)
                        ] [ str $"<- back({model.GraphViewRootNodes.Length})" ]
                ]
            ]
        | None -> div [] [])
