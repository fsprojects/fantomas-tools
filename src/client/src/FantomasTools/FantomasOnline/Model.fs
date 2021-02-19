module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared

type FantomasMode =
    | V2
    | V3
    | V4
    | Preview // master branch

type Msg =
    | VersionReceived of string
    | OptionsReceived of FantomasOption list
    | FormatException of string
    | Format
    | FormattedReceived of FormatResponse
    | UpdateOption of (string * FantomasOption)
    | ChangeMode of FantomasMode
    | SetFsiFile of bool
    | CopySettings

type EditorState =
    | LoadingOptions
    | OptionsLoaded
    | LoadingFormatRequest
    | FormatResult of FormatResponse
    | FormatError of string

type Model =
    { IsFsi: bool
      Version: string
      DefaultOptions: FantomasOption list
      UserOptions: Map<string, FantomasOption>
      Mode: FantomasMode
      State: EditorState }

    member this.SettingsChangedByTheUser =
        let defaultValues = this.DefaultOptions |> List.sortBy sortByOption

        let userValues =
            this.UserOptions
            |> Map.toList
            |> List.map snd
            |> List.sortBy sortByOption

        List.zip defaultValues userValues
        |> List.filter (fun (dv, uv) -> dv <> uv)
        |> List.map snd
