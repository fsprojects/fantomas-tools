module FantomasTools.Client.ASTViewer.State

open Elmish
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client

val init: isActive: bool -> Model * Cmd<Msg>
val update: bubble: BubbleModel -> msg: Msg -> model: Model -> Model * Cmd<Msg>
