module FantomasTools.Client.App

// open FantomasTools.Client
open Fable.React
open Fable.React.Props
open Feliz
open Feliz.Router
open Feliz.UseElmish
open Browser.Dom
open Reactstrap
open FantomasTools.Client

[<ReactComponent>]
let App () =
    let model, dispatch = React.useElmish (State.init, State.update, [||])

    let onUrlChanged url =
        let activeTab = Navigation.parseUrl url
        dispatch (Model.Msg.SelectTab activeTab)

    let routes = View.tabs model dispatch

    fragment [] [
        View.navigation dispatch

        Row.row [ Row.Custom [ ClassName "no-gutters"
                               Id "main" ] ] [
            View.editor model dispatch
            React.router [ router.onUrlChanged onUrlChanged
                           router.children [ routes ] ]
        ]
    ]

ReactDOM.render (App(), document.getElementById ("app"))
