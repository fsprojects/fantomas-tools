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
        | FantomasOption.BoolOption (_, v) ->
            let buttonProps v =
                let className =
                    if v then "rounded-0 text-white" else "rounded-0 hover-white"
                    |> ClassName

                let onClick _ = UpdateOption(key, BoolOption(key, not v)) |> dispatch
                Button.Custom
                    [ className
                      OnClick onClick ]
            ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0 w-25" ] ]
                [ Button.button
                    [ buttonProps v
                      Button.Outline(not v)
                      Button.Size Sm ] [ str "True" ]
                  Button.button
                      [ buttonProps (not v)
                        Button.Outline v
                        Button.Size Sm ] [ str "False" ] ]
        | FantomasOption.IntOption (_, v) ->
            InputGroup.inputGroup
                [ InputGroup.Size Sm
                  InputGroup.Custom [ ClassName "w-25 d-inline-block" ] ]
                [ Input.input
                    [ Input.Custom
                        [ Type "number"
                          ClassName "rounded-0 text-center"
                          Min "0"
                          DefaultValue v
                          Step "1" ] ] ]

    div
        [ Key key
          ClassName "flex-1 px-2" ]
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

let fantomasModeBar model dispatch =
    let buttonProps mode =
        let className =
            if mode = model.Mode then
                "rounded-0 text-white"
            else
                "rounded-0 hover-white"

        let custom =
            Button.Custom [
                ClassName className
                OnClick (fun _ -> ChangeMode mode |> dispatch)
            ]

        [ custom; Button.Outline (mode <> model.Mode) ]

    ButtonGroup.buttonGroup [
        ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0" ]
    ] [
        Button.button
            (buttonProps FantomasMode.Previous)
            [ str "Fantomas 2.9.1" ]
        Button.button
            (buttonProps FantomasMode.Latest)
            [ str "Latest on NuGet" ]
        Button.button
            (buttonProps FantomasMode.Preview)
            [ str "Preview (master branch)" ]
    ]

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        fragment []
            [ div [ ClassName "tab-result" ]
                  [ ofOption
                      (Option.map (fun result ->
                          Editor.editorInTab
                              [ Editor.Value result
                                Editor.IsReadOnly true ]) model.Result) ]
              FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
              options model dispatch
              fantomasModeBar model dispatch
              Button.button
                  [ Button.Color Primary
                    Button.Custom
                        [ OnClick(fun _ -> dispatch Msg.Format)
                          ClassName "rounded-0 w-100" ] ] [ str "Format" ] ]
