module internal OakViewer.Encoders

open Thoth.Json.Net
open Fantomas.FCS.Parse
open Fantomas.Core.SyntaxOak

val encode: root: Node -> diagnostics: FSharpParserDiagnostic list -> JsonValue
