module FantomasTools.Client.FantomasOnline.View

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
            let buttonProps v =
                let className =
                    if v then "rounded-0 text-white" else "rounded-0 hover-white"
                    |> ClassName

                let onClick _ =
                    UpdateOption(key, BoolOption(o, key, not v))
                    |> dispatch

                Button.Custom [ className; OnClick onClick ]

            ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0 w-25" ] ]
                [ Button.button
                    [ buttonProps v
                      Button.Outline(not v)
                      Button.Size Sm ] [ str "True" ]
                  Button.button
                      [ buttonProps (not v)
                        Button.Outline v
                        Button.Size Sm ] [ str "False" ] ]
        | FantomasOption.IntOption (o, _, v) ->
            let onChange (ev: Browser.Types.Event) =
                let v = ev.Value |> (int)
                UpdateOption(key, IntOption(o, key, v))
                |> dispatch

            InputGroup.inputGroup
                [ InputGroup.Size Sm
                  InputGroup.Custom [ ClassName "w-25 d-inline-block" ] ]
                [ Input.input
                    [ Input.Custom
                        [ Type "number"
                          ClassName "rounded-0 text-center"
                          Min "0"
                          DefaultValue v
                          OnChange onChange
                          Step "1" ] ] ]

    div [ Key key; ClassName "flex-1 px-2" ]
        [ label [ ClassName "w-75 m-0" ] [ str key ]
          editor ]

let options model dispatch =
    let optionList = Map.toList model.UserOptions

    List.chunkBySize 2 optionList
    |> List.mapi (fun idx group ->
        div
            [ ClassName "d-flex flex-row"
              Key(sprintf "option-row-%i" idx) ] [ ofList (List.map (mapToOption dispatch) group) ])
    |> ofList
    |> fun options -> div [Id "fantomas-options"] [ options ]

let fantomasModeBar model dispatch =
    let buttonProps mode =
        let className =
            if mode = model.Mode then "rounded-0 text-white" else "rounded-0 hover-white"

        let custom =
            Button.Custom
                [ ClassName className
                  OnClick(fun _ -> ChangeMode mode |> dispatch) ]

        [ custom
          Button.Outline(mode <> model.Mode) ]

    ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0" ] ]
        [ Button.button (buttonProps FantomasMode.Previous) [ str "Fantomas 2.9.1" ]
          Button.button (buttonProps FantomasMode.Latest) [ str "Latest on NuGet" ]
          Button.button (buttonProps FantomasMode.Preview) [ str "Preview (master branch)" ] ]

let fileExtension model dispatch =
    let toggleButton msg active label =
        let className =
            if active then "rounded-0 text-white" else "rounded-0"

        Button.button
            [ Button.Custom
                [ ClassName className
                  OnClick(fun _ -> dispatch msg) ]
              Button.Outline(not active) ] [ str label ]

    ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0" ] ]
        [ toggleButton (SetFsiFile false) (not model.IsFsi) "*.fs"
          toggleButton (SetFsiFile true) model.IsFsi "*.fsi" ]


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

    let title = "Bug report from fantomas-online"
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

    let code = left + "" + right

    let body =
        sprintf """
Issue created from [fantomas-online](%s)

Please describe here fantomas problem you encountered
%s
%s
#### Options

Fantomas %s

| Name | Value |
| ---- | ----- |
%s
        """ location.href left right model.Version options
        |> System.Uri.EscapeDataString

    let uri =
        sprintf "https://github.com/fsprojects/fantomas/issues/new?title=%s&labels=%s&body=%s" title label body

    uri |> Href


let private createGitHubIssue code model =
    match model.Mode with
    | Preview when (not (System.String.IsNullOrWhiteSpace(code))) ->
        Button.button
            [ Button.Color Danger
              Button.Outline true
              Button.Custom [ githubIssueUri code model; ClassName "rounded-0" ] ] [ str "Looks wrong? Create an issue!" ]
    | _ -> null

let view code model dispatch =
    let options =
        [ options model dispatch
          fileExtension model dispatch
          fantomasModeBar model dispatch ]

    let submitButton =
        Button.button
            [ Button.Color Primary
              Button.Custom
                  [ OnClick(fun _ -> dispatch Msg.Format)
                    ClassName "rounded-0 w-100" ] ] [ str "Format" ]

    match model.State with
    | EditorState.LoadingOptions -> FantomasTools.Client.Loader.loader

    | EditorState.OptionsLoaded ->
        fragment []
            [ div [ ClassName "tab-result" ] []
              createGitHubIssue code model
              FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
              yield! options
              submitButton ]

    | EditorState.LoadingFormatRequest ->
        fragment []
            [ div [ ClassName "tab-result" ] [ FantomasTools.Client.Loader.loader ]
              FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
              yield! options ]

    | EditorState.FormatResult result ->
        fragment []
            [ div [ ClassName "tab-result" ]
                  [ Editor.editorInTab
                      [ Editor.Value result
                        Editor.IsReadOnly true ] ]
              createGitHubIssue code model
              FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
              yield! options
              submitButton ]

    | EditorState.FormatError error ->
        fragment []
            [ div [ ClassName "tab-result" ]
                  [ Editor.editorInTab
                      [ Editor.Value error
                        Editor.IsReadOnly true ] ]
              createGitHubIssue code model
              FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
              yield! options
              submitButton ]
