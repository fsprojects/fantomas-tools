module FantomasTools.Client.UrlTools

open Fable.Core.JsInterop
open Fable.Core
open Thoth.Json
open Browser.Types
open Browser
open System

let private setGetParam (encodedJson: string) : unit =
    if not (isNullOrUndefined history.pushState) then
        let hashPieces =
            window.location.hash.Split([| '?' |], StringSplitOptions.RemoveEmptyEntries)

        let hash =
            if
                not (isNullOrUndefined hashPieces)
                && not (String.IsNullOrWhiteSpace(hashPieces.[0]))
            then
                hashPieces.[0]
            else
                ""

        let ``params`` = URLSearchParams.Create("")
        ``params``.set ("data", encodedJson)

        let newUrl =
            $"%s{window.location.protocol}//%s{window.location.host}%s{window.location.pathname}%s{hash}?%s{``params``.ToString()}"

        let currentUrl = window.location.toString ()

        if currentUrl <> newUrl then
            history.pushState ({| path = newUrl |}, "", newUrl)

let private encodeUrl (_x: string) : string =
    import "compressToEncodedURIComponent" "lz-string"

let private decodeUrl (_x: string) : string =
    import "decompressFromEncodedURIComponent" "lz-string"

/// A link carries the whole model, compressed, in its `data` parameter. Every link ever shared is
/// read by the pair below, so they are kept apart from the window: what they do can be checked
/// against links people posted years ago without a browser in the room.
let encodeData (json: string) : string = encodeUrl json

let decodeData (data: string) : string = JS.decodeURIComponent data |> decodeUrl

/// Called rather than bound, so that reading this module does not need a window.
let private urlSearchParamsExist () : bool = emitJsExpr () "'URLSearchParams' in window"

let updateUrlBy (mapFn: string -> string) : unit =
    if urlSearchParamsExist () then
        let hashPieces = window.location.hash.Split('?')
        let ``params`` = URLSearchParams.Create(hashPieces.[1])

        let safeHash =
            if isNullOrUndefined window.location.hash then
                ""
            else
                window.location.hash

        let newHash = (mapFn safeHash).Split('?').[0]

        let newUrl =
            $"%s{window.location.protocol}//%s{window.location.host}%s{window.location.pathname}%s{newHash}?%s{``params``.ToString()}"

        history.pushState ({| path = newUrl |}, "", newUrl)

let updateUrlWithData json = setGetParam (encodeUrl json)

/// The `data` parameter of a hash, if it has one.
let tryDataFromHash (hash: string) : string voption =
    if String.IsNullOrWhiteSpace(hash) then
        ValueNone
    else
        let search = hash.Split('?')

        if Array.length search > 1 then
            search.[1].Split('&')
            |> Array.map (fun kv -> kv.Split('=').[0], kv.Split('=').[1])
            |> Array.tryPick (fun (k, v) -> if k = "data" then Some v else None)
            |> ValueOption.ofOption
        else
            ValueNone

/// Reads a model out of a hash. A link that cannot be read falls back to the default rather than
/// throwing: the hash is whatever was pasted into the address bar.
let restoreModelFromHash (hash: string) decoder defaultValue =
    match tryDataFromHash hash with
    | ValueNone -> defaultValue
    | ValueSome data ->
        match Decode.fromString decoder (decodeData data) with
        | Result.Ok m -> m
        | Error err ->
            printfn "%A" err
            defaultValue

let restoreModelFromUrl decoder defaultValue =
    restoreModelFromHash Browser.Dom.window.location.hash decoder defaultValue
