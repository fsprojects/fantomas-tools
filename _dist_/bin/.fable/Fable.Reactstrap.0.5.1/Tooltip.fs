namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module Tooltip =
    type TooltipProps =
        | Trigger of string
        | BoundariesElement of U2<string, Element>
        | IsOpen of bool
        | HideArrow of bool
        | Toggle of (unit -> unit)
        | Target of U2<string, Element>
        | Container of U2<string, Element>
        | Delay of Common.Delay
        | InnerClassName of string
        | ArrowClassName of string
        | Autohide of bool
        | Placement of Common.Placement
        | Modifiers of obj
        | Offset of U2<string, int>
        | InnerRef of (Element -> unit)
        | Fade of bool
        | Flip of bool
        | Custom of IHTMLProp list

    let tooltip (props: TooltipProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Tooltip" "reactstrap" props elems
