module FantomasTools.Client.FantomasOnline.View

open System.Text.RegularExpressions
open Fable.React
open Fable.React.Props
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model
open Reactstrap

let private mapToOption dispatch (key, fantomasOption) =
    let editor =
        match fantomasOption with
        | FantomasOption.BoolOption (o, _, v) ->
            SettingControls.toggleButton (fun _ ->
                UpdateOption(key, BoolOption(o, key, true))
                |> dispatch) (fun _ ->
                UpdateOption(key, BoolOption(o, key, false))
                |> dispatch) "true" "false" key v

        | FantomasOption.IntOption (o, _, v) ->
            let onChange (nv: string) =
                if Regex.IsMatch(nv, "\\d+") then
                    let v = nv |> (int)
                    UpdateOption(key, IntOption(o, key, v))
                    |> dispatch

            SettingControls.input onChange key "integer" v

    div [ Key key
          ClassName "fantomas-setting" ] [
        editor
    ]

let options model dispatch =
    let optionList = Map.toList model.UserOptions |> List.sortBy fst

    optionList
    |> List.map (mapToOption dispatch)
    |> ofList

let githubIssueUri code (model: Model) =
    let location = Browser.Dom.window.location

    let config =
        model.UserOptions
        |> Map.toList
        |> List.map snd
        |> List.sortBy FantomasOnline.Shared.sortByOption

    let defaultValues =
        model.DefaultOptions
        |> List.sortBy FantomasOnline.Shared.sortByOption

    let options =
        Seq.zip config defaultValues
        |> Seq.toArray
        |> Seq.map (fun (userV, defV) ->

            sprintf (if userV <> defV then "| **`%s`** | **`%s`** |" else "| `%s` | `%s` |") (getOptionKey userV)
                (optionValue userV))
        |> String.concat "\n"

    let title = "<Insert meaningful title>"
    let label = "bug"

    let codeTemplate header code =
        sprintf """
#### %s

```fsharp
%s
```
            """ header code

    let (left, right) =
        match model.State with
        | FormatError e -> codeTemplate "Code" code, codeTemplate "Error" e
        | FormatResult result -> codeTemplate "Code" code, codeTemplate "Result" result
        | _ -> codeTemplate "Code" code, ""

    let body =
        (sprintf """
Issue created from [fantomas-online](%s)

**Please describe here fantomas problem you encountered**
%s
%s
#### Options

Fantomas %s

| Name | Value |
| ---- | ----- |
%s
        """ location.href left right model.Version options)
        |> System.Uri.EscapeDataString

    let uri =
        sprintf "https://github.com/fsprojects/fantomas/issues/new?title=%s&labels=%s&body=%s" title label body

    uri |> Href


let private createGitHubIssue code model =
    match model.Mode with
    | Preview when (not (System.String.IsNullOrWhiteSpace(code))) ->
        Button.button [ Button.Color Danger
                        Button.Outline true
                        Button.Custom [ githubIssueUri code model
                                        ClassName "rounded-0" ] ] [
            str "Looks wrong? Create an issue!"
        ]
    | _ ->
        span [ ClassName "text-muted mr-2" ] [
            str "Looks wrong? Try using the preview version!"
        ]

let view model =
    match model.State with
    | EditorState.LoadingFormatRequest
    | EditorState.LoadingOptions -> FantomasTools.Client.Loader.loader
    | EditorState.OptionsLoaded -> null
    | EditorState.FormatResult result ->
        div [ ClassName "tab-result" ] [
            Editor.editorInTab [ Editor.Value result
                                 Editor.IsReadOnly true ]
        ]

    | EditorState.FormatError error ->
        div [ ClassName "tab-result" ] [
            Editor.editorInTab [ Editor.Value error
                                 Editor.IsReadOnly true ]
        ]

let private userChangedSettings (model: Model) =
    model.SettingsChangedByTheUser
    |> List.isEmpty
    |> not

let commands code model dispatch =
    let formatButton =
        Button.button [ Button.Color Primary
                        Button.Custom [ OnClick(fun _ -> dispatch Msg.Format) ] ] [
            str "Format"
        ]

    let copySettingButton =
        if userChangedSettings model then
            Button.button [ Button.Color Secondary
                            Button.Custom [ ClassName "text-white"
                                            OnClick(fun _ -> dispatch CopySettings) ] ] [
                str "Copy settings"
            ]
            |> Some
        else
            None

    match model.State with
    | EditorState.LoadingOptions -> []
    | EditorState.LoadingFormatRequest ->
        [ formatButton
          ofOption copySettingButton ]
    | EditorState.OptionsLoaded
    | EditorState.FormatResult _
    | EditorState.FormatError _ ->
        [ createGitHubIssue code model
          formatButton
          ofOption copySettingButton ]
    |> fragment []

let settings model dispatch =
    match model.State with
    | EditorState.LoadingOptions -> Spinner.spinner [ Spinner.Color Primary ] []
    | _ ->
        let fantomasMode =
            [ FantomasMode.Previous, "2.9.1"
              FantomasMode.Latest, "Latest"
              FantomasMode.Preview, "Preview" ]
            |> List.map (fun (m, l) ->
                { IsActive = model.Mode = m
                  Label = l
                  OnClick = (fun _ -> ChangeMode m |> dispatch) }: SettingControls.MultiButtonSettings)
            |> SettingControls.multiButton "Mode"

        let fileExtension =
            SettingControls.toggleButton (fun _ -> SetFsiFile true |> dispatch) (fun _ -> SetFsiFile false |> dispatch)
                "*.fsi" "*.fs" "File extension" model.IsFsi

        let options = options model dispatch

        fragment [] [
            FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
            fantomasMode
            fileExtension
            hr []
            options
        ]
