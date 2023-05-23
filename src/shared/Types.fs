namespace FantomasTools.Client

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type Range =
    { StartLine: int
      StartColumn: int
      EndLine: int
      EndColumn: int }

    static member Zero =
        { StartLine = 0
          StartColumn = 0
          EndLine = 0
          EndColumn = 0 }

#if FABLE_COMPILER
    static member Decode: Decoder<Range> =
        Decode.object (fun get ->
            { StartLine = get.Required.Field "startLine" Decode.int
              StartColumn = get.Required.Field "startColumn" Decode.int
              EndLine = get.Required.Field "endLine" Decode.int
              EndColumn = get.Required.Field "endColumn" Decode.int })
#else
    static member Encode(range: Range) : JsonValue =
        Encode.object
            [ "startLine", Encode.int range.StartLine
              "startColumn", Encode.int range.StartColumn
              "endLine", Encode.int range.EndLine
              "endColumn", Encode.int range.EndColumn ]
#endif

type Diagnostic =
    { SubCategory: string
      Range: Range
      Severity: string
      ErrorNumber: int
      Message: string }

#if FABLE_COMPILER
    static member Decode: Decoder<Diagnostic> =
        Decode.object (fun get ->
            { SubCategory = get.Required.Field "subcategory" Decode.string
              Range = get.Required.Field "range" Range.Decode
              Severity = get.Required.Field "severity" Decode.string
              ErrorNumber = get.Required.Field "errorNumber" Decode.int
              Message = get.Required.Field "message" Decode.string })
#else
    static member Encode(diagnostic: Diagnostic) : JsonValue =
        Encode.object
            [ "subcategory", Encode.string diagnostic.SubCategory
              "range", Range.Encode diagnostic.Range
              "severity", Encode.string diagnostic.Severity
              "errorNumber", Encode.int diagnostic.ErrorNumber
              "message", Encode.string diagnostic.Message ]
#endif
