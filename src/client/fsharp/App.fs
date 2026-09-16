module FantomasTools.Client.App

open Fable.Core.JsInterop
open Browser.Types
open Fable.React
open Feliz
open FantomasTools.Client.Routing
open Feliz.UseElmish
open Browser.Dom
open FantomasTools.Client

[<ReactComponent>]
let App () =
    let model, dispatch =
        React.useElmish (State.init, State.update, dependencies = [||])

    let onUrlChanged url =
        let activeTab = Navigation.parseUrl url
        dispatch (Model.Msg.SelectTab activeTab)

    let routes = View.rightPane model dispatch

    fragment [] [
        View.navigation dispatch
        main [] [
            View.editor model dispatch
            Splitter.Splitter()
            RouteListener onUrlChanged routes
        ]
    ]

let createRoot: Element -> {| render: ReactElement -> unit |} =
    import "createRoot" "react-dom/client"

let root = createRoot (document.getElementById "app")
root.render (React.StrictMode [ App() ])
