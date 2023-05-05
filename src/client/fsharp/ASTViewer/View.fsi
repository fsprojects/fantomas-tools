module FantomasTools.Client.ASTViewer.View

open Fable.React
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model

val cursorChanged: bubbleMsg: (BubbleMessage -> unit) -> model: Model -> e: obj -> unit
val commands: dispatch: (Msg -> unit) -> ReactElement
val settings: bubble: BubbleModel -> version: string -> dispatch: (Msg -> unit) -> ReactElement
val view: model: Model -> ReactElement
