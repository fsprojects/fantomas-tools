module FantomasTools.Client.OakViewer.Graph

open Browser.Types
open Fable.Core
open Fable.React
open Fable.React.Props
open Feliz

module VisNetwork =
    type node =
        {| id: U2<int, string>
           label: string
           level: int
           color: string
           shape: string
           value: int |}

    type edge =
        {| from: U2<int, string>
           ``to``: U2<int, string>
           dashes: bool |}

    type data =
        {| nodes: node array
           edges: edge array |}

    type options =
        {| layout: obj
           interaction: obj
           width: int
           height: int |}

    [<Import("Network", "vis-network/peer")>]
    type Network(_container: Element, _data: obj, _options: obj) =
        inherit System.Object()

        [<Emit("$0.on('selectNode', $1)")>]
        member this.OnSelect _callback : unit = jsNative

        [<Emit("$0.on('hoverNode', $1)")>]
        member this.OnHover _callback : unit = jsNative

        [<Emit("$0.setData($1)")>]
        member this.SetData(_data: data) = jsNative

        [<Emit("$0.setOptions($1)")>]
        member this.SetOptions(_options: options) = jsNative

type GraphProps =
    {| options: VisNetwork.options
       data: VisNetwork.data
       selectNode: {| nodes: int array |} -> unit
       hoverNode: {| node: U2<int, string> |} -> unit |}

[<ReactComponent>]
let Graph (props: GraphProps) =
    let divRef = React.useRef None
    let mutable network = None

    React.useEffectOnce (fun () ->
        divRef.current
        |> Option.iter (fun divRef ->
            let network' = VisNetwork.Network(divRef, props.data, props.options)
            network'.OnSelect(props.selectNode)
            network'.OnHover(props.hoverNode)
            network <- Some network'))

    React.useEffect (
        fun () -> network |> Option.iter (fun network -> network.SetData props.data)
        , [| box props.data |]
    )

    React.useEffect (
        fun () -> network |> Option.iter (fun network -> network.SetOptions props.options)
        , [| box props.options |]
    )

    div [ RefValue divRef ] [ str "graph, never render" ]

// module VisReact =
//
//     // vis-react component
//     let inline graph
//         (graphOptions: Options)
//         (parentElementId: string)
//         nodes
//         edges
//         selectNodeCallback
//         hoverNodeCallback
//         : ReactElement
//         =
//         let layout =
//             let hier =
//                 {| enabled = true
//                    direction = "UD"
//                    levelSeparation = 75 |}
//
//             match graphOptions.Layout with
//             | TopDown -> {| hierarchical = hier |}
//             | LeftRight -> {| hierarchical = {| hier with direction = "LR" |} |}
//             | Free -> {| hierarchical = {| hier with enabled = false |} |}
//
//         let scalingLabel =
//             let opt =
//                 {| enabled = true
//                    max = graphOptions.ScaleMaxSize |}
//
//             match graphOptions.Scale with
//             | NoScale -> {| opt with enabled = false |}
//             | SubTreeNodes
//             | AllNodes -> opt
//
//         let parentElement = Browser.Dom.document.getElementById parentElementId
//
//         ofImport
//             "default"
//             "vis-react"
//             {| graph =
//                 {| nodes =
//                     nodes
//                     |> Map.toArray
//                     |> Array.map
//                         (fun
//                             (NodeId i,
//                              { Label = NodeLabel l
//                                Level = level
//                                Color = NodeColor c
//                                Shape = s
//                                ScaleValue = v }) ->
//                             {| id = i
//                                label = l
//                                level = level
//                                color = c
//                                shape = (string s).ToLower()
//                                value = v |})
//                    edges =
//                     edges
//                     |> Set.toArray
//                     |> Array.map
//                         (fun
//                             { From = NodeId f
//                               To = NodeId t
//                               Dashed = d } -> {| from = f; ``to`` = t; dashes = d |}) |}
//                options =
//                 {| layout = layout
//                    interaction = {| hover = true |}
//                    nodes = {| scaling = {| label = scalingLabel |} |}
//                    width = string parentElement.clientWidth
//                    height = string parentElement.clientHeight |}
//                events =
//                 {| selectNode = fun (ev: {| nodes: int[] |}) -> selectNodeCallback (NodeId ev.nodes[0])
//                    hoverNode = fun (ev: {| node: int |}) -> hoverNodeCallback (NodeId ev.node) |}
//                style = {| |}
//                getNetwork = fun _ -> ()
//                getNodes = fun _ -> ()
//                getEdges = fun _ -> ()
//                vis = fun _ -> () |}
//             []
