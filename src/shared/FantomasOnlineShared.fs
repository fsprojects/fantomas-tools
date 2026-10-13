module FantomasOnline.Shared

open FantomasTools.Client

type FantomasOption =
    | IntOption of order: int * name: string * value: int
    | BoolOption of order: int * name: string * value: bool
    | MultilineFormatterTypeOption of order: int * name: string * value: string
    | EndOfLineStyleOption of order: int * name: string * value: string
    | MultilineBracketStyleOption of order: int * name: string * value: string

let sortByOption =
    function
    | IntOption(o, _, _)
    | BoolOption(o, _, _)
    | MultilineFormatterTypeOption(o, _, _)
    | EndOfLineStyleOption(o, _, _)
    | MultilineBracketStyleOption(o, _, _) -> o

let getOptionKey =
    function
    | IntOption(_, k, _)
    | BoolOption(_, k, _)
    | MultilineFormatterTypeOption(_, k, _)
    | EndOfLineStyleOption(_, k, _)
    | MultilineBracketStyleOption(_, k, _) -> k

let optionValue =
    function
    | IntOption(_, _, i) -> i.ToString()
    | BoolOption(_, _, b) -> b.ToString()
    | MultilineFormatterTypeOption(_, _, v)
    | EndOfLineStyleOption(_, _, v)
    | MultilineBracketStyleOption(_, _, v) -> v

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
    {
        SourceCode: string
        Options: FantomasOption list
        IsFsi: bool
    }

/// A `//` or `(* *)` comment of the submitted code: where it is in that code, and its text.
type SourceComment = { Range: Range; Text: string }

/// Something wrong with the code Fantomas produced. Each one is a bug in Fantomas, never in the code
/// that was submitted: code that does not parse fails before there is a result to find fault with.
///
/// `defines` is the define combination the issue was found under. It is empty when there is nothing
/// to say about defines: the code has no conditional directives, the combination is the one without
/// any, or the backend's Fantomas does not say.
[<RequireQualifiedAccess>]
type FormatIssue =
    /// A comment of the submitted code the result does not have.
    | MissingComment of comment: SourceComment
    /// The result is not valid F#. The positions are in the result.
    | NotValidFSharp of defines: string list * diagnostics: Diagnostic array
    /// The comments of the submitted code the result lacks, and those of the result the submitted
    /// code lacks.
    | CommentsChanged of defines: string list * missing: SourceComment array * added: string array
    /// The conditional and warn directives of the submitted code the result lacks, and those of the
    /// result the submitted code lacks.
    | DirectivesChanged of defines: string list * missing: string array * added: string array
    /// Formatting the result again gave this instead of the result.
    | NotIdempotent of formattedAgain: string
    /// A check on the result could not finish, because Fantomas failed on its own result. `check`
    /// names the check as Fantomas does: `Idempotency` when formatting the result again failed.
    | CheckFailed of check: string * message: string

/// The code Fantomas produced and what the checks on it found. The result is reported whatever they
/// found, so the tab can show it beside the issues that condemn it.
type FormatResponse =
    {
        Code: string
        Issues: FormatIssue array
    }

let private supportedProperties =
    set [| "max_line_length"; "indent_size"; "end_of_line" |]

let toEditorConfigName value =
    value
    |> Seq.map (fun c ->
        if System.Char.IsUpper(c) then
            sprintf "_%s" (c.ToString().ToLower())
        else
            c.ToString()
    )
    |> String.concat ""
    |> fun s -> s.TrimStart([| '_' |])
    |> fun name ->
        if Set.contains name supportedProperties then
            name
        else
            sprintf "fsharp_%s" name
