module FantomasTools.Client.VersionBar

open Fable.React
open Fable.React.Props

let versionBar version =
    div [ ClassName "version-bar" ] [
        str version
    ]
