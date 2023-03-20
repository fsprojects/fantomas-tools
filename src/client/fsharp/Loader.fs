module FantomasTools.Client.Loader

open Fable.React
open Fable.React.Props
open FantomasTools.Client

let loader =
    div [ ClassName Style.Loader ] [
        div [ ClassName Style.Inner ] [ div [ ClassName $"{Style.SpinnerBorder} {Style.TextPrimary}" ] [] ]
    ]
