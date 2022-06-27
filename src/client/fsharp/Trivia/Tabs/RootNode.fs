module FantomasTools.Client.Trivia.Tabs.RootNode

open Fable.Core
open Browser.Types
open Fable.React
open Fable.React.Props
open TriviaViewer.Shared
open FantomasTools.Client.Trivia.Model

let view (model: Model) dispatch =
    let rec printNode (node: TriviaNode) =
        let mapRange (r: Range) : FantomasTools.Client.Editor.HighLightRange = {
            StartLine = r.StartLine
            StartColumn = r.StartColumn
            EndLine = r.EndLine
            EndColumn = r.EndColumn
        }

        let rangeString =
            $"({node.Range.StartLine}, {node.Range.StartColumn} - {node.Range.EndLine}, {node.Range.EndColumn})"

        div [
            Class "trivia-node"
            Key $"{node.Type}_{rangeString}"
            OnClick(fun ev ->
                ev.stopPropagation ()
                let div = (ev.target :?> Element)
                div.classList.add "highlight"

                JS.setTimeout (fun () -> div.classList.remove "highlight") 400
                |> ignore

                HighLight(mapRange node.Range) |> dispatch)
        ] [
            str $"{node.Type} {rangeString}"
            ofArray (Array.map printNode node.Children)
        ]

    printNode model.RootNode
