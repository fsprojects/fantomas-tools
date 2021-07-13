namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module Collapse =
    type CollapseProps =
        | In of bool
        | BaseClass of string
        | BaseClassIn of string
        | CssModule of CSSModule
        | TransitionAppearTimeout of int
        | TransitionEnterTimeout of int
        | TransitionLeaveTimeout of int
        | TransitionAppear of bool
        | TransitionEnter of bool
        | TransitionLeave of bool
        | OnLeave of (unit -> unit)
        | OnEnter of (unit -> unit)
        | IsOpen of bool
        | InnerRef of (Element -> unit)
        | Navbar of bool
        | OnEntering of (unit -> unit)
        | OnEntered of (unit -> unit)
        | OnExiting of (unit -> unit)
        | OnExited of (unit -> unit)
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list

    let collapse (props: CollapseProps seq) (elems: ReactElement seq): ReactElement =
        let customProps =
            props
            |> Seq.collect (function
                | Custom props -> props
                | _ -> List.empty)
            |> keyValueList CaseRules.LowerFirst

        let typeProps =
            props
            |> Seq.choose (function
                | Custom _ -> None
                | prop -> Some prop)
            |> keyValueList CaseRules.LowerFirst

        let props =
            JS.Constructors.Object.assign (createEmpty, customProps, typeProps)

        ofImport "Collapse" "reactstrap" props elems
