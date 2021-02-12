module FantomasTools.Client.ASTViewer.View

open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client.Editor
open Reactstrap

let private isEditorView =
    function
    | Editor -> true
    | _ -> false

let private isRawView =
    function
    | Raw -> true
    | _ -> false

let private results model dispatch =
    let result =
        match model.Parsed with
        | Some (Ok parsed) ->
            match model.View with
            | Raw ->
                editorInTab [ EditorProp.Language "fsharp"
                              EditorProp.IsReadOnly true
                              EditorProp.Value parsed.String ]
            | Editor ->
                editorInTab [ EditorProp.Language "fsharp"
                              EditorProp.IsReadOnly true
                              EditorProp.Value(Fable.Core.JS.JSON.stringify (parsed.Node, space = 4)) ]
        | Some (Result.Error errors) ->
            editorInTab [ EditorProp.Language "fsharp"
                          EditorProp.IsReadOnly true
                          EditorProp.Value errors ]
        | None -> str ""

    let astErrors =
        model.Parsed
        |> Option.bind
            (fun parsed ->
                match parsed with
                | Ok (parsed) when (not (Seq.isEmpty parsed.Errors)) ->
                    let badgeColor (e: ASTViewer.Shared.ASTError) =
                        if e.Severity = "warning" then
                            Color.Warning
                        else
                            Color.Danger

                    let errors =
                        parsed.Errors
                        |> Array.mapi
                            (fun i e ->
                                li [ Key(sprintf "ast-error-%i" i) ] [
                                    strong [] [
                                        str (
                                            sprintf
                                                "(%i,%i) (%i, %i)"
                                                e.Range.StartLine
                                                e.Range.StartCol
                                                e.Range.EndLine
                                                e.Range.EndCol
                                        )
                                    ]
                                    Badge.badge [ Badge.Color(badgeColor e) ] [
                                        str e.Severity
                                    ]
                                    Badge.badge [ Badge.Color Color.Dark
                                                  Badge.Custom [ Title "ErrorNumber" ] ] [
                                        ofInt e.ErrorNumber
                                    ]
                                    Badge.badge [ Badge.Color Color.Light
                                                  Badge.Custom [ Title "SubCategory" ] ] [
                                        str e.SubCategory
                                    ]
                                    p [] [ str e.Message ]
                                ])

                    ul [ Id "ast-errors"; ClassName "" ] [
                        ofArray errors
                    ]
                    |> Some
                | _ -> None)
        |> ofOption

    div [ Id "ast-content" ] [
        div [ ClassName "ast-editor-container" ] [
            result
        ]
        astErrors
    ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        results model dispatch

let commands dispatch =
    fragment [] [
        Button.button [ Button.Color Primary
                        Button.Custom [ OnClick(fun _ -> dispatch DoParse) ] ] [
            str "Show Untyped AST"
        ]
        Button.button [ Button.Color Primary
                        Button.Custom [ OnClick(fun _ -> dispatch DoTypeCheck) ] ] [
            str "Show Typed AST"
        ]
    ]

let settings model dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "ast-defines"
            (DefinesUpdated >> dispatch)
            "Defines"
            "Enter your defines separated with a space"
            model.Defines
        SettingControls.toggleButton
            (fun _ -> dispatch (SetFsiFile true))
            (fun _ -> dispatch (SetFsiFile false))
            "*.fsi"
            "*.fs"
            "File extension"
            model.IsFsi
        SettingControls.multiButton
            "Mode"
            [ { IsActive = (isEditorView model.View)
                Label = "Editor"
                OnClick = (fun _ -> dispatch ShowEditor) }
              { IsActive = (isRawView model.View)
                Label = "Raw"
                OnClick = (fun _ -> dispatch ShowRaw) } ]
    ]
