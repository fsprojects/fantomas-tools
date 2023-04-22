module FantomasTools.Client.OakViewer.Graph

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz

module VisNetwork =
    type node =
        {| id: int
           label: string
           title: string
           level: int
           color: string
           shape: string
           value: int
           font: obj |}

    type edge =
        {| from: int
           ``to``: int
           dashes: bool |}

    type options =
        {| layout: obj
           interaction: obj
           width: string
           height: string
           nodes: obj |}

    [<Import("DataSet", "vis-data/peer")>]
    type DataSet(_data: U2<node, edge> array) =
        class
        end

    type data = {| nodes: DataSet; edges: DataSet |}

    [<Import("Network", "vis-network/peer")>]
    type Network(_container: Element, _data: obj, _options: obj) =
        inherit System.Object()

        [<Emit("$0.on('selectNode', $1)")>]
        member this.OnSelect _callback : unit = jsNative

        [<Emit("$0.on('hoverNode', $1)")>]
        member this.OnHover _callback : unit = jsNative

        [<Emit("$0.off('selectNode')")>]
        member this.OffSelect() : unit = jsNative

        [<Emit("$0.off('hoverNode')")>]
        member this.OffHover() : unit = jsNative

        [<Emit("$0.setData($1)")>]
        member this.SetData(_data: data) = jsNative

        [<Emit("$0.setOptions($1)")>]
        member this.SetOptions(_options: options) = jsNative

type GraphProps =
    {| options: VisNetwork.options
       data: VisNetwork.data
       selectNode: {| nodes: int array |} -> unit
       hoverNode: {| node: int |} -> unit |}

type RefProp =
    | Ref of obj

    interface IHTMLProp

[<ReactComponent>]
let Graph (props: GraphProps) =
    let divRef = React.useRef null
    let network, setNetwork = React.useState None

    React.useEffectOnce (fun () ->
        if not (isNullOrUndefined divRef.current) then
            let network' = VisNetwork.Network(divRef.current, props.data, props.options)
            network'.OnSelect(props.selectNode)
            network'.OnHover(props.hoverNode)
            setNetwork (Some network')
            JS.console.log "Network created")

    React.useEffect (
        fun () ->
            JS.console.log ("Data changed", props.data, network)

            network
            |> Option.iter (fun network ->
                network.OffSelect()
                network.OffHover()
                network.SetData props.data
                network.OnSelect(props.selectNode)
                network.OnHover(props.hoverNode))
        , [| box network; box props.data |]
    )

    React.useEffect (
        fun () ->
            JS.console.log ("Options changed", props.options, network)
            network |> Option.iter (fun network -> network.SetOptions props.options)
        , [| box network; box props.options |]
    )

    div [ Ref divRef ] [ str "graph, never render" ]
