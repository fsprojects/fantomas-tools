module FantomasTools.Client.OakViewer.Model

open Fable.Core
open FantomasTools.Client.Editor

type TriviaNode =
    { Type: string
      Range: HighLightRange
      Content: string option }

type OakNode =
    { Type: string
      Text: string option
      Range: HighLightRange
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
    | GetOak
    | OakReceived of OakNode
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
    { Oak: OakNode option
      Error: string option
      IsLoading: bool
      Defines: string
      Version: string
      IsStroustrup: bool
      IsGraphView: bool
      GraphViewOptions: GraphView.Options
      GraphViewRootNodes: GraphView.NodeId list }
