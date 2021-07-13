namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Button =

    type ButtonProps =
        | Tag of U2<string, obj>
        | Color of Common.Color
        | Outline of bool
        | Active of bool
        | Block of bool
        | InnerRef of (Element -> unit)
        | Size of Common.Size
        | CSSModule of Common.CSSModule
        | Close of bool
        | Custom of IHTMLProp list

    let button (props: ButtonProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Button" "reactstrap" props elems
