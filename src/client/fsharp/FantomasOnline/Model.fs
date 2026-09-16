module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared
open FantomasTools.Client

[<Struct>]
type FantomasMode =
    | V6
    | V7
    | V8
    | Main // main branch
    | Preview // also main branch

type Msg =
    | Bubble of BubbleMessage
    | VersionReceived of string
    | OptionsReceived of FantomasOption list
    | FormatFailed of FormatError
    | Format
    | FormattedReceived of FormatResponse
    | UpdateOption of (string * FantomasOption)
    | ChangeMode of FantomasMode
    | CopySettings
    | UpdateSettingsFilter of string
    | ResetSettings

[<RequireQualifiedAccess>]
type FantomasTabState =
    | LoadingOptions
    | OptionsLoaded
    | LoadingFormatRequest
    | FormatResult of FormatResponse
    | FormatFailed of FormatError

type Model =
    {
        Version: string
        DefaultOptions: FantomasOption list
        UserOptions: Map<string, FantomasOption>
        Mode: FantomasMode
        State: FantomasTabState
        SettingsFilter: string
    }

    member this.SettingsChangedByTheUser =
        let defaultValues = this.DefaultOptions |> List.sortBy sortByOption

        let userValues =
            this.UserOptions |> Map.toList |> List.map snd |> List.sortBy sortByOption

        List.zip defaultValues userValues
        |> List.choose (fun (dv, uv) -> if dv <> uv then Some uv else None)

    member this.MaxLineLength: int =
        tryGetOptionValue this.UserOptions this.DefaultOptions "MaxLineLength" int
        |> Option.defaultValue 120
