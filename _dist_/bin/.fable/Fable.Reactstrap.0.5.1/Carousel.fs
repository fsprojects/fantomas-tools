namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Carousel =

    type CarouselProps =
        | ActiveIndex of int
        | Next of (unit -> unit)
        | Previous of (unit -> unit)
        | Keyboard of bool
        | Pause of U2<string, bool>
        | Ride of string
        | Interval of U3<int, string, bool>
        | MouseEnter of (MouseEvent -> unit)
        | MouseLeave of (MouseEvent -> unit)
        | Slide of bool
        | CssModule of CSSModule
        | Custom of IHTMLProp list

    let carousel (props: CarouselProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Carousel" "reactstrap" props elems
