module FantomasTools.Client.FantomasOnline.View

open System.Text.RegularExpressions
open Fable.React
open Fable.React.Props
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model

let mapToOption dispatch (model: Model) (key, fantomasOption) =
    let editor =
        let label =
            a [
                Href $"https://fsprojects.github.io/fantomas/docs/end-users/Configuration.html#{toEditorConfigName key}"
                Target "_blank"
            ] [ str key ]

        match fantomasOption with
        | FantomasOption.BoolOption(o, _, v) ->
            SettingControls.toggleButton
                (fun _ -> UpdateOption(key, BoolOption(o, key, true)) |> dispatch)
                (fun _ -> UpdateOption(key, BoolOption(o, key, false)) |> dispatch)
                "true"
                "false"
                label
                v

        | FantomasOption.IntOption(o, _, v) ->
            let onChange (nv: string) =
                if Regex.IsMatch(nv, "\\d+") then
                    let v = nv |> int

                    UpdateOption(key, IntOption(o, key, v)) |> dispatch

            SettingControls.input key onChange label "integer" v
        | FantomasOption.MultilineFormatterTypeOption(o, _, v) ->
            SettingControls.toggleButton
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "character_width"))
                    |> dispatch)
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "number_of_items"))
                    |> dispatch)
                "CharacterWidth"
                "NumberOfItems"
                label
                (v = "character_width")
        | FantomasOption.EndOfLineStyleOption(o, _, v) ->
            SettingControls.toggleButton
                (fun _ -> UpdateOption(key, EndOfLineStyleOption(o, key, "crlf")) |> dispatch)
                (fun _ -> UpdateOption(key, EndOfLineStyleOption(o, key, "lf")) |> dispatch)
                "CRLF"
                "LF"
                label
                (v = "crlf")
        | FantomasOption.MultilineBracketStyleOption(o, _, v) ->
            let mkButton (value: string) : SettingControls.MultiButtonSettings =
                let label =
                    let capital = System.Char.ToUpper value.[0]
                    $"{capital}{value.[1..]}".Replace("_", " ")

                { Label = label
                  OnClick = (fun _ -> UpdateOption(key, MultilineBracketStyleOption(o, key, value)) |> dispatch)
                  IsActive = v = value }

            SettingControls.multiButton key [
                yield mkButton "cramped"
                yield mkButton "aligned"
                if model.Mode = FantomasMode.V5 then
                    yield mkButton "experimental_stroustrup"
                if
                    model.Mode = FantomasMode.V6
                    || model.Mode = FantomasMode.Main
                    || model.Mode = FantomasMode.Preview
                then
                    yield mkButton "stroustrup"
            ]

    div [ Key key ] [ editor ]

let options model dispatch =
    let optionList =
        Map.toList model.UserOptions
        |> List.sortBy fst
        |> fun optionList ->
            if System.String.IsNullOrWhiteSpace model.SettingsFilter then
                optionList
            else
                let settingsFilter =
                    model.SettingsFilter
                        .Replace("fsharp_", "")
                        .Replace("_", "")
                        .Replace(" ", "")
                        .ToLowerInvariant()

                optionList
                |> List.filter (fun (n, _) ->
                    let setting = n.ToLowerInvariant()
                    setting.Contains(settingsFilter))

    optionList |> List.map (mapToOption dispatch model) |> ofList

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

    let defaultValues = githubIssue.DefaultOptions |> List.sortBy sortByOption

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

    let left, right =
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

- [ ] The formatted result breaks my code.
- [ ] The formatted result gives compiler warnings.
- [ ] I or my company would be willing to help fix this.
- [ ] I would like a release if this problem is solved.

#### Options

Fantomas %s

%s
%s

<sub>Did you know that you can ignore files when formatting by using a [.fantomasignore file](https://fsprojects.github.io/fantomas/docs/end-users/IgnoreFiles.html)?</sub>
<sub>PS: It's unlikely that someone else will solve your specific issue, as it's something that you have a personal stake in.</sub>
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

let createGitHubIssue (bubble: BubbleModel) model =
    let description =
        """Please describe here the Fantomas problem you encountered.
                    Check out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/main/CONTRIBUTING.md#bug-reports)."""

    let bh, bc, ah, ac =
        match model.State with
        | FantomasTabState.FormatError e -> "Code", bubble.SourceCode, "Error", e
        | FantomasTabState.FormatResult result ->
            "Code", bubble.SourceCode, "Result", (Option.defaultValue result.FirstFormat result.SecondFormat)
        | _ -> "Code", bubble.SourceCode, "", ""

    if System.String.IsNullOrWhiteSpace(bubble.SourceCode) then
        span [ ClassName Style.Muted ] [ str "Looks wrong? Try using the main version!" ]
    else
        match model.Mode with
        | Main
        | Preview ->
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
                  IsFsi = bubble.IsFsi }

            a [
                ClassName $"{Style.Btn} {Style.Danger}"
                githubIssueUri githubIssue
                Target "_blank"
            ] [ str "Looks wrong? Create an issue!" ]
        | _ -> span [ ClassName Style.Muted ] [ str "Looks wrong? Try using the main version!" ]

let createIdempotencyIssue isFsi (model: Model) firstFormat secondFormat =
    let githubIssue =
        { BeforeHeader = "Formatted code"
          BeforeContent = firstFormat
          AfterHeader = "Reformatted code"
          AfterContent = secondFormat
          Description = "Fantomas was not able to produce the same code after reformatting the result."
          Title = "Idempotency problem when <add use-case>"
          DefaultOptions = model.DefaultOptions
          UserOptions = model.UserOptions
          Version = model.Version
          IsFsi = isFsi }

    a [
        ClassName $"{Style.Btn} {Style.Warning}"
        githubIssueUri githubIssue
        Target "_blank"
    ] [ str "Report idempotency issue!" ]

let userChangedSettings (model: Model) =
    model.SettingsChangedByTheUser |> List.isEmpty |> not

let commands (bubble: BubbleModel) model dispatch =
    let formatButton =
        button [ ClassName Style.Primary; OnClick(fun _ -> dispatch Msg.Format) ] [ str "Format" ]

    let copySettingButton =
        if userChangedSettings model then
            button [ ClassName Style.Secondary; OnClick(fun _ -> dispatch CopySettings) ] [ str "Copy settings" ]
            |> Some
        else
            None

    let idempotencyButton model =
        match model.State with
        | FantomasTabState.FormatResult { FirstFormat = ff
                                          SecondFormat = Some sf } when ff <> sf ->
            [ createIdempotencyIssue bubble.IsFsi model ff sf ]
        | _ -> []

    match model.State with
    | FantomasTabState.LoadingOptions -> []
    | FantomasTabState.LoadingFormatRequest -> [ formatButton; ofOption copySettingButton ]
    | FantomasTabState.OptionsLoaded
    | FantomasTabState.FormatResult _
    | FantomasTabState.FormatError _ ->
        [ yield! idempotencyButton model
          createGitHubIssue bubble model
          formatButton
          ofOption copySettingButton ]
    |> fragment []

let settings isFsi model dispatch =
    match model.State with
    | FantomasTabState.LoadingOptions -> Loader.loading
    | _ ->
        let fantomasMode =
            [ FantomasMode.V4, "4.x"
              FantomasMode.V5, "5.x"
              FantomasMode.V6, "6.x"
              FantomasMode.Main, "Main"
              FantomasMode.Preview, "7.0 preview" ]
            |> List.map (fun (m, l) ->
                { IsActive = model.Mode = m
                  Label = l
                  OnClick = (fun _ -> ChangeMode m |> dispatch) }
                : SettingControls.MultiButtonSettings)
            |> SettingControls.multiButton "Mode"

        let fileExtension =
            SettingControls.toggleButton
                (fun _ -> BubbleMessage.SetFsi true |> Bubble |> dispatch)
                (fun _ -> BubbleMessage.SetFsi false |> Bubble |> dispatch)
                "*.fsi"
                "*.fs"
                (str "File extension")
                isFsi

        let options = options model dispatch

        let searchBox =
            div [ Id "filter-settings"; ClassName Style.Setting ] [
                label [] [ strong [] [ str "Filter settings" ] ]
                input [
                    Type "search"
                    DefaultValue model.SettingsFilter
                    Placeholder "Filter settings"
                    OnChange(fun (ev: Browser.Types.Event) -> ev.Value |> UpdateSettingsFilter |> dispatch)
                ]
            ]

        let resetSettings =
            div [ ClassName Style.ResetSettings ] [
                button [ ClassName Style.Secondary; OnClick(fun _ -> ResetSettings |> dispatch) ] [
                    str "Reset settings"
                ]
            ]

        fragment [] [
            VersionBar.versionBar (sprintf "Version: %s" model.Version)
            fantomasMode
            fileExtension
            hr []
            if userChangedSettings model then
                resetSettings
                hr []
            searchBox
            options
        ]

let idempotencyProblem =
    div [ Id "idempotent-message" ] [
        h4 [] [ str "The result was not idempotent" ]
        str "Fantomas was able to format the code, but when formatting the result again, the code changed."
        br []
        str "The result after the first format is being displayed."
        br []
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    match model.State with
    | FantomasTabState.LoadingOptions
    | FantomasTabState.LoadingFormatRequest -> Loader.tabLoading
    | FantomasTabState.FormatResult result ->
        match result.SecondFormat with
        | Some secondFormat when (result.FirstFormat <> secondFormat) -> idempotencyProblem
        | _ -> null
    | _ -> null
