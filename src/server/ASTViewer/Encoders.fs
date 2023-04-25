module ASTViewer.Server.Encoders

open FSharp.Compiler.Text
open Fantomas.FCS.Parse
open Thoth.Json.Net
open FSharp.Compiler.Diagnostics

let private mkRange (range: Range) : FantomasTools.Client.Range =
    { StartLine = range.StartLine
      StartColumn = range.StartColumn
      EndLine = range.EndLine
      EndColumn = range.EndColumn }

let private fsharpErrorInfoSeverity =
    function
    | FSharpDiagnosticSeverity.Warning -> "warning"
    | FSharpDiagnosticSeverity.Error -> "error"
    | FSharpDiagnosticSeverity.Hidden -> "hidden"
    | FSharpDiagnosticSeverity.Info -> "info"

let private encodeFSharpErrorInfo (info: FSharpParserDiagnostic) =
    ({ SubCategory = info.SubCategory
       Range =
         match info.Range with
         | None -> mkRange Range.Zero
         | Some r -> mkRange r
       Severity = fsharpErrorInfoSeverity info.Severity
       ErrorNumber = Option.defaultValue 0 info.ErrorNumber
       Message = info.Message }
    : FantomasTools.Client.Diagnostic)
    |> FantomasTools.Client.Diagnostic.Encode

let encodeResponse string (errors: FSharpParserDiagnostic list) =
    let errors = List.map encodeFSharpErrorInfo errors |> Encode.list
    Encode.object [ "string", Encode.string string; "errors", errors ]
