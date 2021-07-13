namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module PaginationLink =

    type PaginationLinkProps =
        | CssModule of CSSModule
        | Next of bool
        | Previous of bool
        | First of bool
        | Last of bool
        | Tag of U2<string, obj>
        | [<CompiledName("aria-label")>] AriaLabel of string
        | Custom of IHTMLProp list

    let paginationLink (props: PaginationLinkProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "PaginationLink" "reactstrap" props elems
