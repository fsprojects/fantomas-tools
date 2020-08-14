module FantomasTools.Client.Trivia.ByTriviaNodeCandidates

open FantomasTools.Client.Trivia.Model
open Fable.React
open Fable.React.Props
open TriviaViewer.Shared

let view (model: Model) _dispatch =
    let nodes =
        model.TriviaNodeCandidates
        |> List.mapi (fun idx node ->
            tr [ Key(sprintf "node_%d" idx)
                 ClassName(sprintf "trivia-candidate-%s" node.Type) ] [
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

    div [ ClassName "d-flex h-100" ] [
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
