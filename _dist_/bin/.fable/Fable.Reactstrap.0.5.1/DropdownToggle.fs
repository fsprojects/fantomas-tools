namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module DropdownToggle =

    type DropdownToggleProps =
        | Caret of bool
        | Color of Common.Color
        | [<CompiledName("data-toggle")>] DataToggle of string
        | [<CompiledName("aria-haspopup")>] AriaHasPopup of bool
        | Size of Common.Size
        | Custom of IHTMLProp list

    let dropdownToggle (props: DropdownToggleProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "DropdownToggle" "reactstrap" props elems
