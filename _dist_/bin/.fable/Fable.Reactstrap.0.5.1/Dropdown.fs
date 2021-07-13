namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Dropdown =

    type DropdownProps =
        | Disabled of bool
        | Direction of Common.Direction
        | Group of bool
        | IsOpen of bool
        | Nav of bool
        | Active of bool
        | InNavbar of bool
        | Toggle of (unit -> unit)
        | SetActiveFromChild of bool
        | Custom of IHTMLProp list

    let dropdown (props: DropdownProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Dropdown" "reactstrap" props elems
