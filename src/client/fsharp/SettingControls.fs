module FantomasTools.Client.SettingControls

open Fable.React
open Fable.React.Props

let input key onChange (labelValue: ReactElement) placeholder value =
    div [ ClassName "mb-3" (* Style.Mb3 *) ] [
        label [ ClassName "form-label" (* Style.FormLabel *) ] [ labelValue ]
        input [
            ClassName "form-control" // Style.FormControl
            Placeholder placeholder
            OnChange(fun ev -> ev.Value |> onChange)
            DefaultValue value
            Key key
        ]
    ]

let private toggleButton_ onClick active label =
    let className =
        if active then
            "text-white btn-secondary" // $"{Style.TextWhite} {Style.BtnOutlinePrimary}"
        else
            "btn-outline-secondary"

    button [ ClassName $"btn {className}"; Key label; OnClick onClick ] [ str label ]
// button [ ClassName $"{Style.Btn} {className}"; Key label; OnClick onClick ] [ str label ]

let toggleButton onTrue onFalse labelTrue labelFalse (labelValue: ReactElement) value =
    div [ ClassName "mb-3" (* Style.Mb3 *) ] [
        label [ ClassName "form-label" (* Style.FormLabel *) ] [ labelValue ]
        br []
        div [ ClassName "btn-group" (* Style.BtnGroup *) ] [
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

    div [] [
        label [ ClassName "form-label" (* Style.FormLabel *) ] [ str labelValue ]
        br []
        div [ ClassName "btn-group" (* Style.BtnGroup *) ] [ ofList buttons ]
    ]
