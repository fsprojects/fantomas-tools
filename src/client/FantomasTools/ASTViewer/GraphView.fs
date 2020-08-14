module FantomasTools.Client.ASTViewer.GraphView

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fable.Import
open ASTViewer
open FantomasTools.Client.ASTViewer.Model

let log (msg, x: obj) = Browser.Dom.console.log (msg, x)

type TreeNode<'a> =
    { Label: string
      Color: string option
      Tooltip: string
      Childrens: TreeNode<'a> list
      Original: 'a }

// We import the vis.css style
importSideEffects "vis/dist/vis.css"

// We create an access to the vis library
// We could put it under vis.fs file but it's easier to access it from here
[<Import("*", "vis")>]
let visLib: Vis.IExports = jsNative

// Helper to make the code easier to read
let createNode id label color tooltip level =
    jsOptions<Vis.Node> (fun o ->
        o.id <- Some !^id
        o.label <- Some label
        color
        |> Option.iter (fun c ->
            o.color <-
                Some
                    (U2.Case2
                     <| jsOptions<Vis.Color> (fun o -> o.background <- Some c)))
        o.title <- Some tooltip
        o.size <- Some 100.
        o.level <- Some(float level))

// Helper to make the code easier to read
let createEdge from ``to`` =
    jsOptions<Vis.Edge> (fun o ->
        o.from <- Some !^from
        o.``to`` <- Some !^``to``)

let createGraph nodes edges =
    jsOptions<Vis.Data> (fun o ->
        o.nodes <- Some(U2.Case2 nodes)
        o.edges <- Some(U2.Case2 edges))

let inline stringify (x: TreeNode<Shared.Node>) = Thoth.Json.Encode.Auto.toString (0, x)

let buildGraph (opts: Graph.Options) (root: TreeNode<_>) =
    let getNodes =
        let rec f depth node =
            (node, depth)
            :: List.collect (f (depth + 1)) node.Childrens

        f 0

    let allNodes = getNodes root

    let nodesToId = allNodes |> List.mapi (fun i (n, _) -> n, float i)

    let nodesToIdMap =
        nodesToId
        |> Seq.map (fun (n, i) -> stringify n, i)
        |> Map.ofSeq

    let getId n = nodesToIdMap.[stringify n] // node as key dont work for some reason

    let idToDepth =
        allNodes
        |> List.mapi (fun i (_, d) -> float i, d)
        |> Seq.toList

    let idToNodes =
        nodesToId
        |> Seq.map (fun (n, i) -> i, n)
        |> Map.ofSeq

    let rec getEdges node =
        (node.Childrens
         |> List.map (fun c -> getId node, getId c))
        @ (List.collect getEdges node.Childrens)

    let allEdges = getEdges root

    let nodeLevel =
        let maxInRow = opts.MaxNodesInRow
        idToDepth
        |> List.groupBy snd
        |> List.sortBy fst
        |> List.map (fun (_, g) ->
            let n = List.length g
            let k = n / maxInRow + 1
            g
            |> List.mapi (fun i (x, _) -> x, i % k)
            |> List.groupBy snd
            |> List.sortBy fst
            |> List.map (snd >> List.map fst))
        |> List.collect id
        |> List.mapi (fun i xs -> xs |> List.map (fun x -> x, i))
        |> List.collect id
        |> Map.ofList

    let nodes =
        nodesToId
        |> Seq.map (fun (n, i) ->
            createNode
                i
                n.Label
                n.Color
                n.Tooltip
                (nodeLevel
                 |> Map.tryFind i
                 |> Option.defaultValue 0))
        |> ResizeArray
        |> visLib.DataSet.Create

    let edges =
        allEdges
        |> Seq.map (fun (v, w) -> createEdge v w)
        |> ResizeArray
        |> visLib.DataSet.Create

    createGraph nodes edges, (fun (nodeId: float) -> idToNodes.[nodeId])

type Props<'a> =
    | Tree of TreeNode<'a>
    | OnHover of ('a -> unit)
    | OnSelect of ('a -> unit)
    | Options of Graph.Options

type GraphView(props: Props<obj> list, ctx) =
    inherit Component<obj, unit>(props, ctx)

    override x.render() =
        let view = div [ Id "graph"; Class "graph" ] []
        view

    override __.componentWillUpdate(prevProps, _) =
        let getProp f = (prevProps :?> Props<'a> list) |> List.tryPick f

        let tree =
            getProp (function
                | Tree t -> Some t
                | _ -> None)
            |> Option.defaultValue
                { Label = "Empty"
                  Color = None
                  Tooltip = ""
                  Childrens = []
                  Original = Unchecked.defaultof<_> }

        let onHover =
            getProp (function
                | OnHover f -> Some f
                | _ -> None)

        let onSelect =
            getProp (function
                | OnSelect f -> Some f
                | _ -> None)

        let opts =
            getProp (function
                | Options o -> Some o
                | _ -> None)
            |> Option.defaultValue (State.initialGraphModel.Options)

        let container = Browser.Dom.document.getElementById ("graph")

        let options =
            jsOptions<Vis.Options> (fun o ->
                o.autoResize <- Some true
                o.edges <- Some(jsOptions<Vis.EdgeOptions> (fun e -> e.arrows <- Some <| U2.Case1 "to"))
                o.interaction <-
                    Some
                        (createObj [ "hover" ==> true
                                     "zoomView" ==> true
                                     "hoverConnectedEdges" ==> false ])
                o.layout <- Some(createObj [ "randomSeed" ==> 0 ])

                let hierOpts dir =
                    createObj [ "enabled" ==> true
                                "levelSeparation" ==> 170
                                "nodeSpacing" ==> 100
                                "treeSpacing" ==> 100
                                "direction" ==> dir ]

                let layout =
                    match opts.Layout with
                    | Graph.Free -> createObj []
                    | Graph.HierarchicalLeftRight -> createObj [ "hierarchical" ==> hierOpts "LR" ]
                    | Graph.HierarchicalUpDown -> createObj [ "hierarchical" ==> hierOpts "UD" ]

                o.layout <- Some layout)

        let (graph, idToNode) = buildGraph opts tree

        let network = visLib.Network.Create(container, graph, options)

        onHover
        |> Option.iter (fun f ->
            network.on
                (Vis.NetworkEvents.HoverNode,
                 (fun o ->
                     //log("hoverNode Event", o)
                     idToNode !!((o?node))
                     |> fun n -> n.Original |> f)))
        onSelect
        |> Option.iter (fun f ->
            network.on
                (Vis.NetworkEvents.SelectNode,
                 (fun o ->
                     //log("selectNode Event", o)
                     idToNode (!!(o?nodes) |> Array.head)
                     |> fun n -> n.Original |> f)))
        ()

let inline graph props = ofType<GraphView, _, _> props []
