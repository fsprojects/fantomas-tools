namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module Nav =

    type NavProps =
        | Tabs of bool
        | Pills of bool
        | Card of bool
        | Justified of bool
        | Fill of bool
        | Vertical of U2<bool, string>
        | Horizontal of string
        | Navbar of bool
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list

    let nav (props: NavProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Nav" "reactstrap" props elems
