module FantomasTools.Client.FantomasOnline.State

open Elmish
open FantomasTools.Client.FantomasOnline.Model

let init code =
    { IsFsi = false }, Cmd.none

let update msg model =
    model, Cmd.none