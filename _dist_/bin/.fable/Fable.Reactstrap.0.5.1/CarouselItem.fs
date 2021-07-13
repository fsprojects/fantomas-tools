namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module CarouselItem =
    type CarouselItemProps =
        | In of bool
        | BaseClass of string
        | BaseClassIn of string
        | CssModule of CSSModule
        | OnEnter of (unit -> unit)
        | OnEntering of (unit -> unit)
        | OnEntered of (unit -> unit)
        | OnExit of (unit -> unit)
        | OnExiting of (unit -> unit)
        | OnExited of (unit -> unit)
        | Slide of bool
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list

    let carouselItem (props: CarouselItemProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "CarouselItem" "reactstrap" props elems
