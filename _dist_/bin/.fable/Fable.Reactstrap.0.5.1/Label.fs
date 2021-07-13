namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module Label =

    type LabelProps =
        | Check of bool
        | Custom of IHTMLProp list

    let label (props: LabelProps seq) (elems: ReactElement seq): ReactElement =
        let props = keyValueList CaseRules.LowerFirst props
        ofImport "Label" "reactstrap" props elems
