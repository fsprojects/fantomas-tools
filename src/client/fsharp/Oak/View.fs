module FantomasTools.Client.OakViewer.View

open System
open Browser.Types
open Fable.Core
open FantomasTools.Client.Editor
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model
open Reactstrap

let private results (model: Model) dispatch =
    let lines =
        model.Oak.Split([| '\n' |])
        |> Array.mapi (fun idx line ->
            let className =
                let trimmedLine = line.TrimStart()

                [
                    if trimmedLine.StartsWith("//") || trimmedLine.StartsWith("(*") then
                        yield "comment"
                    if trimmedLine.StartsWith("Newline") then
                        yield "newline"
                    if trimmedLine.StartsWith("#") then
                        yield "directive"
                ]
                |> String.concat " "

            div [
                Key !!idx
                OnClick(fun ev ->
                    ev.stopPropagation ()
                    let div = (ev.target :?> Element)
                    div.classList.add "highlight"
                    JS.setTimeout (fun () -> div.classList.remove "highlight") 400 |> ignore

                    let coords =
                        line
                        |> Seq.filter (fun (c: char) -> Char.IsDigit c || c = '-' || c = ',')
                        |> Seq.skipWhile (fun (c: char) -> not (Char.IsDigit c))
                        |> fun chars ->
                            Seq.foldBack
                                (fun (c: char) (acc: string list) ->
                                    match acc with
                                    | [] -> acc
                                    | current :: rest ->
                                        if Char.IsDigit c then
                                            $"{c}{current}" :: rest
                                        else
                                            "" :: acc)
                                chars
                                [ "" ]
                        |> Seq.map int
                        |> Seq.toList

                    match coords with
                    | [ startLine; startColumn; endLine; endColumn ] ->
                        let highlightRange: HighLightRange = {
                            StartLine = startLine
                            StartColumn = startColumn
                            EndLine = endLine
                            EndColumn = endColumn
                        }

                        dispatch (HighLight highlightRange)
                    | _ -> JS.console.log $"Could not construct highligh range, got %A{coords}")
            ] [ pre [ ClassName className ] [ str line ] ])

    div [ Id "oakResult" ] [ ofArray lines ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        match model.Error with
        | None -> results model dispatch
        | Some errors -> Editor true [ MonacoEditorProp.DefaultValue errors ]

let commands dispatch =
    Button.button [
        Button.Color Primary
        Button.Custom [ ClassName "rounded-0"; OnClick(fun _ -> dispatch GetOak) ]
    ] [ i [ ClassName "fas fa-code mr-1" ] []; str "Get oak" ]

let settings isFsi (model: Model) dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "trivia-defines"
            (DefinesUpdated >> dispatch)
            (str "Defines")
            "Enter your defines separated with a space"
            model.Defines
        SettingControls.toggleButton
            (fun _ -> dispatch (SetFsiFile true))
            (fun _ -> dispatch (SetFsiFile false))
            "*.fsi"
            "*.fs"
            (str "File extension")
            isFsi
        SettingControls.toggleButton
            (fun _ -> dispatch (SetStroustrup true))
            (fun _ -> dispatch (SetStroustrup false))
            "Stroustrup enabled"
            "Stroustrup disabled"
            (str "Is stroustrup?")
            model.IsStroustrup
    ]
