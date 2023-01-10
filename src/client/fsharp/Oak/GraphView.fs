module FantomasTools.Client.Oak.GraphView

open System.Collections.Generic
open Fable.React
open Fable.React.Props
open FantomasTools.Client.Editor
open FantomasTools.Client.OakViewer.Graph
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView
open FantomasTools.Client.Utils
open Reactstrap

type NodeType =
    | Standard
    | Comment
    | Newline
    | Directive

type GraphOakNode =
    { Id: int
      Node: string
      Type: NodeType
      Coords: HighLightRange
      CoordsUnion: HighLightRange
      Children: GraphOakNode list
      Limited: bool
      Level: int
      Size: int }

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
        let mkNode (level: int) (text: string) (range: HighLightRange) (t: NodeType) : GraphOakNode =
            let n =
                { Id = nodeIdCounter
                  Node = text
                  Coords = range
                  CoordsUnion = range
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
                | s when s.StartsWith "comment" -> Comment
                | _ -> Standard

            mkNode level (t.Content |> Option.defaultValue t.Type) t.Range typ

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
            let n = mkNode level n.Type n.Range Standard
            let ranges = n.Coords :: (children |> List.map (fun x -> x.CoordsUnion))

            let unionRange =
                { StartLine = ranges |> Seq.map (fun n -> n.StartLine) |> Seq.min
                  StartColumn = ranges |> Seq.map (fun n -> n.StartColumn) |> Seq.min
                  EndLine = ranges |> Seq.map (fun n -> n.EndLine) |> Seq.max
                  EndColumn = ranges |> Seq.map (fun n -> n.EndColumn) |> Seq.max }

            { n with
                Children = children
                CoordsUnion = unionRange
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
           secondary = "#2FBADC"
           danger = "#C74910"
           warning = "#C7901B"
           success = "#88D1A6"
           white = "#FFF" |}

    let getColor =
        function
        | Standard -> colors.secondary
        | Comment -> colors.success
        | Newline -> colors.warning
        | Directive -> colors.primary

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

            let nodes =
                oakNodes
                |> Map.map (fun _ n ->
                    { Label = NodeLabel(n.Node.Trim())
                      Color = NodeColor(getColor n.Type)
                      Shape = if n.Limited then Box else Ellipse
                      ScaleValue =
                        match model.GraphViewOptions.Scale with
                        | NoScale -> 1
                        | SubTreeNodes -> if not n.Limited then 1 else n.Size
                        | AllNodes -> n.Size })

            let edges =
                oakNodes
                |> Map.values
                |> Seq.collect (fun n -> n.Children |> Seq.map (fun m -> NodeId n.Id, NodeId m.Id))
                |> set

            let graph =
                VisReact.graph
                    model.GraphViewOptions
                    "tab-content"
                    nodes
                    edges
                    (fun nId -> dispatch (GraphViewSetRoot nId))
                    (fun nId -> dispatch (HighLight oakNodes[nId].CoordsUnion))

            fragment
                []
                [ graph
                  div
                      [ Id "graph-view-commands" ]
                      [ if model.GraphViewRootNodes <> [] then
                            Button.button
                                [ Button.Color Primary
                                  Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch GraphViewGoBack) ] ]
                                [ str $"<- back({model.GraphViewRootNodes.Length})" ] ] ]
        | None -> div [] [])
