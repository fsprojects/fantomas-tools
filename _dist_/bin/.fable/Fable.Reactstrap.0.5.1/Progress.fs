namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Progress =

    type ProgressProps =
        | Tag of U2<string, obj>
        | Bar of bool
        | Value of U2<string, int>
        | Max of U2<string, int>
        | Animated of bool
        | Multi of bool
        | Striped of bool
        | Color of Common.Color
        | BarClassName of string
        | Custom of IHTMLProp list

    let progress (props: ProgressProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Progress" "reactstrap" props elems
