module FantomasTools.Client.FantomasOnline.View

open System.Text.RegularExpressions
open Fable.React
open Fable.React.Props
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model

let mapToOption dispatch (key, fantomasOption) =
    let editor =
        let label =
            a [
                Href
                    $"https://fsprojects.github.io/fantomas/docs/end-users/Configuration.html#%s{toEditorConfigName key}"
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
                    |> dispatch
                )
                (fun _ ->
                    UpdateOption(key, MultilineFormatterTypeOption(o, key, "number_of_items"))
                    |> dispatch
                )
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
                    $"%c{capital}%s{value.[1..]}".Replace("_", " ")

                {
                    Label = label
                    OnClick = (fun _ -> UpdateOption(key, MultilineBracketStyleOption(o, key, value)) |> dispatch)
                    IsActive = v = value
                }

            SettingControls.multiButton key [ mkButton "cramped"; mkButton "aligned"; mkButton "stroustrup" ]

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
                    model.SettingsFilter.Replace("fsharp_", "").Replace("_", "").Replace(" ", "").ToLowerInvariant()

                optionList
                |> List.filter (fun (n, _) ->
                    let setting = n.ToLowerInvariant()
                    setting.Contains(settingsFilter)
                )

    optionList |> List.map (mapToOption dispatch) |> ofList

type GithubIssue =
    {
        BeforeHeader: string
        BeforeContent: string
        AfterHeader: string
        AfterContent: string
        Description: string
        Title: string
        DefaultOptions: FantomasOption list
        UserOptions: Map<string, FantomasOption>
        Version: string
        IsFsi: bool
    }

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

/// Where an issue was found, said only when there is something to say: see `FormatIssue`.
let private underDefines (defines: string list) : string =
    match defines with
    | [] -> ""
    | defines ->
        let named = String.concat ", " defines
        $" with %s{named} defined"

/// An issue as a heading and the lines that explain it, in plain text: the tab shows them as they
/// are and the GitHub issue lists them.
let private describeIssue (issue: FormatIssue) : string * string list =
    let atLine (comment: SourceComment) =
        $"\"%s{comment.Text}\" at line %i{comment.Range.StartLine}"

    let changes (missing: string seq) (added: string seq) =
        [
            for m in missing do
                $"Lost: %s{m}"
            for a in added do
                $"Added: \"%s{a}\""
        ]

    match issue with
    | FormatIssue.MissingComment comment ->
        "Fantomas lost a comment", [ $"The result does not have the comment %s{atLine comment} of your code." ]
    | FormatIssue.NotValidFSharp(defines, _) ->
        "The result is not valid F#",
        [
            $"Fantomas cannot parse the code it produced%s{underDefines defines}. The diagnostics are about that code."
        ]
    | FormatIssue.CommentsChanged(defines, missing, added) ->
        $"The comments changed%s{underDefines defines}", changes (Array.map atLine missing) added
    | FormatIssue.DirectivesChanged(defines, missing, added) ->
        $"The directives changed%s{underDefines defines}", changes (Array.map (sprintf "\"%s\"") missing) added
    | FormatIssue.NotIdempotent _ ->
        "The result was not idempotent",
        [
            "Formatting the result again changed it. The result after the first format is shown."
        ]
    | FormatIssue.CheckFailed("Idempotency", message) -> "Formatting the result again failed", [ message ]
    | FormatIssue.CheckFailed("TriviaComparison", message) -> "Reading the comments of the result failed", [ message ]
    | FormatIssue.CheckFailed(check, message) -> $"The %s{check} check failed", [ message ]

/// The title a GitHub issue about this issue starts from.
let private issueTitle (issue: FormatIssue) : string =
    match issue with
    | FormatIssue.MissingComment _ -> "Comment lost when <add use-case>"
    | FormatIssue.NotValidFSharp _ -> "Invalid result when <add use-case>"
    | FormatIssue.CommentsChanged _ -> "Comments changed when <add use-case>"
    | FormatIssue.DirectivesChanged _ -> "Directives changed when <add use-case>"
    | FormatIssue.NotIdempotent _ -> "Idempotency problem when <add use-case>"
    | FormatIssue.CheckFailed _ -> "Formatting the result fails when <add use-case>"

/// Every issue as a markdown list, trimmed like `FormatError.toIssueText`: the GitHub issue travels
/// as a URL, and the code in it gives back whatever is left out here.
let private issuesAsMarkdown (issues: FormatIssue array) : string =
    let text =
        issues
        |> Array.map (fun issue ->
            let heading, lines = describeIssue issue
            let lines = lines |> List.map (sprintf "  - %s") |> String.concat "\n"
            $"- **%s{heading}**\n%s{lines}"
        )
        |> String.concat "\n"

    if text.Length <= 1000 then
        text
    else
        text.Substring(0, 1000) + "\n\n(truncated)"

[<Literal>]
let IssueLinkId = "create-issue"

let createGitHubIssue (bubble: BubbleModel) model =
    let description =
        """Please describe here the Fantomas problem you encountered.
                    Check out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/main/CONTRIBUTING.md#bug-reports)."""

    let bh, bc, ah, ac =
        match model.State with
        | FantomasTabState.FormatFailed error -> "Code", bubble.SourceCode, "Error", FormatError.toIssueText error
        // Not being idempotent is about the result and what became of it, not about the code.
        | FantomasTabState.FormatResult {
                                            Code = code
                                            Issues = [| FormatIssue.NotIdempotent formattedAgain |]
                                        } -> "Formatted code", code, "Reformatted code", formattedAgain
        | FantomasTabState.FormatResult result -> "Code", bubble.SourceCode, "Result", result.Code
        | _ -> "Code", bubble.SourceCode, "", ""

    // What Fantomas found wrong with its own result names the problem better than the user can be
    // asked to, so the issue starts from it.
    let issues =
        match model.State with
        | FantomasTabState.FormatResult result -> result.Issues
        | _ -> Array.empty

    let title, description =
        match Array.tryHead issues with
        | None -> "<Insert meaningful title>", description
        | Some first ->
            issueTitle first,
            $"Fantomas found this wrong with its result:\n\n%s{issuesAsMarkdown issues}\n\n%s{description}"

    // Source that does not parse is the one failure Fantomas is not to blame for. Inviting an issue
    // for it sends the user to the wrong place, so say what happened instead.
    let sourceIsTheProblem =
        match model.State with
        | FantomasTabState.FormatFailed error -> error.Kind = FormatErrorKind.InvalidSource
        | _ -> false

    if System.String.IsNullOrWhiteSpace(bubble.SourceCode) then
        span [ ClassName Style.Muted ] [ str "Looks wrong? Try using the main version!" ]
    elif sourceIsTheProblem then
        span [ ClassName Style.Muted ] [ str "This code is not valid F#, nothing to report." ]
    else
        match model.Mode with
        | Main
        | Preview ->
            let githubIssue =
                {
                    BeforeHeader = bh
                    BeforeContent = bc
                    AfterHeader = ah
                    AfterContent = ac
                    Description = description
                    Title = title
                    DefaultOptions = model.DefaultOptions
                    UserOptions = model.UserOptions
                    Version = model.Version
                    IsFsi = bubble.IsFsi
                }

            let link (style: string) (label: string) =
                a [
                    Id IssueLinkId
                    ClassName $"%s{Style.Btn} %s{style}"
                    githubIssueUri githubIssue
                    Target "_blank"
                    Keyboard.ctrlShortcut "I"
                ] [ str label; Keyboard.hint (Keyboard.ctrlKeys "I") ]

            if Array.isEmpty issues then
                link Style.Danger "Looks wrong? Create an issue!"
            else
                link Style.Warning "Report this problem!"
        | _ -> span [ ClassName Style.Muted ] [ str "Looks wrong? Try using the main version!" ]

let userChangedSettings (model: Model) =
    model.SettingsChangedByTheUser |> List.isEmpty |> not

let commands (bubble: BubbleModel) model dispatch =
    let formatButton =
        button [
            ClassName Style.Primary
            Keyboard.runShortcut
            OnClick(fun _ -> dispatch Msg.Format)
        ] [ str "Format"; Keyboard.hint (Keyboard.runKeys ()) ]

    let copySettingButton =
        if userChangedSettings model then
            button [ ClassName Style.Secondary; OnClick(fun _ -> dispatch CopySettings) ] [ str "Copy settings" ]
            |> Some
        else
            None

    match model.State with
    | FantomasTabState.LoadingOptions -> []
    | FantomasTabState.LoadingFormatRequest -> [ formatButton; ofOption copySettingButton ]
    | FantomasTabState.OptionsLoaded
    | FantomasTabState.FormatResult _
    | FantomasTabState.FormatFailed _ -> [ createGitHubIssue bubble model; formatButton; ofOption copySettingButton ]
    |> fragment []

let settings isFsi model dispatch =
    match model.State with
    | FantomasTabState.LoadingOptions -> Loader.loading
    | _ ->
        let fantomasMode =
            modes
            |> List.map (fun (m, l) ->
                {
                    IsActive = model.Mode = m
                    Label = l
                    OnClick = (fun _ -> ChangeMode m |> dispatch)
                }
                : SettingControls.MultiButtonSettings
            )
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

/// What Fantomas found wrong with its result, above the result.
let private issuesPanel (issues: FormatIssue array) =
    div [ Id "format-issues" ] [
        for idx, issue in Array.indexed issues do
            let heading, lines = describeIssue issue

            section [ Key(string<int> idx) ] [
                h4 [] [ str heading ]
                for line in lines do
                    p [] [ str line ]
            ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    match model.State with
    | FantomasTabState.LoadingOptions
    | FantomasTabState.LoadingFormatRequest -> Loader.tabLoading
    | FantomasTabState.FormatResult result when not (Array.isEmpty result.Issues) -> issuesPanel result.Issues
    | _ -> null
