module FantomasOnline.Shared

type FantomasOption =
    | IntOption of order: int * name: string * value: int
    | BoolOption of order: int * name: string * value: bool

let sortByOption =
    function
    | IntOption (o, _, _)
    | BoolOption (o, _, _) -> o

let getOptionKey =
    function
    | IntOption (_, k, _)
    | BoolOption (_, k, _) -> k

let optionValue =
    function
    | IntOption (_, _, i) -> i.ToString()
    | BoolOption (_, _, b) -> b.ToString()

type FormatRequest =
    { SourceCode: string
      Options: FantomasOption list
      IsFsi: bool }
