module OakViewer.GetOak

open Fantomas.Core
open Thoth.Json.System.Text.Json
open Fantomas.FCS.Diagnostics
open Fantomas.FCS.Parse
open FantomasTools.Client
open OakViewer.Server

let getVersion () : string =
    let assembly = Fantomas.FCS.Parse.parseFile.GetType().Assembly
    let version = assembly.GetName().Version
    $"%i{version.Major}.%i{version.Minor}.%i{version.Revision}"

let private parseAST source defines isFsi = Fantomas.FCS.Parse.parseFile isFsi source defines

[<RequireQualifiedAccess>]
type GetOakResponse =
    | Ok of text: string
    /// The request itself could not be read.
    | BadRequest of body: string
    /// The request was understood and getting the oak from it failed.
    | Failed of error: FormatError

/// The status code and body that report a failure, the same way the Fantomas tabs report theirs.
let describeFailure (error: FormatError) : int * string =
    let statusCode =
        match error.Kind with
        | FormatErrorKind.InvalidSource -> 400
        | FormatErrorKind.FantomasBug
        | FormatErrorKind.Unknown -> 500

    statusCode, (FormatError.Encode error |> Encode.toString 4)

/// What the parser said about the source, as one sentence naming the earliest error.
let private firstParseError (errors: FSharpParserDiagnostic list) : string =
    let position (d: FSharpParserDiagnostic) : int * int =
        match d.Range with
        | None -> System.Int32.MaxValue, 0
        | Some range -> range.StartLine, range.StartColumn

    match errors |> List.sortBy position |> List.tryHead with
    | None -> "Fantomas could not parse the source."
    | Some error ->
        match error.Range with
        | None -> $"Fantomas could not parse the source: %s{error.Message}"
        | Some range ->
            $"Fantomas could not parse the source: %s{error.Message} at line %i{range.StartLine}, column %i{range.StartColumn + 1}."

/// The exceptions the transformer raises, told apart, given what the parser made of the source.
///
/// Parsing hands its diagnostics back rather than raising, and hands back a tree either way: for
/// source that does not parse, one holding the error nodes the parser invented to carry on. The
/// transformer has no Oak node for those and says so by raising, which reads as Fantomas failing on
/// a tree it should have handled. It is not. The parse errors decide whose fault it is, so they are
/// what this is given and what it answers with.
let private describeException (diagnostics: FSharpParserDiagnostic list) (ex: exn) : FormatError =
    let parseErrors =
        diagnostics
        |> List.filter (fun (d: FSharpParserDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error)

    match ex with
    | :? ParseException as parseException ->
        {
            Kind = FormatErrorKind.InvalidSource
            Message = parseException.Message
            Diagnostics = parseException.Diagnostics |> List.map Encoders.mkDiagnostic |> Array.ofList
            Detail = None
        }
    | :? DefineParseException as defineParseException ->
        {
            Kind = FormatErrorKind.InvalidSource
            Message = defineParseException.Message
            Diagnostics = Array.empty
            Detail = None
        }
    // The source does not parse. Whatever the transformer then made of the recovered tree says
    // nothing about Fantomas, so report the parse errors, which are the reason it got that tree.
    | _ when not (List.isEmpty parseErrors) ->
        {
            Kind = FormatErrorKind.InvalidSource
            Message = firstParseError parseErrors
            Diagnostics = diagnostics |> List.map Encoders.mkDiagnostic |> Array.ofList
            Detail = None
        }
    // The transformer reached a state its own model says is impossible. The message is composed from
    // the data rather than taken from the exception, whose own ends by asking the reader to report
    // this via fantomas-tools: the reader is already there.
    | :? InvariantViolationException as invariantViolation ->
        let range = invariantViolation.Range

        {
            Kind = FormatErrorKind.FantomasBug
            Message =
                $"%s{invariantViolation.Invariant}\nAt line %i{range.StartLine}, column %i{range.StartColumn + 1}."
            Diagnostics = Array.empty
            Detail = FormatError.truncateDetail invariantViolation.SyntaxNode
        }
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

let getOak json : GetOakResponse =
    let parseRequest = Decoders.decodeParseRequest json

    match parseRequest with
    | Ok pr ->
        let {
                SourceCode = content
                Defines = defines
                IsFsi = isFsi
            } =
            pr

        // Parsing and transforming are caught apart, so that a transform that fails knows what the
        // parser said about the source it was handed.
        let parsed =
            try
                Result.Ok(
                    Fantomas.FCS.Parse.parseFile
                        isFsi
                        (Fantomas.FCS.Text.SourceText.ofString content)
                        (List.ofArray defines)
                )
            with ex ->
                Result.Error(describeException [] ex)

        match parsed with
        | Error error -> GetOakResponse.Failed error
        | Ok(ast, diagnostics) ->

        try
            let oak = CodeFormatter.TransformAST(ast, content)

            let responseText = Encoders.encode oak diagnostics |> Encode.toString 4

            GetOakResponse.Ok responseText
        with ex ->
            GetOakResponse.Failed(describeException diagnostics ex)

    | Error err -> GetOakResponse.BadRequest(string err)
