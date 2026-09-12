module FantomasOnlineMain.FormatCode

open System
open Fantomas.FCS.Diagnostics
open Fantomas.FCS.Parse
open Fantomas.FCS.Text
open Fantomas.Core
open FantomasOnline.Shared
open FantomasOnline.Server.Shared.Http
open FantomasTools.Client

let private mapFantomasOptionsToRecord options =
    let newValues =
        options
        |> Seq.map (function
            | BoolOption(_, _, v) -> box v
            | IntOption(_, _, v) -> box v
            | MultilineFormatterTypeOption(_, _, v) ->
                MultilineFormatterType.OfConfigString(v)
                |> Option.defaultValue CharacterWidth
                |> box
            | EndOfLineStyleOption(_, _, v) ->
                EndOfLineStyle.OfConfigString(v)
                |> Option.defaultValue EndOfLineStyle.CRLF
                |> box
            | MultilineBracketStyleOption(_, _, v) ->
                MultilineBracketStyle.OfConfigString(v)
                |> Option.defaultValue MultilineBracketStyle.Cramped
                |> box)
        |> Seq.toArray

    let formatConfigType = typeof<FormatConfig>
    Microsoft.FSharp.Reflection.FSharpValue.MakeRecord(formatConfigType, newValues) :?> FormatConfig

let private format (fileName: string) code config =
    let isSignature = fileName.EndsWith(".fsi", StringComparison.Ordinal)

    async {
        let! result = CodeFormatter.FormatDocumentAsync(isSignature, code, config)
        return result.Code
    }

let private toDiagnostic (e: FSharpParserDiagnostic) : Diagnostic =
    let range =
        match e.Range with
        | None ->
            {
                StartLine = 0
                StartColumn = 0
                EndLine = 0
                EndColumn = 0
            }
        | Some r ->
            {
                StartLine = r.StartLine
                StartColumn = r.StartColumn
                EndLine = r.EndLine
                EndColumn = r.EndColumn
            }

    {
        SubCategory = e.SubCategory
        Range = range
        Severity = $"%O{e.Severity}".ToLower()
        ErrorNumber = Option.defaultValue -1 e.ErrorNumber
        Message = e.Message
    }

let private validate (fileName: string) code =
    let isSignature = fileName.EndsWith(".fsi", StringComparison.Ordinal)

    async {
        // Ask Fantomas rather than parsing once here. Fantomas reads the code once per define
        // combination, and code can parse with nothing defined and fail with something defined:
        // a single parse with no defines never sees that, and the failure then surfaces as an
        // exception out of the next format instead of as a diagnostic about this one.
        let! result = CodeFormatter.ValidateFSharpCodeAsync(isSignature, code)
        return result.Diagnostics |> List.map toDiagnostic
    }

/// What the parser said about a piece of code, as one sentence naming the earliest error.
let private firstParseError (diagnostics: FSharpParserDiagnostic list) : string =
    let position (d: FSharpParserDiagnostic) : int * int =
        match d.Range with
        | None -> System.Int32.MaxValue, 0
        | Some range -> range.StartLine, range.StartColumn

    let errors =
        diagnostics
        |> List.filter (fun (d: FSharpParserDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error)

    match errors |> List.sortBy position |> List.tryHead with
    | None -> "It does not parse."
    | Some error ->
        match error.Range with
        | None -> $"%s{error.Message}"
        | Some range -> $"%s{error.Message} at line %i{range.StartLine}, column %i{range.StartColumn + 1}."

/// The exceptions Fantomas raises, told apart. Every failure used to reach the user as the string of
/// whatever was caught, stack trace and all, which said "this is broken" about source that simply
/// does not parse as loudly as it did about a genuine bug. Fantomas 8 derives all of them from
/// `FormatException` and gives each one its data, so the backend can say which of the two happened.
let private describeException (ex: exn) : FormatError =
    match ex with
    // The submitted code is not valid F#. Fantomas never got as far as formatting it and the parser
    // already said where it gave up, so report that instead of the exception wrapped around it.
    // The message says whose code it is: the tool formats twice and this is the only failure of the
    // three that is about what the user typed rather than about what Fantomas made of it.
    | :? ParseException as parseException ->
        {
            Kind = FormatErrorKind.InvalidSource
            Message = $"The code you submitted does not parse: %s{firstParseError parseException.Diagnostics}"
            Diagnostics = parseException.Diagnostics |> List.map toDiagnostic |> Array.ofList
            Detail = None
        }
    // The code parses with nothing defined and not with something defined. Fantomas names the
    // combinations and drops their diagnostics; the caller fills those back in.
    | :? DefineParseException as defineParseException ->
        let combinations = defineParseException.Combinations |> String.concat "; "

        {
            Kind = FormatErrorKind.InvalidSource
            Message = $"The code you submitted does not parse with these defines set: %s{combinations}."
            Diagnostics = Array.empty
            Detail = None
        }
    // Fantomas reached a state its own model says is impossible. That is never the source's doing.
    // The syntax node is what whoever picks the issue up needs, so it travels along as detail.
    // The message is composed from the data rather than taken from the exception: the one the
    // exception writes ends by asking the reader to report this via fantomas-tools, which is where
    // the reader already is. Saying it is a bug is this tab's job, and it has a button for it.
    | :? InvariantViolationException as invariantViolation ->
        let range = invariantViolation.Range

        {
            Kind = FormatErrorKind.FantomasBug
            Message =
                $"%s{invariantViolation.Invariant}\nAt line %i{range.StartLine}, column %i{range.StartColumn + 1}."
            Diagnostics = Array.empty
            Detail = FormatError.truncateDetail invariantViolation.SyntaxNode
        }
    // Anything else Fantomas raises on purpose: it failed on code the parser accepted, which is
    // worth reporting. Its message is written to be read, so nothing else is needed here.
    | :? Fantomas.Core.FormatException as formatException ->
        {
            Kind = FormatErrorKind.FantomasBug
            Message = formatException.Message
            Diagnostics = Array.empty
            Detail = None
        }
    | ex ->
        // The stack trace goes to the log and no further. It is of no use to whoever pasted the
        // code, and the tool hands the error to GitHub as part of a URL: one long enough to push
        // that URL past what GitHub accepts takes the report down with it.
        eprintfn $"%O{ex}"

        {
            Kind = FormatErrorKind.Unknown
            Message = $"%s{ex.GetType().Name}: %s{ex.Message}"
            Diagnostics = Array.empty
            Detail = None
        }

let getVersion () =
    let date =
        let lastCommitInfo =
            sprintf
                "%s - %s"
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_TIMESTAMP"))
                (System.Environment.GetEnvironmentVariable("LAST_COMMIT_SHA"))

        if lastCommitInfo.Trim() <> "-" then
            lastCommitInfo
        else
            let assembly = typeof<FormatConfig>.Assembly

            System.IO.FileInfo assembly.Location
            |> fun f -> f.LastWriteTime.ToShortDateString()

    $"main branch at %s{date}"

let getOptions () : string =
    Reflection.getRecordFields FormatConfig.Default
    |> Seq.indexed
    |> Seq.choose (fun (idx, (k: string, v: obj)) ->
        match v with
        | :? int as i -> FantomasOption.IntOption(idx, k, i) |> Some
        | :? bool as b -> FantomasOption.BoolOption(idx, k, b) |> Some
        | :? MultilineFormatterType as mft ->
            FantomasOption.MultilineFormatterTypeOption(idx, k, (MultilineFormatterType.ToConfigString mft))
            |> Some
        | :? EndOfLineStyle as eol ->
            FantomasOption.EndOfLineStyleOption(idx, k, (EndOfLineStyle.ToConfigString eol))
            |> Some
        | :? MultilineBracketStyle as mbs ->
            FantomasOption.MultilineBracketStyleOption(idx, k, (MultilineBracketStyle.ToConfigString mbs))
            |> Some
        | _ -> None)
    |> Seq.toList
    |> mapOptionsToJson

let formatCode: string -> Async<FormatResponse> =
    formatCode mapFantomasOptionsToRecord format validate describeException
