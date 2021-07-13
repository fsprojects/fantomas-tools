namespace Reactstrap

open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

[<RequireQualifiedAccess>]
module PopoverBody =

    type PopoverBodyProps = Custom of IHTMLProp list

    let popoverBody (props: PopoverBodyProps seq) (elems: ReactElement seq): ReactElement =
        let props = keyValueList CaseRules.LowerFirst props
        ofImport "PopoverBody" "reactstrap" props elems
