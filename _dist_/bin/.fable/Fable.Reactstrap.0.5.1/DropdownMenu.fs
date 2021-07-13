namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module DropdownMenu =

    type DropdownMenuProps =
        | Tag of U2<string, obj>
        | Right of bool
        | Flip of bool
        | CssModule of Common.CSSModule
        | Modifiers of obj
        | Persist of bool
        | Custom of IHTMLProp list

    let dropdownMenu (props: DropdownMenuProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "DropdownMenu" "reactstrap" props elems
