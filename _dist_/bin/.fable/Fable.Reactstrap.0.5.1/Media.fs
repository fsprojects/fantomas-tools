namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module Media =

    type MediaProps =
        | Body of bool
        | Bottom of bool
        | Heading of bool
        | Left of bool
        | List of bool
        | Middle of bool
        | Object of bool
        | Right of bool
        | Top of bool
        | Tag of U2<string, obj>
        | Custom of IHTMLProp list

    let media (props: MediaProps seq) (elems: ReactElement seq): ReactElement =
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

        ofImport "Media" "reactstrap" props elems
