namespace Reactstrap

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Reactstrap
open Fable.React.Props

[<RequireQualifiedAccess>]
module Input =

    [<StringEnum>]
    type InputType =
        | [<CompiledName("text")>] Text
        | [<CompiledName("email")>] Email
        | [<CompiledName("select")>] Select
        | [<CompiledName("file")>] File
        | [<CompiledName("radio")>] Radio
        | [<CompiledName("checkbox")>] Checkbox
        | [<CompiledName("textarea")>] Textarea
        | [<CompiledName("button")>] Button
        | [<CompiledName("reset")>] Reset
        | [<CompiledName("submit")>] Submit
        | [<CompiledName("date")>] Date
        | [<CompiledName("datetime-local")>] DatetimeLocal
        | [<CompiledName("hidden")>] Hidden
        | [<CompiledName("image")>] Image
        | [<CompiledName("month")>] Month
        | [<CompiledName("number")>] Number
        | [<CompiledName("range")>] Range
        | [<CompiledName("search")>] Search
        | [<CompiledName("tel")>] Tel
        | [<CompiledName("url")>] Url
        | [<CompiledName("week")>] Week
        | [<CompiledName("password")>] Password
        | [<CompiledName("datetime")>] Datetime
        | [<CompiledName("time")>] Time
        | [<CompiledName("color")>] Color


    type InputProps =
        | Size of Common.Size
        | BsSize of Common.Size
        | Valid of bool
        | Invalid of bool
        | Plaintext of string
        | Addon of bool
        | Min of int
        | Max of int
        | Step of int
        | InnerRef of (Element -> unit)
        | CssModule of CSSModule
        | Tag of U2<string, obj>
        | Type of InputType
        | Custom of IHTMLProp list

    let input (props: InputProps seq): ReactElement =
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

        ofImport "Input" "reactstrap" props []
