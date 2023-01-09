module FantomasTools.Client.OakViewer.Model

open Fable.Core

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
          Color: NodeColor
          Shape: NodeShape
          ScaleValue: int }

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
    | GetOak
    | OakReceived of string
    | DefinesUpdated of string
    | FSCVersionReceived of string
    | SetFsiFile of bool
    | SetStroustrup of bool
    | SetGraphView of bool
    | SetGraphViewNodeLimit of int
    | SetGraphViewLayout of GraphView.Layout
    | SetGraphViewScale of GraphView.Scale
    | SetGraphViewScaleMax of int
    | GraphViewSetRoot of GraphView.NodeId
    | GraphViewGoBack
    | Error of string
    | HighLight of FantomasTools.Client.Editor.HighLightRange

type Model =
    { Oak: string
      Error: string option
      IsLoading: bool
      Defines: string
      Version: string
      IsStroustrup: bool
      IsGraphView: bool
      GraphViewOptions: GraphView.Options
      GraphViewRootNodes: GraphView.NodeId list }
