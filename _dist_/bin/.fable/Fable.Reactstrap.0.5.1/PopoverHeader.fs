namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module PopoverHeader =

    type PopoverHeaderProps = Custom of IHTMLProp list

    let popoverHeader (props: PopoverHeaderProps seq) (elems: ReactElement seq): ReactElement =
        let props = keyValueList CaseRules.LowerFirst props
        ofImport "PopoverHeader" "reactstrap" props elems
