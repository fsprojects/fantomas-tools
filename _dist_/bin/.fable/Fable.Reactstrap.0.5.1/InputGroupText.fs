namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module InputGroupText =

    type InputGroupTextProps = Custom of IHTMLProp list

    let inputGroupText (props: InputGroupTextProps seq) (elems: ReactElement seq): ReactElement =
        let props = keyValueList CaseRules.LowerFirst props
        ofImport "InputGroupText" "reactstrap" props elems
