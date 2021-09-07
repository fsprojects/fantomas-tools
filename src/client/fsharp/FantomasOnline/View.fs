module FantomasTools.Client.FantomasOnline.View

open System.Text.RegularExpressions
open Fable.React
open Fable.React.Props
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.Editor
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
                    UpdateOption(key, EndOfLineStyleOption(o, key, "crlf"))
                    |> dispatch)
                (fun _ ->
                    UpdateOption(key, EndOfLineStyleOption(o, key, "lf"))
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

type GithubIssue =
    { BeforeHeader: string
      BeforeContent: string
      AfterHeader: string
      AfterContent: string
      Description: string
      Title: string
      DefaultOptions: FantomasOption list
      UserOptions: Map<string, FantomasOption>
      Version: string
      IsFsi: bool }

let githubIssueUri (githubIssue: GithubIssue) =
    let location = Browser.Dom.window.location

    let config =
        githubIssue.UserOptions
        |> Map.toList
        |> List.map snd
        |> List.sortBy sortByOption

    let defaultValues =
        githubIssue.DefaultOptions
        |> List.sortBy sortByOption

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
        codeTemplate githubIssue.BeforeHeader githubIssue.BeforeContent,
        codeTemplate githubIssue.AfterHeader githubIssue.AfterContent


    let fileType =
        if githubIssue.IsFsi then
            "\n*Signature file*"
        else
            System.String.Empty

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

%s

#### Extra information

- [ ] The formatted result breaks by code.
- [ ] The formatted result gives compiler warnings.
- [ ] I or my company would be willing to help fix this.

#### Options

Fantomas %s

%s
%s

<sub>Did you know that you can ignore files when formatting from fantomas-tool or the FAKE targets by using a [.fantomasignore file](https://github.com/fsprojects/fantomas/blob/master/docs/Documentation.md#ignore-files-fantomasignore)?</sub>
        """
            location.href
            left
            right
            githubIssue.Description
            githubIssue.Version
            options
            fileType)
        |> System.Uri.EscapeDataString

    let uri =
        sprintf "https://github.com/fsprojects/fantomas/issues/new?title=%s&body=%s" githubIssue.Title body

    uri |> Href


let private createGitHubIssue code model =
    let description =
        """Please describe here the Fantomas problem you encountered.
                    Check out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/master/CONTRIBUTING.md#bug-reports)."""

    let bh, bc, ah, ac =
        match model.State with
        | FormatError e -> "Code", code, "Error", e
        | FormatResult result -> "Code", code, "Result", (Option.defaultValue result.FirstFormat result.SecondFormat)
        | _ -> "Code", code, "", ""

    match model.Mode with
    | Preview when (not (System.String.IsNullOrWhiteSpace(code))) ->
        let githubIssue =
            { BeforeHeader = bh
              BeforeContent = bc
              AfterHeader = ah
              AfterContent = ac
              Description = description
              Title = "<Insert meaningful title>"
              DefaultOptions = model.DefaultOptions
              UserOptions = model.UserOptions
              Version = model.Version
              IsFsi = model.IsFsi }

        Button.button [ Button.Color Danger
                        Button.Outline true
                        Button.Custom [ githubIssueUri githubIssue
                                        Target "_blank"
                                        ClassName "rounded-0" ] ] [
            str "Looks wrong? Create an issue!"
        ]
    | _ ->
        span [ ClassName "text-muted mr-2" ] [
            str "Looks wrong? Try using the preview version!"
        ]

let private viewErrors (model: Model) result isIdempotent errors =
    let errors =
        match errors with
        | [] -> []
        | errors ->
            let badgeColor (e: FantomasOnline.Shared.ASTError) =
                match e.Severity with
                | ASTErrorSeverity.Error -> Danger
                | ASTErrorSeverity.Warning -> Warning
                | _ -> Info

            errors
            |> List.mapi (fun i e ->
                li [ Key(sprintf "ast-error-%i" i) ] [
                    strong [] [
                        str (
                            sprintf "(%i,%i) (%i, %i)" e.Range.StartLine e.Range.StartCol e.Range.EndLine e.Range.EndCol
                        )
                    ]
                    Badge.badge [ Badge.Color(badgeColor e) ] [
                        str (e.Severity.ToString())
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

    let idempotency =
        if isIdempotent then
            None
        else
            let githubIssue =
                { BeforeHeader = "Formatted code"
                  BeforeContent = result.FirstFormat
                  AfterHeader = "Reformatted code"
                  AfterContent = Option.defaultValue result.FirstFormat result.SecondFormat
                  Description = "Fantomas was not able to produce the same code after reformatting the result."
                  Title = "Idempotency problem when <add use-case>"
                  DefaultOptions = model.DefaultOptions
                  UserOptions = model.UserOptions
                  Version = model.Version
                  IsFsi = model.IsFsi }

            div [ ClassName "idempotent-error" ] [
                h6 [] [
                    str "The result was not idempotent"
                ]
                str "Fantomas was able to format the code, but when formatting the result again, the code changed."
                br []
                str "The result after the first format is being displayed."
                br []
                Button.button [ Button.Color Danger
                                Button.Custom [ githubIssueUri githubIssue
                                                Target "_blank"
                                                ClassName "rounded-0" ] ] [
                    str "Report idempotancy issue"
                ]
            ]
            |> Some

    if not isIdempotent || not (List.isEmpty errors) then
        ul [ Id "ast-errors"; ClassName "" ] [
            ofOption idempotency
            ofList errors
        ]
        |> Some
    else
        None

let view model =
    match model.State with
    | EditorState.LoadingFormatRequest
    | EditorState.LoadingOptions -> Loader.loader
    | EditorState.OptionsLoaded -> null
    | EditorState.FormatResult result ->
        let formattedCode, isIdempotent, astErrors =
            match result.SecondFormat with
            | Some sf when sf = result.FirstFormat -> sf, true, result.SecondValidation
            | Some _ -> result.FirstFormat, false, result.FirstValidation
            | None -> result.FirstFormat, true, result.FirstValidation

        div [ ClassName "tab-result fantomas-result" ] [
            div [ ClassName "fantomas-editor-container" ] [
                Editor true [ MonacoEditorProp.DefaultValue formattedCode ]
            ]
            ofOption (viewErrors model result isIdempotent astErrors)
        ]

    | EditorState.FormatError error ->
        div [ ClassName "tab-result" ] [
            Editor true [ MonacoEditorProp.DefaultValue error ]
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
            |> List.map (fun (m, l) ->
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
