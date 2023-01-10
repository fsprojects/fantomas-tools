module FantomasTools.Client.OakViewer.Graph

open Fable.Core
open Fable.React
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView

module VisReact =

    // vis-react component
    let inline graph
        (graphOptions: Options)
        (parentElementId: string)
        nodes
        edges
        selectNodeCallback
        hoverNodeCallback
        : ReactElement
        =
        let layout =
            let hier =
                {| enabled = true
                   direction = "UD"
                   levelSeparation = 75 |}

            match graphOptions.Layout with
            | TopDown -> {| hierarchical = hier |}
            | LeftRight -> {| hierarchical = {| hier with direction = "LR" |} |}
            | Free -> {| hierarchical = {| hier with enabled = false |} |}

        let scalingLabel =
            let opt =
                {| enabled = true
                   max = graphOptions.ScaleMaxSize |}

            match graphOptions.Scale with
            | NoScale -> {| opt with enabled = false |}
            | SubTreeNodes
            | AllNodes -> opt

        let parentElement = Browser.Dom.document.getElementById parentElementId

        ofImport
            "default"
            "vis-react"
            {| graph =
                {| nodes =
                    nodes
                    |> Map.toArray
                    |> Array.map
                        (fun
                            (NodeId i,
                             { Label = NodeLabel l
                               Level = level
                               Color = NodeColor c
                               Shape = s
                               ScaleValue = v }) ->
                            {| id = i
                               label = l
                               level = level
                               color = c
                               shape = (string s).ToLower()
                               value = v |})
                   edges =
                    edges
                    |> Set.toArray
                    |> Array.map
                        (fun
                            { From = NodeId f
                              To = NodeId t
                              Dashed = d } -> {| from = f; ``to`` = t; dashes = d |}) |}
               options =
                {| layout = layout
                   interaction = {| hover = true |}
                   nodes = {| scaling = {| label = scalingLabel |} |}
                   width = string parentElement.clientWidth
                   height = string parentElement.clientHeight |}
               events =
                {| selectNode = fun (ev: {| nodes: int[] |}) -> selectNodeCallback (NodeId ev.nodes[0])
                   hoverNode = fun (ev: {| node: int |}) -> hoverNodeCallback (NodeId ev.node) |}
               style = {| |}
               getNetwork = fun _ -> ()
               getNodes = fun _ -> ()
               getEdges = fun _ -> ()
               vis = fun _ -> () |}
            []
