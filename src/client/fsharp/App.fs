module FantomasTools.Client.App

open Fable.Core.JsInterop
open Browser.Types
open Fable.React
open Feliz
open Feliz.Router
open Feliz.UseElmish
open Browser.Dom
open FantomasTools.Client

[<ReactComponent>]
let App () =
    let model, dispatch = React.useElmish (State.init, State.update, [||])

    let onUrlChanged url =
        printfn "onUrlChanged"
        let activeTab = Navigation.parseUrl url
        dispatch (Model.Msg.SelectTab activeTab)

    let routes = View.rightPane model dispatch

    fragment [] [
        View.navigation dispatch
        main [] [
            View.editor model dispatch
            React.router [ router.onUrlChanged onUrlChanged; router.children [ routes ] ]
        ]
    ]

let createRoot: Element -> {| render: ReactElement -> unit |} =
    import "createRoot" "react-dom/client"

let root = createRoot (document.getElementById "app")
root.render (React.strictMode [ App() ])
