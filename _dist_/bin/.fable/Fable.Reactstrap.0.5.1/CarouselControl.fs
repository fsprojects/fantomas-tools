namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module CarouselControl =
    [<StringEnum>]
    type CarouselDirection =
        | [<CompiledName("prev")>] Prev
        | [<CompiledName("next")>] Next

    type CarouselControlProps =
        | Direction of CarouselDirection
        | OnClickHandler of (MouseEvent -> unit)
        | DirectionText of string
        | CssModule of CSSModule
        | Custom of IHTMLProp list

    let carouselControl (props: CarouselControlProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "CarouselControl" "reactstrap" props elems
