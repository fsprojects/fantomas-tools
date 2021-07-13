namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module Badge =
    type BadgeProps =
        | Tag of U2<string, obj>
        | Color of Common.Color
        | Pill
        | CSSModule of Common.CSSModule
        | Transition of TransitionProps
        | Custom of IHTMLProp list

    let badge (props: BadgeProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Badge" "reactstrap" props elems
