module FantomasOnline.Shared

type FantomasOption =
    | IntOption of order: int * name: string * value: int
    | BoolOption of order: int * name: string * value: bool
    | MultilineFormatterTypeOption of order: int * name: string * value: string
    | EndOfLineStyleOption of order: int * name: string * value: string

let sortByOption =
    function
    | IntOption (o, _, _)
    | BoolOption (o, _, _)
    | MultilineFormatterTypeOption (o, _, _)
    | EndOfLineStyleOption (o, _, _) -> o

let getOptionKey =
    function
    | IntOption (_, k, _)
    | BoolOption (_, k, _)
    | MultilineFormatterTypeOption (_, k, _)
    | EndOfLineStyleOption (_, k, _) -> k

let optionValue =
    function
    | IntOption (_, _, i) -> i.ToString()
    | BoolOption (_, _, b) -> b.ToString()
    | MultilineFormatterTypeOption (_, _, v)
    | EndOfLineStyleOption (_, _, v) -> v

type FormatRequest =
    { SourceCode: string
      Options: FantomasOption list
      IsFsi: bool }
