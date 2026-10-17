module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared
open FantomasTools.Client

[<Struct>]
type FantomasMode =
    | V6
    | V7
    | V8
    | Main // main branch
    | Preview // next major branch

/// The versions the Fantomas tab offers, in the order it shows them, with their labels.
let modes: (FantomasMode * string) list =
    [
        V6, "6.x"
        V7, "7.x"
        V8, "8.x"
        Main, "Main"
        // The next major's branch, see `previewBranch` in build.fsx.
        Preview, "Preview v9.0"
    ]

/// The label of `mode` in `modes`.
let modeLabel (mode: FantomasMode) : string =
    modes |> List.find (fun (m, _) -> m = mode) |> snd

/// The version after `mode` in `modes`, and after the last one the first.
let nextMode (mode: FantomasMode) : FantomasMode =
    let index = modes |> List.findIndex (fun (m, _) -> m = mode)
    fst modes[(index + 1) % modes.Length]

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
