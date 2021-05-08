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

        let ``params`` = URLSearchParams.Create()
        ``params``.set ("data", encodedJson)

        let newUrl =
            $"{window.location.protocol}//{window.location.host}{window.location.pathname}{hash}?{``params``.ToString()}"

        history.pushState ({| path = newUrl |}, "", newUrl)

let private encodeUrl (_x: string) : string =
    import "compressToEncodedURIComponent" "lz-string"

let private decodeUrl (_x: string) : string =
    import "decompressFromEncodedURIComponent" "lz-string"

let private URLSearchParamsExist : bool = emitJsExpr () "'URLSearchParams' in window"

let updateUrlBy (mapFn: string -> string) : unit =
    if URLSearchParamsExist then
        let hashPieces = window.location.hash.Split('?')
        let ``params`` = URLSearchParams.Create(hashPieces.[1])

        let safeHash =
            if isNullOrUndefined (window.location.hash) then
                ""
            else
                window.location.hash

        let newHash = (mapFn (safeHash)).Split('?').[0]

        let newUrl =
            $"{window.location.protocol}//{window.location.host}{window.location.pathname}{newHash}?{``params``.ToString()}"

        history.pushState ({| path = newUrl |}, "", newUrl)

let updateUrlWithData json = setGetParam (encodeUrl json)

let private (|KeyValuesFromHash|_|) hash =
    if String.IsNullOrWhiteSpace(hash) then
        None
    else
        let search = hash.Split('?')

        if Seq.length search > 1 then
            search.[1].Split('&')
            |> Array.map (fun kv -> kv.Split('=').[0], kv.Split('=').[1])
            |> Array.choose (fun (k, v) -> if k = "data" then Some v else None)
            |> Array.tryHead
        else
            None

let restoreModelFromUrl decoder defaultValue =
    match Browser.Dom.window.location.hash with
    | KeyValuesFromHash (v) ->
        let json = JS.decodeURIComponent (v) |> decodeUrl
        let modelResult = Decode.fromString decoder json

        match modelResult with
        | Result.Ok m -> m
        | Error err ->
            printfn "%A" err
            defaultValue
    | _ -> defaultValue
