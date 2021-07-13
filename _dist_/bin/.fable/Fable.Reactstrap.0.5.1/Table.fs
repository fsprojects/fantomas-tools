namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Table =

    type TableProps =
        | Tag of U2<string, obj>
        | Bordered of bool
        | Borderless of bool
        | Size of Common.Size
        | Striped of bool
        | Dark of bool
        | Hover of bool
        | Responsive of bool
        | InnerRef of (Element -> unit)
        | Custom of IHTMLProp list

    let table (props: TableProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Table" "reactstrap" props elems
