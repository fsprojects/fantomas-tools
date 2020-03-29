module FantomasTools.Client.State

open System
open Browser
open Browser
open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Elmish
open FantomasTools.Client
open FantomasTools.Client
open FantomasTools.Client
open FantomasTools.Client.Model
open Fetch
open Thoth.Json

[<Emit("process.env.BACKEND")>]
let private backend: string = jsNative

let init _ =
    let (triviaModel, triviaCmd) = FantomasTools.Client.Trivia.State.init ()
    let currentTab, redirectCmd =
        JS.console.log (Navigation.parser Dom.window.location)
        match Navigation.parser Dom.window.location with
        | Some tab -> tab, Cmd.none
        | None -> ActiveTab.HomeTab, Elmish.Navigation.Navigation.modifyUrl (Navigation.toHash ActiveTab.HomeTab)


    let cmd =
        Cmd.batch [
            redirectCmd
            Cmd.map TriviaMsg triviaCmd
        ]

    let model =
            { ActiveTab = currentTab
              SourceCode = "let a = 42 // ;)"
              TriviaModel = triviaModel }

    model, cmd

let update msg model =
    match msg with
    | SelectTab tab ->
        model, Elmish.Navigation.Navigation.newUrl (Navigation.toHash tab)
    | UpdateSourceCode code ->
        { model with SourceCode = code }, Cmd.none
    | TriviaMsg tMsg ->
        let (tModel, tCmd) = FantomasTools.Client.Trivia.State.update model.SourceCode tMsg model.TriviaModel
        { model with TriviaModel = tModel }, Cmd.map TriviaMsg tCmd
    | _ ->
        failwith "not implemented"