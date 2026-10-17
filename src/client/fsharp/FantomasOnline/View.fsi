module FantomasTools.Client.FantomasOnline.View

open Fable.React
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model

/// The id of the link that creates a GitHub issue, which Ctrl+I clicks while the tab shows it.
[<Literal>]
val IssueLinkId: string = "create-issue"

val commands: bubble: BubbleModel -> model: Model -> dispatch: (Msg -> unit) -> ReactElement
val settings: isFsi: bool -> model: Model -> dispatch: (Msg -> unit) -> ReactElement
val view: model: Model -> dispatch: (Msg -> unit) -> ReactElement
