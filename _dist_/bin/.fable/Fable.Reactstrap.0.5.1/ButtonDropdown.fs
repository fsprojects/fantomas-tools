namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module ButtonDropdown =

    type ButtonDropdownProps =
        | Tag of U2<string, obj>
        | Disabled of bool
        | Direction of Common.Direction
        | Group of bool
        | IsOpen of bool
        | Toggle of (unit -> unit)
        | Custom of IHTMLProp list

    let buttonDropdown (props: ButtonDropdownProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "ButtonDropdown" "reactstrap" props elems
