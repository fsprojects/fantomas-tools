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
            SettingControls.toggleButton
                (fun _ ->
                    UpdateOption(key, BoolOption(o, key, true))
                    |> dispatch)
                (fun _ ->
                    UpdateOption(key, BoolOption(o, key, false))
                    |> dispatch)
                "true"
                "false"
                key
                v

        | FantomasOption.IntOption (o, _, v) ->
            let onChange (nv: string) =
                if Regex.IsMatch(nv, "\\d+") then
                    let v = nv |> (int)

                    UpdateOption(key, IntOption(o, key, v))
                    |> dispatch

            SettingControls.input key onChange key "integer" v
        | FantomasOption.MultilineFormatterTypeOption (o, _, v) ->
            SettingControls.toggleButton
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "character_width"))
                    |> dispatch)
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "number_of_items"))
                    |> dispatch)
                "CharacterWidth"
                "NumberOfItems"
                key
                (v = "character_width")
        | FantomasOption.EndOfLineStyleOption (o, _, v) ->
            SettingControls.toggleButton
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "crlr"))
                    |> dispatch)
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "lf"))
                    |> dispatch)
                "CRLF"
                "LF"
                key
                (v = "crlf")


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
        |> List.sortBy sortByOption

    let defaultValues = model.DefaultOptions |> List.sortBy sortByOption

    let options =
        let changedOptions =
            Seq.zip config defaultValues
            |> Seq.toArray
            |> Seq.choose (fun (userV, defV) -> if userV <> defV then Some userV else None)
            |> Seq.toList

        if List.isEmpty changedOptions then
            "Default Fantomas configuration"
        else
            changedOptions
            |> Seq.map (fun opt -> sprintf "                %s = %s" (getOptionKey opt) (optionValue opt))
            |> String.concat "\n"
            |> sprintf
                """```fsharp
    { config with
%s }
```"""

    let title = "<Insert meaningful title>"
    let label = "bug"

    let codeTemplate header code =
        sprintf
            """
#### %s

```fsharp
%s
```
            """
            header
            code

    let (left, right) =
        match model.State with
        | FormatError e -> codeTemplate "Code" code, codeTemplate "Error" e
        | FormatResult result -> codeTemplate "Code" code, codeTemplate "Result" result
        | _ -> codeTemplate "Code" code, ""

    let body =
        (sprintf
            """
<!--

    Please only use this to create issues.
    If you wish to suggest a feature,
    please fill in the feature request template at https://github.com/fsprojects/fantomas/issues/new/choose

-->
Issue created from [fantomas-online](%s)

%s
%s
#### Problem description

Please describe here the Fantomas problem you encountered.
Check out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/master/CONTRIBUTING.md#bug-reports).

#### Extra information

- [ ] The formatted result breaks by code.
- [ ] The formatted result gives compiler warnings.
- [ ] I or my company would be willing to help fix this.

#### Options

Fantomas %s

%s
        """
            location.href
            left
            right
            model.Version
            options)
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
    | EditorState.LoadingOptions -> Loader.loader
    | EditorState.OptionsLoaded -> null
    | EditorState.FormatResult result ->
        div [ ClassName "tab-result fantomas-result" ] [
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
            [ FantomasMode.V2, "2.x"
              FantomasMode.V3, "3.x"
              FantomasMode.V4, "4.x"
              FantomasMode.Preview, "Preview" ]
            |> List.map
                (fun (m, l) ->
                    { IsActive = model.Mode = m
                      Label = l
                      OnClick = (fun _ -> ChangeMode m |> dispatch) }: SettingControls.MultiButtonSettings)
            |> SettingControls.multiButton "Mode"

        let fileExtension =
            SettingControls.toggleButton
                (fun _ -> SetFsiFile true |> dispatch)
                (fun _ -> SetFsiFile false |> dispatch)
                "*.fsi"
                "*.fs"
                "File extension"
                model.IsFsi

        let options = options model dispatch

        fragment [] [
            VersionBar.versionBar (sprintf "Version: %s" model.Version)
            fantomasMode
            fileExtension
            hr []
            options
        ]
