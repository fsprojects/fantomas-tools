module FantomasTools.Client.FantomasOnline.View

open Fable.React
open Fable.React.Props
open FantomasTools.Client.FantomasOnline.Model

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        div [ClassName "tab-result"] [
            str "fantomas"
            FantomasTools.Client.VersionBar.versionBar (sprintf "Version: %s" model.Version)
        ]
