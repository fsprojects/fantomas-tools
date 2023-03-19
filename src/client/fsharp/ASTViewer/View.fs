module FantomasTools.Client.ASTViewer.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client.Editor

let private cursorChanged dispatch (e: obj) =
    let lineNumber: int = e?position?lineNumber
    let column: int = e?position?column
    dispatch (HighLight(lineNumber, column))

let private results dispatch model =
    let result =
        match model.Parsed with
        | Some(Ok parsed) -> EditorAux (cursorChanged dispatch) true [ MonacoEditorProp.DefaultValue parsed.String ]
        | Some(Result.Error errors) -> ReadOnlyEditor [ MonacoEditorProp.DefaultValue errors ]
        | None -> str ""

    let astErrors =
        model.Parsed
        |> Option.bind (fun parsed ->
            match parsed with
            | Ok parsed when (not (Seq.isEmpty parsed.Errors)) ->
                let badgeColor (e: ASTViewer.Shared.ASTError) =
                    if e.Severity = "warning" then
                        Style.TextBgWarning
                    else
                        Style.TextBgDanger

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
                            span [ ClassName $"{Style.Badge} {badgeColor}" ] [ str e.Severity ]
                            span [ ClassName $"{Style.Badge} {Style.TextBgDark}"; Title "ErrorNumber" ] [
                                ofInt e.ErrorNumber
                            ]
                            span [ ClassName $"{Style.Badge} {Style.TextBgLight}"; Title "SubCategory" ] [
                                str e.SubCategory
                            ]
                            p [] [ str e.Message ]
                        ])

                ul [ Id "ast-errors"; ClassName "" ] [ ofArray errors ] |> Some
            | _ -> None)
        |> ofOption

    div [ Id "ast-content" ] [ div [ ClassName Style.AstEditorContainer ] [ result ]; astErrors ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        results dispatch model

let commands dispatch =
    button [
        ClassName $"{Style.Btn} {Style.BtnPrimary}"
        OnClick(fun _ -> dispatch DoParse)
    ] [ str "Show Untyped AST" ]

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
