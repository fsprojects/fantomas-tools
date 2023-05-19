module FantomasTools.Client.SettingControls

open Fable.React
open Fable.React.Props
open FantomasTools.Client

let input key onChange (labelValue: ReactElement) placeholder value =
    div [ ClassName Style.Setting ] [
        label [] [ labelValue ]
        input [
            Placeholder placeholder
            OnChange(fun ev -> ev.Value |> onChange)
            DefaultValue value
            Key key
        ]
    ]

let private toggleButton_ onClick active label =
    let className = if active then Style.Active else ""

    button [ ClassName className; Key label; OnClick onClick ] [ str label ]

let toggleButton onTrue onFalse labelTrue labelFalse (labelValue: ReactElement) value =
    div [ ClassName Style.Setting ] [
        label [] [ labelValue ]
        div [ ClassName Style.ToggleButton ] [
            toggleButton_ onTrue value labelTrue
            toggleButton_ onFalse (not value) labelFalse
        ]
    ]

type MultiButtonSettings =
    { Label: string
      OnClick: obj -> unit
      IsActive: bool }

let multiButton labelValue (options: MultiButtonSettings list) =
    let buttons =
        options
        |> List.map (fun { Label = l; OnClick = o; IsActive = i } -> toggleButton_ o i l)

    div [ ClassName Style.Setting ] [
        label [] [ str labelValue ]
        div [ ClassName Style.ToggleButton ] [ ofList buttons ]
    ]
