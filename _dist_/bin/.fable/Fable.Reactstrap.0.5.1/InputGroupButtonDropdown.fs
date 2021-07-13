namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module InputGroupButtonDropdown =

    type InputGroupButtonDropdownProps =
        | AddonType of Common.AddonType
        | GroupClassName of string
        | GroupAttributes of obj
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list

    let inputGroupButtonDropdown (props: InputGroupButtonDropdownProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "InputGroupButtonDropdown" "reactstrap" props elems
