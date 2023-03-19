module FantomasTools.Client.VersionBar

open Fable.React
open Fable.React.Props

let versionBar version =
    div [ ClassName Style.VersionBar ] [ str version ]
