module FantomasTools.Client.Tests.UrlCompat

open Fable.Core
open Thoth.Json
open FantomasTools.Client

// Every piece of state the tool shares lives in the link: the code, the file extension, the
// defines, the settings. A link posted to a Fantomas issue in 2020 has to open the same document
// today, which puts three things under test at once: lz-string, the way the `data` parameter is
// spelled, and the decoders that read what comes out. The corpus below holds links people actually
// shared, so the test fails for the same reason a user would notice.

[<Import("describe", "vitest")>]
let private describe (name: string) (body: unit -> unit) : unit = jsNative

[<Import("test", "vitest")>]
let private test (name: string) (body: unit -> unit) : unit = jsNative

[<AllowNullLiteral>]
type Matchers =
    abstract toBe: obj -> unit
    abstract toEqual: obj -> unit

[<Import("expect", "vitest")>]
let private expect (value: obj) : Matchers = jsNative

[<Import("readFileSync", "node:fs")>]
let private readFileSync (path: string, encoding: string) : string = jsNative

type Fixture =
    {
        Name: string
        /// Written the way it appears in the link, so a parameter that travelled through a query
        /// string carries %2B where the compressor writes a plain +.
        Encoded: string
        Json: string
    }

let private fixtures: Fixture list =
    let decoder =
        Decode.object (fun get ->
            {
                Name = get.Required.Field "name" Decode.string
                Encoded = get.Required.Field "encoded" Decode.string
                Json = get.Required.Field "json" Decode.string
            })

    // vitest runs from src/client, where the corpus sits next to this file.
    let corpus = readFileSync ("fsharp/test/url-fixtures.json", "utf8")

    match Decode.fromString (Decode.field "fixtures" (Decode.list decoder)) corpus with
    | Ok fixtures -> fixtures
    | Error error -> failwith $"The fixtures could not be read: %s{error}"

/// A link as the tool writes it, so that what is read back went through the same query string.
let private link (hash: string) (json: string) : string =
    $"https://fsprojects.github.io/fantomas-tools/%s{hash}?data=%s{JS.encodeURIComponent (UrlTools.encodeData json)}"

describe "links that already exist" (fun () ->
    for fixture in fixtures do
        test $"decodes %s{fixture.Name}" (fun () -> expect(UrlTools.decodeData fixture.Encoded).toBe(fixture.Json))

    for fixture in fixtures do
        test $"reads the shared state of %s{fixture.Name}" (fun () ->
            // The decoders, not just the decompression: a renamed field would leave the link
            // decoding into an empty editor, which is the failure nobody notices in review.
            let hash = $"#/fantomas/main?data=%s{fixture.Encoded}"

            let bubble =
                UrlTools.restoreModelFromHash hash BubbleModel.decoder BubbleModel.empty

            let expectedCode =
                Decode.unsafeFromString (Decode.field "code" Decode.string) fixture.Json

            expect(bubble.SourceCode).toBe(expectedCode)))

describe "links made from now on" (fun () ->
    // Not what keeps old links working, but if the encoder starts producing a different string for
    // the same document, two links to the same code stop matching, and that is worth a failing
    // test rather than a bug report.
    for fixture in fixtures do
        test $"encodes %s{fixture.Name} to the same string as before" (fun () ->
            expect(UrlTools.encodeData fixture.Json).toBe(JS.decodeURIComponent fixture.Encoded)))

describe "the round trip the tool performs" (fun () ->
    let documents =
        [
            "empty", ""
            "one binding", "let a = 1\n"
            "unicode", "let ``héllo`` = \"ünïcodé 👋🏽\"\n"
            "quotes and slashes", "let s = \"a\\b\\\"c\"\r\nlet t = '\\n'\r\n"
            "the characters a query string cares about", "%&?=#+$ /\\ <>[]{}|^`\"'"
            "something long", String.replicate 5000 "x"
        ]

    for name, code in documents do
        test $"survives a link for %s{name}" (fun () ->
            let json =
                Encode.object [ "code", Encode.string code; "isFsi", Encode.bool false ]
                |> Encode.toString 0

            let bubble =
                UrlTools.restoreModelFromHash (link "#/fantomas/v8" json) BubbleModel.decoder BubbleModel.empty

            expect(bubble.SourceCode).toBe(code))

    test "carries the file extension and the defines" (fun () ->
        let json =
            Encode.object
                [
                    "code", Encode.string "val x: int"
                    "isFsi", Encode.bool true
                    "defines", Encode.string "DEBUG;TRACE"
                ]
            |> Encode.toString 0

        let bubble =
            UrlTools.restoreModelFromHash (link "#/fantomas/v8" json) BubbleModel.decoder BubbleModel.empty

        expect(bubble.IsFsi).toBe(true)
        expect(bubble.Defines).toBe("DEBUG;TRACE"))

    test "keeps the tab next to the data" (fun () ->
        let json = Encode.object [ "code", Encode.string "let a = 1" ] |> Encode.toString 0

        for hash in [ "#/fantomas/v8"; "#/fantomas/main"; "#/ast"; "#/oak" ] do
            let url = link hash json
            expect(url.Contains($"%s{hash}?data=")).toBe(true)

            let bubble = UrlTools.restoreModelFromHash url BubbleModel.decoder BubbleModel.empty
            expect(bubble.SourceCode).toBe("let a = 1")))

describe "a link the tool cannot read" (fun () ->
    // restoreModelFromHash answers with the default rather than throwing, because a hash is
    // whatever was pasted into the address bar.
    let cases =
        [
            "no hash at all", ""
            "a hash without data", "#/fantomas/v8"
            "a data parameter that is not compressed", "#/fantomas/v8?data=not-compressed"
            "a truncated parameter", "#/fantomas/v8?data=N4KAB"
        ]

    for name, hash in cases do
        test $"falls back for %s{name}" (fun () ->
            let bubble =
                UrlTools.restoreModelFromHash hash BubbleModel.decoder BubbleModel.empty

            expect(bubble.SourceCode).toBe("")))
