module FantomasTools.Client.Http

open Fable.Core
open Fable.Core.JsInterop
open Fetch

let postJson<'TResponse> (url: string) (body: string) : JS.Promise<int * string> =
    let options =
        requestProps [ requestHeaders [ ContentType "application/json" ]
                       Method HttpMethod.POST
                       Body !^body ]

    GlobalFetch.fetch (RequestInfo.Url url, options)
    |> Promise.bind (fun res ->
        promise {
            let! text = res.text ()
            return (res.Status, text)
        })

let getText (url: string) : JS.Promise<string> =
    let options =
        requestProps [ requestHeaders [ ContentType "application/json" ]
                       Method HttpMethod.GET ]

    GlobalFetch.fetch (RequestInfo.Url url, options)
    |> Promise.bind (fun res -> res.text ())
