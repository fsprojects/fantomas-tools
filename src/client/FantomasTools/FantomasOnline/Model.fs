module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared

type FantomasMode =
    | Previous // Fantomas 2.x
    | Latest // Latest stable on NuGet
    | Preview // master branch

type Msg =
    | VersionReceived of string
    | OptionsReceived of FantomasOption list
    | FormatException of string
    | Format
    | FormattedReceived of string
    | UpdateOption of (string * FantomasOption)
    | ChangeMode of FantomasMode
    | SetFsiFile of bool
    | CopySettings

type EditorState =
    | LoadingOptions
    | OptionsLoaded
    | LoadingFormatRequest
    | FormatResult of string
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
