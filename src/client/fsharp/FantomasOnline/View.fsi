module FantomasTools.Client.FantomasOnline.View

open Fable.React
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model

val commands: bubble: BubbleModel -> model: Model -> dispatch: (Msg -> unit) -> ReactElement
val settings: isFsi: bool -> model: Model -> dispatch: (Msg -> unit) -> ReactElement
val view: model: Model -> dispatch: (Msg -> unit) -> ReactElement
