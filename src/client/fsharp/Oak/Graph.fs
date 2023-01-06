module FantomasTools.Client.OakViewer.Graph

open Fable.Core
open Fable.React
open FantomasTools.Client.OakViewer.Model
open FantomasTools.Client.OakViewer.Model.GraphView

module VisReact =

    // vis-react component
    let inline graph (graphOptions: Options) nodes edges selectNodeCallback hoverNodeCallback : ReactElement =
        let layout =
            match graphOptions.Layout with
            | TopDown -> {| hierarchical = {| enabled = true; direction = "UD" |} |}
            | LeftRight -> {| hierarchical = {| enabled = true; direction = "LR" |} |}
            | Free -> {| hierarchical = {| enabled = false; direction = "UD" |} |}

        Fable.Core.JS.console.log (Map.toArray nodes)

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
                               Color = NodeColor c
                               Shape = s }) ->
                            {| id = i
                               label = l
                               color = c
                               shape = (string s).ToLower() |})
                   edges =
                    edges
                    |> Set.toArray
                    |> Array.map (fun (NodeId f, NodeId t) -> {| from = f; ``to`` = t |}) |}
               options =
                {| layout = layout
                   interaction = {| hover = true |}
                   width = "1000"
                   height = "700" |}
               events =
                {| selectNode = fun (ev: {| nodes: int[] |}) -> selectNodeCallback (NodeId ev.nodes[0])
                   hoverNode = fun (ev: {| node: int |}) -> hoverNodeCallback (NodeId ev.node) |}
               style = {| |}
               getNetwork = fun _ -> ()
               getNodes = fun _ -> ()
               getEdges = fun _ -> ()
               vis = fun _ -> () |}
            []
