/// How the keyboard shortcuts of App.fs are shown: as keycaps next to what they do, written the way
/// the platform writes them.
module FantomasTools.Client.Keyboard

open Fable.Core
open Fable.React
open Fable.React.Props
open FantomasTools.Client

[<Emit("navigator.userAgent")>]
let private userAgent: string = jsNative

let private isMac () : bool = userAgent.Contains "Mac"

/// A modifier and a key: on a Mac as its symbols side by side, elsewhere as their names with a
/// plus between them, which the stylesheet colours.
let private combination (modifier: string) (macModifier: string) (key: string) : ReactElement list =
    if isMac () then
        [ str $"%s{macModifier} %s{key}" ]
    else
        [ str modifier; span [ ClassName Style.KeycapPlus ] [ str "+" ]; str key ]

/// The keys of a shortcut that goes with Ctrl, or Cmd on a Mac.
let ctrlKeys (key: string) : ReactElement list = combination "Ctrl" "⌘" key

/// The keys of the shortcut that does what the button of the active tab does.
let runKeys () : ReactElement list = ctrlKeys "↵"

/// The keys of a shortcut that goes with Alt.
let altKeys (letter: string) : ReactElement list = combination "Alt" "⌥" letter

/// The keys of a shortcut that goes with Alt, as plain text, for a tooltip.
let altKeysText (letter: string) : string =
    if isMac () then
        $"⌥ %s{letter}"
    else
        $"Alt+%s{letter}"

/// A shortcut that goes with Ctrl, or Cmd on a Mac, for a screen reader.
let ctrlShortcut (key: string) : IHTMLProp =
    HTMLAttr.Custom("aria-keyshortcuts", $"Control+%s{key} Meta+%s{key}")

/// The shortcut of a run button, for a screen reader.
let runShortcut: IHTMLProp = ctrlShortcut "Enter"

/// A shortcut that goes with Alt, for a screen reader.
let altShortcut (letter: string) : IHTMLProp =
    HTMLAttr.Custom("aria-keyshortcuts", $"Alt+%s{letter}")

/// Keys shown as a keycap.
let keycap (keys: ReactElement list) : ReactElement = kbd [ ClassName Style.Keycap ] keys

/// Keys shown as a keycap on a control whose shortcut `aria-keyshortcuts` already names, so a
/// screen reader does not read them as part of its label.
let hint (keys: ReactElement list) : ReactElement =
    kbd [ ClassName Style.Keycap; HTMLAttr.Custom("aria-hidden", "true") ] keys
