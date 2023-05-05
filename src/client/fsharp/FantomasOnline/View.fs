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
            let mkButton (value: string) =
                let label =
                    let capital = System.Char.ToUpper value.[0]
                    $"{capital}{value.[1..]}".Replace("_", " ")

                let activeBtnClass =
                    if v <> value then
                        Style.BtnOutlineSecondary
                    else
                        $"{Style.BtnSecondary} {Style.TextWhite}"

                button [
                    ClassName $"{Style.Btn} {activeBtnClass}"
                    Key value
                    OnClick(fun _ -> UpdateOption(key, MultilineBracketStyleOption(o, key, value)) |> dispatch)
                ] [ str label ]

            div [ ClassName Style.Mb3 ] [
                Standard.label [ ClassName Style.FormLabel ] [ label ]
                br []
                div [ ClassName $"{Style.BtnGroup}" ] [
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

#### Options

Fantomas %s

%s
%s

<sub>Did you know that you can ignore files when formatting from fantomas-tool or the FAKE targets by using a [.fantomasignore file](https://fsprojects.github.io/fantomas/docs/end-users/IgnoreFiles.html)?</sub>
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
        span [ ClassName $"{Style.TextMuted} {Style.Me2}" ] [ str "Looks wrong? Try using the main version!" ]
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
        | _ -> span [ ClassName $"{Style.TextMuted} {Style.Me2}" ] [ str "Looks wrong? Try using the main version!" ]

let viewErrors (model: Model) isFsi result isIdempotent errors =
    let errors =
        match errors with
        | [] -> []
        | errors ->
            let badgeColor (e: Diagnostic) = e.Severity

            errors
            |> List.mapi (fun i e ->
                li [ Key(sprintf "ast-error-%i" i) ] [
                    strong [] [
                        str (
                            sprintf
                                "(%i,%i) (%i, %i)"
                                e.Range.StartLine
                                e.Range.StartColumn
                                e.Range.EndLine
                                e.Range.EndColumn
                        )
                    ]
                    span [ ClassName $"{Style.Badge} {badgeColor}" ] [ str (e.Severity.ToString()) ]
                    span [ ClassName $"{Style.Badge} {Style.TextBgDark}"; Title "ErrorNumber" ] [ ofInt e.ErrorNumber ]
                    span [ ClassName $"{Style.Badge} {Style.TextBgLight}"; Title "SubCategory" ] [ str e.SubCategory ]
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
                  IsFsi = isFsi }

            div [ ClassName Style.IdempotentError ] [
                h6 [] [ str "The result was not idempotent" ]
                str "Fantomas was able to format the code, but when formatting the result again, the code changed."
                br []
                str "The result after the first format is being displayed."
                br []
                a [
                    ClassName $"{Style.Btn} {Style.BtnDanger}"
                    githubIssueUri githubIssue
                    Target "_blank"
                ] [ str "Report idempotancy issue" ]
            ]
            |> Some

    if not isIdempotent || not (List.isEmpty errors) then
        ul [ Id "ast-errors"; ClassName "" ] [ ofOption idempotency; ofList errors ]
        |> Some
    else
        None

// TODO: idempotency case
// Sample link: https://fsprojects.github.io/fantomas-tools/#/fantomas/main?data=N4KABGBEDGD2AmBTSAuKBbBBXANosAKogM4AuAOgE7kB2VtepY0liAhqYgVgA55gAKNmjKUAljQDmASkEAjEaXFTZAXjBsANGDn061GozAA3Njiz5VeiBHQdoACzDlIoiZJdgA7mNIPrNgA%2BzpCm5oieALQAfCEAjC7aLgBMLgEQwQD6YDHxcfkFhUXFJaVl5RX5iSHJtXX1DY1NTWkGNmCBgbEs7JzcfMia4FDEiKSk7sSoYADawxCg7RCQACSkAJ48yGiQEqSQQ0tQK2EW03NHEAAMh5eQAJI0SDSkAMpiAF6D80sALD8QAC6PwAvrcFgDjhsttNdi8DpDVqdtrNIRA4uClpAALJsAAeABkJIgCYgpH4EZcwHFkldIcD2mCfossWtNijIGT4AB5ABmRJoiFeGzwlNZyPOaLAyUx7UgAFEnnyBd8qZAcLzIPTQZiWXK2TCdnJYLAcGL9RK0BdLgBmWU2B40UaUUgAMQkZgAcogvDhiealkoLNrGbrEQaOcbTQHliczGcrVLfvblq8eGxoIgAEKIXmwVgABTYlDY6DGiEoMZsQcQIZsTPaeodEdhUbN9qR8ZR1qOAFYU1A0xns7n8yTYF4K9A2KNHsZYNOJrAaFWIDW6xAGzYm7HoZGTe3w5bUVSAGwDyBDzM5vOsACqPC2lGns5o88XYmXq7AvLMow3YBbhCRyrHurYHlWnbhJKVIAOwXleI63ogADCOAzsQKHLqIWDQKQ%2Bbfr%2BOD-kcDL1mGIEtkaEEdnG0GJlSAAcCHpteo6sNiiDoHIFaEX%2BtakTqzLhmB1HRrRx49ksACcLHDjeY5YTgX4DkRJFLGRm4UayolQG2kF0QmJ6XHENxSperGIAAgrynCUFh6B2N%2B66CaGwmUbpkD6RJXYwSZGLmYhCmsK8XFiHAykrqp-EAUBYA7lC7LgeJR6%2BQxJkyoFlk2XZoXoOFpoqVKLkaUJjYiUlYmHpRklSnEdpZcOVmULAWBPAAIogfr5XZzmUMGrnke5OmVVAewGbVVJxMm5m4ni9y8gQDhkq8Dj5qQADqYjwBSA50oNWnDfqnnjT59HGUccT9rN%2BILUtZLysRQprS6W07Q436nvtpVueVHmjXC%2BxnUZUntHE543fNNC8mIeLck%2BHD5vKeI8KwxDEJ%2BUVSox33tJpgHacdAOnal52gzYcTwZDABKiBwJQ8BvbtSa4zY%2BNxQloHE-CwPdnVzE03T%2BbwJ6WDcRWfL3Jw6BTAOcSxYTzaeeguATH6gquvmdjjBWBBJbzfmXbJ5m0-T8DYqrYjq4gmuUNrvUXo4xYZnZmQ%2BO9WoHQTR1K9zQOkyDUq0hec3NSW6zcpQRJkEzH0DjjCs%2B7ufsTWlF1LMkAVqqHlDh5H0ekKL4uUJL0uy3Vid-SNhoYJb1u2-buv6wHfNUrUF5h2wEdR2IZAWzgavEg3HAO%2BZTslnhFZu9tFKV9uFU14Dqdk0HDXZ-iABqXZZhI8DuLH34J17HMLxyJM1Wn5MQMkM3r3irptXhmM708%2B8z3HLNz8B1dnzzLeGxna6d92qwFIAAcTGMjVGJAMbLgPvHVmQIyrz3%2Bovby-90pHGSBDNU3pfTEhzKQScZI9ZbE6jDGgvhMZWSeJxYu5cqQlTxsg7%2BRM0E0QwendoyQqZqisn6SQNAH40Cfsud4giOBYFYAQWAjxnikA4JjPiT0v7xVPslaq4pL5BwFnwgeFYaCKOMOOKQdCeKUHIRIKh2FlHqWYb9FBP8NHL0Dm3Y22dLYqgJKWOQ8A2BoVgBjKQeDra2IEj9IaVc2H7hShfFeVIbRmTVFAisYgywvDMAAaUQIgHgciySkEeFmEsIiP5UjUuE%2BxkTHHROcQbTBSwbRZzuFmdCNAADWKpiDNVak8b0ZBEDmzrsSKBaNYFOj6gNCJh0om%2B3YbErR8TbSZTVFmYswVEDtV7iwNJHpODwDvJQ5cnVoDoRLEuLG5SYrH0VsnReKsB5WwIRPdpYxhTrFFPUrhNgbRrzuP3QegpikZleW8EUqo7gsFLFseAntpne1mXc3%2B-s4muNtLfO42TclzSLuYvkrS2AdK6d%2BUyiCwDs1uYleZmiLTaISUAu4ISCHsUQACp5goHI8CwAoi5oyYFKIHEwtmLC1GoJiTS5sk1bQ4LuCk8Q6SFE4EevlYgZTLgVNisMQEtxdjEFdBjaYGqQRAA

// let view isFsi model =
//     match model.State with
//     | EditorState.LoadingFormatRequest
//     | EditorState.LoadingOptions -> Loader.tabLoading
//     | EditorState.OptionsLoaded -> null
//     | EditorState.FormatResult result ->
//         let formattedCode, isIdempotent, astErrors =
//             match result.SecondFormat with
//             | Some sf when sf = result.FirstFormat -> sf, true, result.SecondValidation
//             | Some _ -> result.FirstFormat, false, result.FirstValidation
//             | None -> result.FirstFormat, true, result.FirstValidation
//
//         div [ ClassName Style.TabContent ] [
//             div [ Id Style.FantomasEditorContainer ] [
//                 ReadOnlyEditor [
//                     MonacoEditorProp.Value formattedCode
//                     MonacoEditorProp.Options(MonacoEditorProp.rulerOption model.MaxLineLength)
//                 ]
//             ]
//             ofOption (viewErrors model isFsi result isIdempotent astErrors)
//         ]
//
//     | EditorState.FormatError error ->
//         div [ ClassName Style.TabContent ] [ ReadOnlyEditor [ MonacoEditorProp.Value error ] ]

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

    match model.State with
    | FantomasTabState.LoadingOptions -> []
    | FantomasTabState.LoadingFormatRequest -> [ formatButton; ofOption copySettingButton ]
    | FantomasTabState.OptionsLoaded
    | FantomasTabState.FormatResult _
    | FantomasTabState.FormatError _ -> [ createGitHubIssue bubble model; formatButton; ofOption copySettingButton ]
    |> fragment []

let settings isFsi model dispatch =
    match model.State with
    | FantomasTabState.LoadingOptions -> span [ ClassName $"{Style.SpinnerBorder} {Style.TextPrimary}" ] []
    | _ ->
        let fantomasMode =
            [ FantomasMode.V4, "4.x"
              FantomasMode.V5, "5.x"
              FantomasMode.V6, "6.x"
              FantomasMode.Main, "Main"
              FantomasMode.Preview, "6.1 preview" ]
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
            div [ ClassName $"{Style.My3} {Style.BorderBottom}" ] [
                div [] [
                    label [ ClassName Style.DBlock ] [
                        strong [ ClassName $"{Style.H4} {Style.TextCenter} {Style.DBlock} {Style.Mb2}" ] [
                            str "Filter settings"
                        ]
                    ]
                    input [
                        Type "search"
                        ClassName Style.FormControl
                        DefaultValue model.SettingsFilter
                        Placeholder "Filter settings"
                        OnChange(fun (ev: Browser.Types.Event) -> ev.Value |> UpdateSettingsFilter |> dispatch)
                    ]
                ]
            ]

        fragment [] [
            VersionBar.versionBar (sprintf "Version: %s" model.Version)
            fantomasMode
            fileExtension
            hr []
            searchBox
            options
        ]

let view (model: Model) (dispatch: Msg -> unit) =
    match model.State with
    | FantomasTabState.LoadingOptions
    | FantomasTabState.LoadingFormatRequest -> Loader.tabLoading
    | _ -> null
