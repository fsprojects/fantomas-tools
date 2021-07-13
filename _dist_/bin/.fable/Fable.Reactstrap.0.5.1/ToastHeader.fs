namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module ToastHeader =

    type ToastHeaderProps =
        | Tag of U2<string, obj>
        | CssModule of CSSModule
        | WrapTag of string
        | Toggle of (unit -> unit)
        | Icon of U2<string, ReactElement>
        | Close of ReactElement
        | CharCode of U2<string, int>
        | CloseAriaLabel of string
        | Custom of IHTMLProp list

    let toastHeader (props: ToastHeaderProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "ToastHeader" "reactstrap" props elems
