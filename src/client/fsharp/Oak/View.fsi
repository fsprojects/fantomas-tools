module FantomasTools.Client.OakViewer.View

open Fable.React
open FantomasTools.Client
open FantomasTools.Client.OakViewer.Model

val view: model: Model -> dispatch: (Msg -> unit) -> ReactElement
val commands: dispatch: (Msg -> unit) -> ReactElement
val settings: bubble: BubbleModel -> model: Model -> dispatch: (Msg -> unit) -> ReactElement
