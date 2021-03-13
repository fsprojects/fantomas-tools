module ASTViewer.Server.Encoders

open Thoth.Json.Net
open FSharp.Compiler.SourceCodeServices

let private rangeEncoder (range: FSharp.Compiler.Text.Range) =
    Encode.object
        [ "startLine", Encode.int range.StartLine
          "startCol", Encode.int range.StartColumn
          "endLine", Encode.int range.EndLine
          "endCol", Encode.int range.EndColumn ]

let private encodeFSharpErrorInfoSeverity =
    function
    | FSharpDiagnosticSeverity.Warning -> Encode.string "warning"
    | FSharpDiagnosticSeverity.Error -> Encode.string "error"
    | FSharpDiagnosticSeverity.Hidden -> Encode.string "hidden"
    | FSharpDiagnosticSeverity.Info -> Encode.string "info"

let private encodeFSharpErrorInfo (info: FSharpDiagnostic) =
    Encode.object
        [ "subcategory", Encode.string info.Subcategory
          "range", rangeEncoder info.Range
          "severity", encodeFSharpErrorInfoSeverity info.Severity
          "errorNumber", Encode.int info.ErrorNumber
          "message", Encode.string info.Message ]

let encodeResponse string (errors: FSharpDiagnostic array) =
    let errors =
        Array.map encodeFSharpErrorInfo errors
        |> Encode.array

    Encode.object
        [ "string", Encode.string string
          "errors", errors ]
