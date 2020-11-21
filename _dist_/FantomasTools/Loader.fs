module FantomasTools.Client.Loader

open Fable.React
open Fable.React.Props
open Reactstrap

let loader =
    div [ ClassName "loader" ] [
        div [ ClassName "inner" ] [
            Spinner.spinner [ Spinner.Color Primary ] []
        ]
    ]
