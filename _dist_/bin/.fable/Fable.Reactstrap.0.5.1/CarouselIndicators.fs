namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

type Item =
    { Src: string
      AltText: string
      Caption: string }

[<RequireQualifiedAccess>]
module CarouselIndicators =

    type CarouselIndicatorsProps =
        | Items of Item seq
        | ActiveIndex of int
        | CssModule of CSSModule
        | OnClickHandler of (int -> unit)
        | Custom of IHTMLProp list

    let carouselIndicators (props: CarouselIndicatorsProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "CarouselIndicators" "reactstrap" props elems
