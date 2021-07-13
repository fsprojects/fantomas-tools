namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module Form =
    type FormProps = Custom of IHTMLProp list

    let form (props: FormProps seq) (elems: ReactElement seq): ReactElement =
        let props =
            props
            |> Seq.collect (function
                | Custom props -> props)
            |> keyValueList CaseRules.LowerFirst

        ofImport "Form" "reactstrap" props elems
