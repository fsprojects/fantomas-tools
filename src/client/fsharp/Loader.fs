module FantomasTools.Client.Loader

open Fable.React
open Fable.React.Props
open FantomasTools.Client

let loading = div [ Id "loading" ] [ div [] [] ]

let tabLoading = div [ ClassName Style.TabContent ] [ loading ]
