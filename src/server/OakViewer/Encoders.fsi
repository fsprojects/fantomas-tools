module internal OakViewer.Encoders

open Thoth.Json.Net
open Fantomas.FCS.Parse
open Fantomas.Core.SyntaxOak

val mkDiagnostic: info: FSharpParserDiagnostic -> FantomasTools.Client.Diagnostic
val encode: root: Node -> diagnostics: FSharpParserDiagnostic list -> JsonValue
