module FantomasTools.Client.Loader

open Fable.React
open Fable.React.Props

let loader =
    div [ ClassName "loader" ] [
        div [ ClassName "inner" ] [ div [ ClassName "spinner-border text-primary" ] [] ]
    ]
// div [ ClassName Style.Loader ] [
//     div [ ClassName Style.Inner ] [ div [ ClassName $"{Style.SpinnerBorder} {Style.TextPrimary}" ] [] ]
// ]
