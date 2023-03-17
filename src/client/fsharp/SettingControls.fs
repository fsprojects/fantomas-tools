module FantomasTools.Client.SettingControls

open Fable.React
open Fable.React.Props
open Reactstrap

let input key onChange (labelValue: ReactElement) placeholder value =
    FormGroup.formGroup [] [
        label [] [ labelValue ]
        Input.input [
            Input.Custom [
                Placeholder placeholder
                OnChange(fun ev -> ev.Value |> onChange)
                DefaultValue value
                Key key
            ]
        ]
    ]

let private toggleButton_ onClick active label =
    let className =
        if active then
            "rounded-0 text-white"
        else
            "rounded-0"

    Button.button [
        Button.Custom [ ClassName className; Key label; OnClick onClick ]
        Button.Outline(not active)
    ] [ str label ]

let toggleButton onTrue onFalse labelTrue labelFalse (labelValue: ReactElement) value =
    FormGroup.formGroup [] [
        label [] [ labelValue ]
        br []
        ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0" ] ] [
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

    FormGroup.formGroup [] [
        label [] [ str labelValue ]
        br []
        ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0" ] ] [ ofList buttons ]
    ]
