module FantomasTools.Client.VersionBar

open Fable.React
open Fable.React.Props
open FantomasTools.Client

let versionBar version =
    div [ ClassName Style.VersionBar ] [ str version ]
