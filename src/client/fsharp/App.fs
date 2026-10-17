module FantomasTools.Client.App

open Fable.Core.JsInterop
open Browser.Types
open Fable.React
open Feliz
open FantomasTools.Client.Routing
open Feliz.UseElmish
open Browser.Dom
open FantomasTools.Client

/// The letter of the key, in capitals, as the keyboard layout has it. Option on a Mac turns a letter
/// into another character, Option+A is å, and then only the physical key says which letter it was.
let private letterOf (ev: KeyboardEvent) : string =
    let key = ev.key.ToUpperInvariant()

    if key.Length = 1 && key >= "A" && key <= "Z" then
        key
    elif ev.code.StartsWith("Key", System.StringComparison.Ordinal) then
        ev.code.Substring 3
    else
        ""

[<ReactComponent>]
let App () =
    let model, dispatch =
        React.useElmish (State.init, State.update, dependencies = [||])

    let onUrlChanged url =
        let activeTab = Navigation.parseUrl url
        dispatch (Model.Msg.SelectTab activeTab)

    // Wherever the focus is, Ctrl+Enter, or Cmd+Enter on a Mac, does what the button of the active
    // tab does, Ctrl+I clicks the link that creates a GitHub issue, and Alt with a letter of
    // `Navigation.altShortcuts` does what goes with it. The listener runs before the key reaches the
    // page, and takes it: Monaco would insert a line for Ctrl+Enter, and the browser opens its menu
    // for Alt+F. Ctrl+I it takes only while the link is there, as Monaco suggests and Firefox shows
    // the page info for it. Alt with Ctrl is left alone, as that is AltGr on Windows, which types
    // characters.
    React.useEffect (
        (fun () ->
            let onKeyDown (ev: Event) =
                let ev = ev :?> KeyboardEvent

                let press (shortcut: Model.Shortcut) () = dispatch (Model.Msg.Pressed shortcut)
                let withCtrl = (ev.ctrlKey || ev.metaKey) && not ev.altKey && not ev.shiftKey

                let shortcut: (unit -> unit) option =
                    if ev.key = "Enter" && (ev.ctrlKey || ev.metaKey) then
                        Some(press Model.Shortcut.Run)
                    elif withCtrl && letterOf ev = "I" then
                        document.getElementById FantomasOnline.View.IssueLinkId
                        |> Option.ofObj
                        |> Option.map (fun link -> fun () -> link.click ())
                    elif ev.altKey && not ev.ctrlKey && not ev.metaKey && not ev.shiftKey then
                        Navigation.altShortcuts
                        |> List.tryFind (fun (letter, _) -> letter = letterOf ev)
                        |> Option.map (snd >> press)
                    else
                        None

                match shortcut with
                | None -> ()
                | Some run ->
                    ev.preventDefault ()
                    ev.stopPropagation ()

                    if not ev.repeat then
                        run ()

            window.addEventListener ("keydown", onKeyDown, true)

            { new System.IDisposable with
                member _.Dispose() =
                    window.removeEventListener ("keydown", onKeyDown, true)
            }
        ),
        [| box dispatch |]
    )

    let routes = View.rightPane model dispatch

    fragment [] [
        View.navigation dispatch
        SkillBanner.SkillBanner()
        main [] [
            View.editor model dispatch
            Splitter.Splitter()
            RouteListener onUrlChanged routes
        ]
    ]

let createRoot: Element -> {| render: ReactElement -> unit |} =
    import "createRoot" "react-dom/client"

let root = createRoot (document.getElementById "app")
root.render (React.StrictMode [ App() ])
