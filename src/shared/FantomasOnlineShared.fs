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

let tryGetUserOptionValue userOptions key castFunc =
    userOptions |> Map.tryFind key |> Option.map (optionValue >> castFunc)

let tryGetDefaultOptionValue defaultOptions key castFunc =
    defaultOptions
    |> List.tryFind (fun o -> (getOptionKey o) = key)
    |> Option.map (optionValue >> castFunc)

let tryGetOptionValue userOptions defaultOptions key castFunc =
    let userOption = tryGetUserOptionValue userOptions key castFunc

    match userOption with
    | Some n -> Some n
    | None -> tryGetDefaultOptionValue defaultOptions key castFunc

type FormatRequest =
    { SourceCode: string
      Options: FantomasOption list
      IsFsi: bool }

type Range =
    { StartLine: int
      StartCol: int
      EndLine: int
      EndCol: int }

[<RequireQualifiedAccessAttribute>]
type ASTErrorSeverity =
    | Error
    | Warning
    | Info
    | Hidden

type ASTError =
    { SubCategory: string
      Range: Range
      Severity: ASTErrorSeverity
      ErrorNumber: int
      Message: string }

type FormatResponse =
    { FirstFormat: string
      FirstValidation: ASTError list
      SecondFormat: string option
      SecondValidation: ASTError list }
