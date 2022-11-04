module FantomasTools.Client.ASTViewer.View

open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client.Editor
open Reactstrap

let private results model =
    let result =
        match model.Parsed with
        | Some(Ok parsed) -> Editor true [ MonacoEditorProp.DefaultValue parsed.String ]
        | Some(Result.Error errors) -> Editor true [ MonacoEditorProp.DefaultValue errors ]
        | None -> str ""

    let astErrors =
        model.Parsed
        |> Option.bind (fun parsed ->
            match parsed with
            | Ok parsed when (not (Seq.isEmpty parsed.Errors)) ->
                let badgeColor (e: ASTViewer.Shared.ASTError) =
                    if e.Severity = "warning" then
                        Color.Warning
                    else
                        Color.Danger

                let errors =
                    parsed.Errors
                    |> Array.mapi (fun i e ->
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
                            Badge.badge [ Badge.Color(badgeColor e) ] [ str e.Severity ]
                            Badge.badge [ Badge.Color Color.Dark; Badge.Custom [ Title "ErrorNumber" ] ] [
                                ofInt e.ErrorNumber
                            ]
                            Badge.badge [ Badge.Color Color.Light; Badge.Custom [ Title "SubCategory" ] ] [
                                str e.SubCategory
                            ]
                            p [] [ str e.Message ]
                        ])

                ul [ Id "ast-errors"; ClassName "" ] [ ofArray errors ] |> Some
            | _ -> None)
        |> ofOption

    div [ Id "ast-content" ] [ div [ ClassName "ast-editor-container" ] [ result ]; astErrors ]

let view model _dispatch =
    if model.IsLoading then
        Loader.loader
    else
        results model

let commands dispatch =
    fragment [] [
        Button.button [ Button.Color Primary; Button.Custom [ OnClick(fun _ -> dispatch DoParse) ] ] [
            str "Show Untyped AST"
        ]
    ]

let settings isFsi model dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "ast-defines"
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
    ]
