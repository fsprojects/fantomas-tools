module FantomasOnline.Shared

type FantomasOption =
    | IntOption of string * int
    | BoolOption of string * bool

type FormatRequest =
    { SourceCode: string
      Options: FantomasOption list
      IsFsi: bool }
