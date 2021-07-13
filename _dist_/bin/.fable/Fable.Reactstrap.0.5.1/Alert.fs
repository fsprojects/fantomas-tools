namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module Alert =
    type AlertProps =
        | Tag of U2<string, obj>
        | CSSModule of Common.CSSModule
        | Color of Common.Color
        | Transition of TransitionProps
        | IsOpen of bool
        | Toggle of (unit -> unit)
        | Custom of IHTMLProp list

    let alert (props: AlertProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Alert" "reactstrap" props elems
