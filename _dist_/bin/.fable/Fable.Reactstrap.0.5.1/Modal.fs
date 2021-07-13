namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Modal =

    type ModalProps =
        | IsOpen of bool
        | AutoFocus of bool
        | Centered of bool
        | Size of Common.Size
        | Toggle of (unit -> unit)
        | Role of string
        | LabelledBy of string
        | Keyboard of bool
        | Backdrop of U2<bool, string>
        | Scrollable of bool
        | OnEnter of (unit -> unit)
        | OnExit of (unit -> unit)
        | OnOpened of (unit -> unit)
        | OnClosed of (unit -> unit)
        | WrapClassName of string
        | ModalClassName of string
        | BackdropClassName of string
        | ContentClassName of string
        | Fade of bool
        | CssModule of CSSModule
        | ZIndex of U2<int, string>
        | BackdropTransition of Fade.FadeProps seq
        | ModalTransition of Fade.FadeProps seq
        | InnerRef of (Element -> unit)
        | UnmountOnClose of bool
        | Custom of IHTMLProp list

    let modal (props: ModalProps seq) (elems: ReactElement seq): ReactElement =
        let modalProps =
            if Seq.isEmpty props then
                createObj []
            else
                props
                |> Seq.map (fun prop ->
                    match prop with
                    | BackdropTransition fade ->
                        createObj [ "backdropTransition"
                                    ==> keyValueList CaseRules.LowerFirst fade ]
                    | ModalTransition fade ->
                        createObj [ "modalTransition"
                                    ==> keyValueList CaseRules.LowerFirst fade ]
                    | prop -> keyValueList CaseRules.LowerFirst (Seq.singleton prop))
                |> Seq.reduce (fun a b -> Fable.Core.JS.Constructors.Object.assign (a, b))

        ofImport "Modal" "reactstrap" modalProps elems
