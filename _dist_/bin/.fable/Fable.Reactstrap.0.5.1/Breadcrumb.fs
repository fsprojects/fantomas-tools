namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Breadcrumb =
    type BreadcrumbProps =
        | Tag of U2<string, obj>
        | ListTag of U2<string, obj>
        | ListClassName of string
        | CSSModule of Common.CSSModule
        | Custom of IHTMLProp list

    let breadcrumb (props: BreadcrumbProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Breadcrumb" "reactstrap" props elems
