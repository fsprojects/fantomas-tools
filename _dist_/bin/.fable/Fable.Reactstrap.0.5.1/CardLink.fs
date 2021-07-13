namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Browser.Types
open Fable.React.Props

[<RequireQualifiedAccess>]
module CardLink =

    type CardLinkProps =
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list
        | InnerRef of (Element -> unit)

    let cardLink (props: CardLinkProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "CardLink" "reactstrap" props elems
