module ASTViewer.Server.Encoders

open FSharp.Compiler.Text
open Fantomas.FCS.Parse
open Thoth.Json.Net
open FSharp.Compiler.Diagnostics

let private rangeEncoder (range: Range) =
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

let private encodeFSharpErrorInfo (info: FSharpParserDiagnostic) =
    Encode.object
        [ "subcategory", Encode.string info.SubCategory
          "range", rangeEncoder (Option.defaultValue Range.Zero info.Range)
          "severity", encodeFSharpErrorInfoSeverity info.Severity
          "errorNumber", Encode.int (Option.defaultValue -1 info.ErrorNumber)
          "message", Encode.string info.Message ]

let encodeResponse string (errors: FSharpParserDiagnostic list) =
    let errors =
        List.map encodeFSharpErrorInfo errors
        |> Encode.list

    Encode.object
        [ "string", Encode.string string
          "errors", errors ]
