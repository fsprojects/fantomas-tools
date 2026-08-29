/// Hash-based routing, cut down to the three things Fantomas Tools actually needs:
/// the current URL as segments, a pattern for the query string, and an element that
/// reports hash changes.
///
/// Adapted from Feliz.Router (https://github.com/Zaid-Ajaj/Feliz.Router), MIT licensed,
/// Copyright (c) 2018 Zaid Ajaj. That package targets Feliz 2 and has been unmaintained
/// since 2022; on Feliz 3 it silently compiles to `throw 1` because `React.fragment`,
/// `React.useCallbackRef` and `React.createDisposable` no longer exist.
namespace FantomasTools.Client.Routing

open System
open Browser.Dom
open Browser.Types
open Fable.Core
open Feliz

module private Parsing =
    /// Splits a hash into route segments, keeping any query string as a trailing segment:
    /// `#/` becomes `[]`, `#/fantomas/v7` becomes `[ "fantomas"; "v7" ]` and
    /// `#/ast?data=xyz` becomes `[ "ast"; "?data=xyz" ]`.
    let urlSegments (path: string) : string list =
        let withoutHash =
            if path.StartsWith "#" then
                path.Substring(1, path.Length - 1)
            elif path.EndsWith "#" || path.EndsWith "#/" then
                ""
            else
                path

        withoutHash.Split '/'
        |> List.ofArray
        |> List.collect (fun segment ->
            if String.IsNullOrWhiteSpace segment then
                []
            else
                let segment = segment.TrimEnd '#'

                if segment = "?" then
                    []
                elif segment.StartsWith "?" then
                    [ segment ]
                else
                    match segment.Split [| '?' |] with
                    | [| value |]
                    | [| value; "" |] -> [ JS.decodeURIComponent value ]
                    | [| value; query |] -> [ JS.decodeURIComponent value; "?" + query ]
                    | _ -> [])

[<RequireQualifiedAccess>]
module Route =
    /// Matches the query string segment of a route, so that `#/ast?data=xyz` can be
    /// matched as `[ "ast"; Route.Query [ "data", data ] ]`.
    let (|Query|_|) (input: string) =
        if not (input.StartsWith "?") then
            None
        else
            input.Substring 1
            |> fun query -> query.Split '&'
            |> List.ofArray
            |> List.choose (fun pair ->
                match pair.Split([| '=' |], 2) with
                | [| key; value |] -> Some(JS.decodeURIComponent key, JS.decodeURIComponent value)
                | _ -> None)
            |> Some

[<RequireQualifiedAccess>]
module Router =
    /// The segments of the URL currently in the address bar.
    let currentUrl () : string list = Parsing.urlSegments window.location.hash

[<AutoOpen>]
module Components =
    /// Renders `content` and reports the current route to `onUrlChanged`, both on mount and
    /// whenever the hash changes.
    [<ReactComponent>]
    let RouteListener (onUrlChanged: string list -> unit) (content: ReactElement) : ReactElement =
        // The listeners are registered once, so they read the handler from a ref rather than
        // closing over the one that happened to be current on the first render.
        let handler = React.useRef onUrlChanged
        handler.current <- onUrlChanged

        React.useEffectOnce (fun () ->
            let onChange (_: Event) = handler.current (Router.currentUrl ())

            window.addEventListener ("hashchange", onChange)
            window.addEventListener ("popstate", onChange)

            // Report the route the page was opened on.
            handler.current (Router.currentUrl ())

            let subscription =
                { new IDisposable with
                    member _.Dispose() =
                        window.removeEventListener ("hashchange", onChange)
                        window.removeEventListener ("popstate", onChange)
                }

            subscription)

        content
