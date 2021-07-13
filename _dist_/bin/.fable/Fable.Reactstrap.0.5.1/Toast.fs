namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Toast =

    type ToastProps =
        | Color of Common.Color
        | IsOpen of bool
        | Tag of U2<string, obj>
        | Transition of Fade.FadeProps seq
        | Custom of IHTMLProp list

    let toast (props: ToastProps seq) (elems: ReactElement seq): ReactElement =
        let toastProps =
            if Seq.isEmpty props then
                createObj []
            else
                props
                |> Seq.map (fun prop ->
                    match prop with
                    | Transition fade ->
                        createObj [ "transition"
                                    ==> keyValueList CaseRules.LowerFirst fade ]
                    | Custom customProps -> keyValueList CaseRules.LowerFirst customProps
                    | prop -> keyValueList CaseRules.LowerFirst (Seq.singleton prop))
                |> Seq.reduce (fun a b -> Fable.Core.JS.Constructors.Object.assign (a, b))

        ofImport "Toast" "reactstrap" toastProps elems
