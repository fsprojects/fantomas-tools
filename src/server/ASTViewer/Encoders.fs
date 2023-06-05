module ASTViewer.Server.Encoders

open Fantomas.FCS.Diagnostics
open Fantomas.FCS.Text
open Fantomas.FCS.Parse
open Thoth.Json.Net

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

let private encodeFSharpParserDiagnostic (info: FSharpParserDiagnostic) =
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

let encodeResponse ast (diagnostics: FSharpParserDiagnostic list) =
    let errors = List.map encodeFSharpParserDiagnostic diagnostics |> Encode.list
    Encode.object [ "ast", Encode.string ast; "diagnostics", errors ]
