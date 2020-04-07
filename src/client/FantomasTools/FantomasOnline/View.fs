module FantomasTools.Client.FantomasOnline.View

open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model
open Reactstrap

let options model dispatch =
    let groups =
        let length = List.length model.Options
        List.chunkBySize 2 model.Options
        |> List.map (fun group ->
            div [] []
        )

    div [] [str "options"]

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        div [ClassName "tab-result"] [
            ofOption (Option.map (fun result ->  Editor.editorInTab [ Editor.Value result; Editor.IsReadOnly true ] ) model.Result)
            FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
            Button.button [ Button.Color Primary; Button.Custom [ OnClick (fun _ -> dispatch Msg.Format) ] ] [ str "format!" ]
            options model dispatch
        ]
