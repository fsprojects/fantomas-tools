module FantomasTools.Client.OakViewer.Model

open Fable.Core
open FantomasTools.Client

type TriviaNode =
    {
        Type: string
        Range: Range
        Content: string option
    }

type OakNode =
    {
        Type: string
        Text: string option
        Range: Range
        ContentBefore: TriviaNode array
        Children: OakNode array
        ContentAfter: TriviaNode array
    }

module GraphView =
    [<Erase; Struct>]
    type NodeId = NodeId of int

    [<Erase; Struct>]
    type NodeLabel = NodeLabel of string

    [<Erase; Struct>]
    type NodeColor = NodeColor of string

    [<Erase; Struct>]
    type NodeShape =
        | Ellipse
        | Box

    type Node =
        {
            Label: NodeLabel
            Level: int
            Color: NodeColor
            Shape: NodeShape
            ScaleValue: int
        }

    type Edge =
        {
            From: NodeId
            To: NodeId
            Dashed: bool
        }

    [<Struct>]
    type Layout =
        | TopDown
        | LeftRight
        | Free

    [<Struct>]
    type Scale =
        | NoScale
        | SubTreeNodes
        | AllNodes

    type Options =
        {
            NodeLimit: int
            Layout: Layout
            Scale: Scale
            ScaleMaxSize: int
        }

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
    | Failed of FormatError

[<RequireQualifiedAccess>]
type OakViewerTabState =
    | Loading
    | Result of OakNode
    | Failed of FormatError

type Model =
    {
        State: OakViewerTabState
        Version: string
        IsGraphView: bool
        GraphViewOptions: GraphView.Options
        GraphViewRootNodes: GraphView.NodeId list
    }
