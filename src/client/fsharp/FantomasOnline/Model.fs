module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared

type FantomasMode =
    | V4
    | V5
    | Main // main branch
    | Preview // Also main branch, formerly v6.0 branch

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
    | UpdateSettingsFilter of string

type EditorState =
    | LoadingOptions
    | OptionsLoaded
    | LoadingFormatRequest
    | FormatResult of FormatResponse
    | FormatError of string

type Model =
    { Version: string
      DefaultOptions: FantomasOption list
      UserOptions: Map<string, FantomasOption>
      Mode: FantomasMode
      State: EditorState
      SettingsFilter: string }

    member this.SettingsChangedByTheUser =
        let defaultValues = this.DefaultOptions |> List.sortBy sortByOption

        let userValues =
            this.UserOptions |> Map.toList |> List.map snd |> List.sortBy sortByOption

        List.zip defaultValues userValues
        |> List.filter (fun (dv, uv) -> dv <> uv)
        |> List.map snd

    member this.MaxLineLength: int =
        FantomasOnline.Shared.tryGetOptionValue this.UserOptions this.DefaultOptions "MaxLineLength" int
        |> Option.defaultValue 120
