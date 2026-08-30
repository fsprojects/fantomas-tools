namespace FantomasTools.Client.Routing

open System
open Browser.Dom
open Browser.Types
open Fable.Core
open Feliz

[<RequireQualifiedAccess>]
module Route =
    /// Matches the query string segment of a route, so that `#/ast?data=xyz` can be
    /// matched as `[ "ast"; Route.Query [ "data", data ] ]`.
    val (|Query|_|): input: string -> (string * string) list option

[<RequireQualifiedAccess>]
module Router =
    /// The segments of the URL currently in the address bar.
    val currentUrl: unit -> string list

[<AutoOpen>]
module Components =
    /// Renders `content` and reports the current route to `onUrlChanged`, both on mount and
    /// whenever the hash changes.
    [<ReactComponent>]
    val RouteListener: onUrlChanged: (string list -> unit) -> content: ReactElement -> ReactElement
