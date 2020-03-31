module FantomasTools.Client.ASTViewer.Model

open ASTViewer.Shared

module Graph =
    type Layout =
        | HierarchicalUpDown
        | HierarchicalLeftRight
        | Free

    type Options =
        { MaxNodes: int
          MaxNodesInRow: int
          Layout: Layout }

    type Model =
        { RootsPath: Node list
          Options: Options }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type GraphMsg =
    | SetRoot of Node
    | RootBack
    | SetOptions of Graph.Options

type Msg =
    | VersionFound of string
    | SetSourceText of string
    | DoParse
    | DoTypeCheck
    | Parsed of Dto
    | TypeChecked of Dto
    | Error of string
    | ShowJsonViewer
    | ShowEditor
    | ShowRaw
    | ShowGraph
    | Graph of GraphMsg
    | DefinesUpdated of string
    | SetFsiFile of bool

type EditorState =
    | Loading
    | Loaded

type View =
    | Editor
    | JsonViewer
    | Raw
    | Graph

type Model =
    { Source: string
      Defines: string
      IsFsi: bool
      Parsed: Result<Dto option, string>
      IsLoading: bool
      Version: string
      View: View
      FSharpEditorState: EditorState
      Graph: Graph.Model }
