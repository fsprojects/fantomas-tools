module FantomasTools.Client.Http

open Fable.Core
open Fable.Core.JsInterop

let postJson<'TResponse> _url _body : JS.Promise<int * 'TResponse> = import "postJson" "../js/http"

let getText _url : JS.Promise<string> = import "getJson" "../js/http"
