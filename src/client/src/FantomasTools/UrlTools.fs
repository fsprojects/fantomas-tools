module FantomasTools.Client.UrlTools

open Fable.Core.JsInterop
open Fable.Core
open Thoth.Json

let private setGetParam _encodedJson: unit = import "setGetParam" "../js/urlUtils.js"

let private encodeUrl (_x: string): string =
    import "compressToEncodedURIComponent" "../js/urlUtils.js"

let private decodeUrl (_x: string): string =
    import "decompressFromEncodedURIComponent" "../js/urlUtils.js"

let updateUrlBy (_mapFn: string -> string): unit = import "updateUrlBy" "../js/urlUtils.js"

let updateUrlWithData json = setGetParam (encodeUrl json)

let private (|KeyValuesFromHash|_|) hash =
    if System.String.IsNullOrWhiteSpace(hash) then
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
        printfn "v: %s" v
        let json = JS.decodeURIComponent (v) |> decodeUrl
        printfn "json: %s" json
        let modelResult = Decode.fromString decoder json

        match modelResult with
        | Result.Ok m -> m
        | Error err ->
            printfn "%A" err
            defaultValue
    | _ -> defaultValue
