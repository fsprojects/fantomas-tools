module FantomasTools.Client.FantomasOnline.Model

open FantomasOnline.Shared

type FantomasMode =
    | Previous // Fantomas 2.x
    | Latest // Latest stable on NuGet
    | Preview // master branch

type Msg =
    | VersionReceived of string
    | OptionsReceived of FantomasOption list
    | NetworkError of exn
    | Format
    | FormattedReceived of string
    | UpdateOption of (string * FantomasOption)
    | ChangeMode of FantomasMode

type Model =
    { IsFsi: bool
      Version: string
      IsLoading: bool
      DefaultOptions: FantomasOption list
      UserOptions: Map<string, FantomasOption>
      Mode: FantomasMode
      Result: string option }