namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module ListGroupItem =

    type ListGroupItemProps =
        | Active of bool
        | Disabled of bool
        | Color of Common.Color
        | Action of bool
        | CssModule of CSSModule
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list


    let listGroupItem (props: ListGroupItemProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "ListGroupItem" "reactstrap" props elems
