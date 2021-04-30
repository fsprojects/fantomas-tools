module FantomasTools.Client.Http

open Fable.Core
open Fable.Core.JsInterop

let postJson<'TResponse> (url:string) (body:string) : JS.Promise<int * 'TResponse> = 
    emitJsStatement
        (url, body)
        """
return fetch($0, {
            headers: { "Content-Type": "application/json" },
            body:$1,
            method: "POST"
        })
        .then((res) => Promise.all([Promise.resolve(res.status), res.text()]));
        """ 

let getText (url: string) : JS.Promise<string> = 
    emitJsStatement
        url
        """
return fetch(url, {
                headers: { "Content-Type": "application/json" },
                method: "GET"
        })
        .then((res) => res.text());
        """
