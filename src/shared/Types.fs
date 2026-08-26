namespace FantomasTools.Client

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type Range =
    {
        StartLine: int
        StartColumn: int
        EndLine: int
        EndColumn: int
    }

    static member Zero =
        {
            StartLine = 0
            StartColumn = 0
            EndLine = 0
            EndColumn = 0
        }

#if FABLE_COMPILER
    static member Decode: Decoder<Range> =
        Decode.object (fun get ->
            {
                StartLine = get.Required.Field "startLine" Decode.int
                StartColumn = get.Required.Field "startColumn" Decode.int
                EndLine = get.Required.Field "endLine" Decode.int
                EndColumn = get.Required.Field "endColumn" Decode.int
            })
#else
    static member Encode(range: Range) : JsonValue =
        Encode.object
            [
                "startLine", Encode.int range.StartLine
                "startColumn", Encode.int range.StartColumn
                "endLine", Encode.int range.EndLine
                "endColumn", Encode.int range.EndColumn
            ]
#endif

type Diagnostic =
    {
        SubCategory: string
        Range: Range
        Severity: string
        ErrorNumber: int
        Message: string
    }

#if FABLE_COMPILER
    static member Decode: Decoder<Diagnostic> =
        Decode.object (fun get ->
            {
                SubCategory = get.Required.Field "subcategory" Decode.string
                Range = get.Required.Field "range" Range.Decode
                Severity = get.Required.Field "severity" Decode.string
                ErrorNumber = get.Required.Field "errorNumber" Decode.int
                Message = get.Required.Field "message" Decode.string
            })
#else
    static member Encode(diagnostic: Diagnostic) : JsonValue =
        Encode.object
            [
                "subcategory", Encode.string diagnostic.SubCategory
                "range", Range.Encode diagnostic.Range
                "severity", Encode.string diagnostic.Severity
                "errorNumber", Encode.int diagnostic.ErrorNumber
                "message", Encode.string diagnostic.Message
            ]
#endif

/// What sort of failure a backend ran into, which decides what the user can do about it.
[<RequireQualifiedAccess>]
type FormatErrorKind =
    /// The submitted code is not valid F#. Fantomas never got as far as doing anything with it, and
    /// there is nothing here for the Fantomas maintainers to fix.
    | InvalidSource
    /// Fantomas failed on code the parser accepted. That is a bug and worth reporting.
    | FantomasBug
    /// The backend could not place the failure. The exception is all there is to go on.
    | Unknown

/// A failure reported rather than dumped: the backend recognises the exception it caught, which
/// every one of Fantomas' own exceptions can be since they all derive from `FormatException`, and
/// turns it into something a tab can act on instead of sending the stack trace along.
type FormatError =
    {
        Kind: FormatErrorKind
        /// What went wrong, meant to be read.
        Message: string
        /// The parser diagnostics behind the failure, when there are any, so a tab can point at the
        /// source rather than only describe it.
        Diagnostics: Diagnostic array
        /// What whoever triages the report needs and whoever files it does not: the syntax node
        /// involved, or the stack trace of an exception nobody recognised.
        Detail: string option
    }

#if FABLE_COMPILER
    static member private DecodeKind: Decoder<FormatErrorKind> =
        Decode.string
        |> Decode.map (fun kind ->
            match kind with
            | "invalidSource" -> FormatErrorKind.InvalidSource
            | "fantomasBug" -> FormatErrorKind.FantomasBug
            | _ -> FormatErrorKind.Unknown)

    static member Decode: Decoder<FormatError> =
        Decode.object (fun get ->
            {
                Kind = get.Required.Field "kind" FormatError.DecodeKind
                Message = get.Required.Field "message" Decode.string
                Diagnostics = get.Required.Field "diagnostics" (Decode.array Diagnostic.Decode)
                Detail = get.Optional.Field "detail" Decode.string
            })
#else
    static member private EncodeKind(kind: FormatErrorKind) : JsonValue =
        match kind with
        | FormatErrorKind.InvalidSource -> "invalidSource"
        | FormatErrorKind.FantomasBug -> "fantomasBug"
        | FormatErrorKind.Unknown -> "unknown"
        |> Encode.string

    static member Encode(error: FormatError) : JsonValue =
        Encode.object
            [
                "kind", FormatError.EncodeKind error.Kind
                "message", Encode.string error.Message
                "diagnostics", (error.Diagnostics |> Array.map Diagnostic.Encode |> Encode.array)
                "detail", Encode.option Encode.string error.Detail
            ]
#endif

module FormatError =
    /// A failure with nothing known about it beyond what it said.
    let ofMessage (message: string) : FormatError =
        {
            Kind = FormatErrorKind.Unknown
            Message = message
            Diagnostics = Array.empty
            Detail = None
        }

    /// The failure as text for an editor: the message, and the detail below it when there is any.
    let toText (error: FormatError) : string =
        match error.Detail with
        | None -> error.Message
        | Some detail -> $"%s{error.Message}\n\n%s{detail}"

    /// Detail worth carrying, trimmed. A syntax node dumped in full runs to hundreds of lines and
    /// every reader of it reads the first screen.
    let truncateDetail (detail: string) : string option =
        if System.String.IsNullOrWhiteSpace detail then
            None
        elif detail.Length <= 4000 then
            Some detail
        else
            Some(detail.Substring(0, 4000) + "\n\n(truncated)")

    /// What a GitHub issue can carry about the failure. The issue travels as a URL, and one that
    /// grows past what GitHub accepts stops working without saying so, which is how a report gets
    /// lost. The detail stays out and the message is trimmed: the issue carries the source code,
    /// and running it gives back everything left out here.
    let toIssueText (error: FormatError) : string =
        if error.Message.Length <= 1000 then
            error.Message
        else
            error.Message.Substring(0, 1000) + "\n\n(truncated)"
