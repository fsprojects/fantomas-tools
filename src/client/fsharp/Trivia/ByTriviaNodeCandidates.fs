module FantomasTools.Client.Trivia.ByTriviaNodeCandidates

open FantomasTools.Client.Trivia.Model
open Fable.React
open Fable.React.Props
open TriviaViewer.Shared

let view (model: Model) dispatch =
    let nodes =
        model.TriviaNodeCandidates
        |> List.mapi (fun idx node ->
            let mapRange (r: Range) : FantomasTools.Client.Editor.HighLightRange =
                { StartLine = r.StartLine
                  StartColumn = r.StartColumn
                  EndLine = r.EndLine
                  EndColumn = r.EndColumn }

            tr [ Key(sprintf "node_%d" idx)
                 ClassName($"trivia-candidate-{node.Type} pointer")
                 OnClick(fun _ -> HighLight(mapRange node.Range) |> dispatch) ] [
                td [] [ str node.Name ]
                td [ ClassName "text-center" ] [
                    ofInt node.Range.StartLine
                ]
                td [ ClassName "text-center" ] [
                    ofInt node.Range.StartColumn
                ]
                td [ ClassName "text-center" ] [
                    ofInt node.Range.EndLine
                ]
                td [ ClassName "text-center" ] [
                    ofInt node.Range.EndColumn
                ]
            ])

    div [] [
        table [ ClassName "table table-bordered" ] [
            thead [] [
                tr [] [
                    th [] [ str "Name" ]
                    th [ ClassName "text-center" ] [
                        str "StartLine"
                    ]
                    th [ ClassName "text-center" ] [
                        str "StartCol"
                    ]
                    th [ ClassName "text-center" ] [
                        str "EndLine"
                    ]
                    th [ ClassName "text-center" ] [
                        str "EndCol"
                    ]
                ]
            ]
            tbody [] [ ofList nodes ]
        ]
    ]
