namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Browser.Types
open Fable.React

[<RequireQualifiedAccess>]
module Fade =

    type TimeoutEx = { enter: int; exit: int }

    type FadeProps =
        | In of bool
        | MountOnEnter of bool
        | UnmountOnExit of bool
        | Appear of bool
        | Enter of bool
        | Exit of bool
        | Timeout of U2<int, TimeoutEx>
        | AddEndListener of (string -> Event -> unit)
        | OnEnter of (unit -> unit)
        | OnEntering of (unit -> unit)
        | OnEntered of (unit -> unit)
        | OnExit of (unit -> unit)
        | OnExiting of (unit -> unit)
        | OnExited of (unit -> unit)
        | BaseClass of string

    let fade (props: FadeProps seq) (elems: ReactElement seq): ReactElement =
        let fadeProps =
            props |> keyValueList CaseRules.LowerFirst

        ofImport "Fade" "reactstrap" fadeProps elems
