namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module NavLink =

    type NavLinkProps =
        | Active of bool
        | InnerRef of (Element -> unit)
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list


    let navLink (props: NavLinkProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "NavLink" "reactstrap" props elems
