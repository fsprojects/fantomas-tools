module FantomasTools.Client.Tests.FormatResponse

open Fable.Core
open Thoth.Json
open FantomasOnline.Shared
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline

// The backends encode a format response in .NET and the tab decodes it here, with nothing shared
// between the two but field names. The response below is what the server's encoder writes for one
// issue of every kind, so a name that drifts on either side fails here rather than in the tab.

[<Import("describe", "vitest")>]
let private describe (name: string) (body: unit -> unit) : unit = jsNative

[<Import("test", "vitest")>]
let private test (name: string) (body: unit -> unit) : unit = jsNative

[<AllowNullLiteral>]
type Matchers =
    abstract toBe: obj -> unit

[<Import("expect", "vitest")>]
let private expect (value: obj) : Matchers = jsNative

let private encoded =
    """{"code":"x","issues":[{"kind":"missingComment","comment":{"range":{"startLine":0,"startColumn":0,"endLine":0,"endColumn":0},"text":"// c"}},{"kind":"notValidFSharp","defines":["DEBUG"],"diagnostics":[{"subcategory":"parse","range":{"startLine":0,"startColumn":0,"endLine":0,"endColumn":0},"severity":"error","errorNumber":10,"message":"m"}]},{"kind":"commentsChanged","defines":[],"missing":[{"range":{"startLine":0,"startColumn":0,"endLine":0,"endColumn":0},"text":"// c"}],"added":["// d"]},{"kind":"directivesChanged","defines":["A","B"],"missing":["#if A"],"added":[]},{"kind":"notIdempotent","formattedAgain":"y"},{"kind":"checkFailed","check":"Idempotency","message":"boom"}]}"""

let private comment: SourceComment = { Range = Range.Zero; Text = "// c" }

let private expected: FormatResponse =
    {
        Code = "x"
        Issues =
            [|
                FormatIssue.MissingComment comment
                FormatIssue.NotValidFSharp(
                    [ "DEBUG" ],
                    [|
                        {
                            SubCategory = "parse"
                            Range = Range.Zero
                            Severity = "error"
                            ErrorNumber = 10
                            Message = "m"
                        }
                    |]
                )
                FormatIssue.CommentsChanged([], [| comment |], [| "// d" |])
                FormatIssue.DirectivesChanged([ "A"; "B" ], [| "#if A" |], [||])
                FormatIssue.NotIdempotent "y"
                FormatIssue.CheckFailed("Idempotency", "boom")
            |]
    }

describe
    "format response"
    (fun () ->
        test
            "decodes one issue of every kind as the server encodes it"
            (fun () ->
                match Decode.fromString Decoders.decodeFormatResponse encoded with
                | Error error -> failwith error
                | Ok response -> expect(response = expected).toBe true
            )
    )
