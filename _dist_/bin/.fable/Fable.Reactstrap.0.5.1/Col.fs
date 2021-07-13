namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module Col =
    type ColumnProps =
        { size: U3<bool, int, string>
          order: U2<string, int>
          offset: U2<string, int> }

    let mkCol size =
        { size = size
          order = !^0
          offset = !^0 }

    let withOffset offset col = { col with offset = offset }

    let withOrder order col = { col with order = order }

    type ColProps =
        | Tag of U2<string, obj>
        | Xs of ColumnProps
        | Sm of ColumnProps
        | Md of ColumnProps
        | Lg of ColumnProps
        | Xl of ColumnProps
        | Widths of obj array
        | Custom of IHTMLProp list

    let col (props: ColProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Col" "reactstrap" props elems
