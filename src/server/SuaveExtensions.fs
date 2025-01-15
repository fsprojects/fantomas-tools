module SuaveExtensions

open System.Net
open System.Net.Sockets
open Suave
open Suave.Operators
open Suave.Writers
open Suave.Response
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors
open HttpConstants

type HttpRequest with

    member this.BodyText = System.Text.Encoding.UTF8.GetString this.rawForm

let applicationJson = setMimeType HeaderValues.ApplicationJson
let applicationText = setMimeType HeaderValues.ApplicationText
let textPlain = setMimeType HeaderValues.TextPlain
let private getBytes (v: string) = System.Text.Encoding.UTF8.GetBytes v
let REQUEST_ENTITY_TOO_LARGE (body: string) = response HTTP_413 (getBytes body)
let INTERNAL_SERVER_ERROR (body: string) = response HTTP_500 (getBytes body)

let setCORSHeaders =
    addHeader "Access-Control-Allow-Origin" "*"
    >=> addHeader "Access-Control-Allow-Headers" "*"
    >=> addHeader "Access-Control-Allow-Methods" "*"

let startFantomasTool port routes =
    try
        setCORSHeaders
        >=> choose [ OPTIONS >=> no_content; yield! routes; NOT_FOUND "Not found" ]
        |> startWebServer
            { defaultConfig with
                bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ] }
    with :? SocketException ->
        printfn $"Port {port} is already in use"
