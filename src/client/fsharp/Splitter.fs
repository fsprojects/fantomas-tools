module FantomasTools.Client.Splitter

open Browser.Types
open Fable.Core
open Fable.React
open Fable.React.Props
open Feliz
open FantomasTools.Client

/// How far the divider may travel, as a percentage of the row. Past either end one of the two panes
/// has nothing left to show, and the whole point of the screen is that you see both.
[<Literal>]
let private Minimum = 15.0

[<Literal>]
let private Maximum = 85.0

/// What the stylesheet falls back to, and where a double click puts the divider back.
[<Literal>]
let private Default = 40.0

/// A hand is not still, so a press is allowed a few pixels of travel before it counts as a drag.
[<Literal>]
let private Slack = 3.0

[<Emit("$0.style.setProperty($1, $2)")>]
let private setStyleProperty (element: HTMLElement) (name: string) (value: string) : unit = jsNative

[<Emit("$0.setPointerCapture($1)")>]
let private capturePointer (element: HTMLElement) (pointerId: float) : unit = jsNative

[<Emit("$0.hasPointerCapture($1)")>]
let private hasPointerCapture (element: HTMLElement) (pointerId: float) : bool = jsNative

/// The divider between the input pane and the tools pane, dragged to give one of them more room.
///
/// Where it stands is written to the row as a custom property rather than kept in the model: a drag
/// reports every pixel it passes, the model holds the source code, and a model update would put
/// both editors through a render for each of those pixels. Monaco is told nothing either, it is
/// mounted with `automaticLayout` and notices its own box changing.
[<ReactComponent>]
let Splitter () =
    let position = React.useRef Default
    /// Where the pointer went down, and whether it has travelled since. A double click resets the
    /// divider, and two drags in a row are a double click as far as the browser is concerned, so
    /// the reset has to be able to tell a click from a drag that ended where the last one did.
    let pressedAt = React.useRef 0.0
    let dragged = React.useRef false

    /// The row the divider sits in, which is the element the percentage applies to.
    let row (element: HTMLElement) : HTMLElement = element.parentElement

    let apply (element: HTMLElement) (percentage: float) =
        position.current <- percentage
        setStyleProperty (row element) "--split" $"%.2f{percentage}%%"
        element.setAttribute ("aria-valuenow", $"%.0f{percentage}")

    let clamp (percentage: float) = System.Math.Clamp(percentage, Minimum, Maximum)

    let onPointerDown (ev: PointerEvent) =
        let element = ev.currentTarget :?> HTMLElement
        pressedAt.current <- ev.clientX
        dragged.current <- false
        // The pointer keeps reporting to the divider once it is captured, so a fast drag that ends
        // up over an editor still moves the divider rather than landing in the editor.
        capturePointer element ev.pointerId
        element.classList.add Style.Dragging

    let onPointerMove (ev: PointerEvent) =
        let element = ev.currentTarget :?> HTMLElement

        if hasPointerCapture element ev.pointerId then
            if abs (ev.clientX - pressedAt.current) > Slack then
                dragged.current <- true

            let bounds = (row element).getBoundingClientRect()
            apply element (clamp ((ev.clientX - bounds.left) / bounds.width * 100.0))

    let onPointerUp (ev: PointerEvent) =
        let element = ev.currentTarget :?> HTMLElement
        element.classList.remove Style.Dragging

    let onDoubleClick (ev: MouseEvent) =
        // Adjusting the divider twice in a row arrives here as a double click. Undoing both drags
        // is the opposite of what was asked for, so only a double click that stayed put resets.
        if not dragged.current then
            apply (ev.currentTarget :?> HTMLElement) Default

    let onKeyDown (ev: KeyboardEvent) =
        let step =
            match ev.key with
            | "ArrowLeft" -> Some -2.0
            | "ArrowRight" -> Some 2.0
            | "Home" -> Some(Default - position.current)
            | _ -> None

        match step with
        | None -> ()
        | Some step ->
            ev.preventDefault ()
            apply (ev.currentTarget :?> HTMLElement) (clamp (position.current + step))

    div
        [
            Id "splitter"
            Role "separator"
            TabIndex 0
            Title "Drag to resize. Double click to reset."
            HTMLAttr.Custom("aria-orientation", "vertical")
            HTMLAttr.Custom("aria-label", "Resize the input pane")
            HTMLAttr.Custom("aria-valuemin", Minimum)
            HTMLAttr.Custom("aria-valuemax", Maximum)
            HTMLAttr.Custom("aria-valuenow", Default)
            OnPointerDown onPointerDown
            OnPointerMove onPointerMove
            OnPointerUp onPointerUp
            OnDoubleClick onDoubleClick
            OnKeyDown onKeyDown
        ]
        [ i [ ClassName "fa-solid fa-grip-lines-vertical" ] [] ]
