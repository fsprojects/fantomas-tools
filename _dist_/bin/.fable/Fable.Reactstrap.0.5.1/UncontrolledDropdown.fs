namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap

[<RequireQualifiedAccess>]
module UncontrolledDropdown =

    type UncontrolledProps = Custom of IHTMLProp list

    let uncontrolledDropdown (props: UncontrolledProps seq) (elems: ReactElement seq): ReactElement =
        ofImport "UncontrolledDropdown" "reactstrap" (keyValueList CaseRules.LowerFirst props) elems
