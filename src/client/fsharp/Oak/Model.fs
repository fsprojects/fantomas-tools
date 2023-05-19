﻿module FantomasTools.Client.OakViewer.Model

open Fable.Core
open FantomasTools.Client

type TriviaNode =
    { Type: string
      Range: Range
      Content: string option }

type OakNode =
    { Type: string
      Text: string option
      Range: Range
      ContentBefore: TriviaNode array
      Children: OakNode array
      ContentAfter: TriviaNode array }

module GraphView =
    [<Erase>]
    type NodeId = NodeId of int

    [<Erase>]
    type NodeLabel = NodeLabel of string

    [<Erase>]
    type NodeColor = NodeColor of string

    [<Erase>]
    type NodeShape =
        | Ellipse
        | Box

    type Node =
        { Label: NodeLabel
          Level: int
          Color: NodeColor
          Shape: NodeShape
          ScaleValue: int }

    type Edge =
        { From: NodeId
          To: NodeId
          Dashed: bool }

    type Layout =
        | TopDown
        | LeftRight
        | Free

    type Scale =
        | NoScale
        | SubTreeNodes
        | AllNodes

    type Options =
        { NodeLimit: int
          Layout: Layout
          Scale: Scale
          ScaleMaxSize: int }

type Msg =
    | Bubble of BubbleMessage
    | GetOak
    | OakReceived of oak: OakNode * diagnostics: Diagnostic array
    | FSCVersionReceived of string
    | SetGraphView of bool
    | SetGraphViewNodeLimit of int
    | SetGraphViewLayout of GraphView.Layout
    | SetGraphViewScale of GraphView.Scale
    | SetGraphViewScaleMax of int
    | GraphViewSetRoot of GraphView.NodeId
    | GraphViewGoBack
    | Error of string

[<RequireQualifiedAccess>]
type OakViewerTabState =
    | Loading
    | Result of OakNode
    | Error of string

type Model =
    { State: OakViewerTabState
      Version: string
      IsGraphView: bool
      GraphViewOptions: GraphView.Options
      GraphViewRootNodes: GraphView.NodeId list }
