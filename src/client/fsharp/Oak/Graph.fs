module FantomasTools.Client.OakViewer.Graph

open Fable.Core
open Fable.React

[<Erase>]
type NodeId = NodeId of int

[<Erase>]
type NodeLabel = NodeLabel of string

module VisReact =

    // vis-react component
    let inline graph nodes edges selectNodeCallback hoverNodeCallback : ReactElement =
        ofImport
            "default"
            "vis-react"
            {| graph =
                {| nodes =
                    nodes
                    |> Map.toArray
                    |> Array.map (fun (NodeId i, NodeLabel l) -> {| id = i; label = l |})
                   edges =
                    edges
                    |> Set.toArray
                    |> Array.map (fun (NodeId f, NodeId t) -> {| from = f; ``to`` = t |}) |}
               options =
                {| layout = {| hierarchical = true |}
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
