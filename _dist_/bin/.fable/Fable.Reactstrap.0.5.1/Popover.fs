namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Popover =

    type PopoverProps =
        | Trigger of string
        | IsOpen of bool
        | Toggle of (unit -> unit)
        | BoundariesElement of U2<string, Element>
        | Target of U2<string, Element>
        | Container of U2<string, Element>
        | InnerClassName of string
        | Disabled of bool
        | HideArrow of bool
        | PlacementPrefix of string
        | Delay of Common.Delay
        | Placement of Common.Placement
        | Modifiers of obj
        | Offset of U2<string, int>
        | Fade of bool
        | Flip of bool
        | Custom of IHTMLProp list

    let popover (props: PopoverProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Popover" "reactstrap" props elems
