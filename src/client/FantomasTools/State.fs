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


let private getCodeFromUrl () =
    UrlTools.restoreModelFromUrl (Decode.object (fun get -> get.Required.Field "code" Decode.string)) ""

let init _ =
    let sourceCode = getCodeFromUrl ()
    let currentTab, redirectCmd =
        JS.console.log (Navigation.parser Dom.window.location)
        match Navigation.parser Dom.window.location with
        | Some tab -> tab, Cmd.none
        | None -> ActiveTab.HomeTab, Elmish.Navigation.Navigation.modifyUrl (Navigation.toHash ActiveTab.HomeTab)

    let (triviaModel, triviaCmd) = FantomasTools.Client.Trivia.State.init sourceCode
    let (fsharpTokensModel, fsharpTokensCmd) = FantomasTools.Client.FSharpTokens.State.init sourceCode
    let (astModel, astCmd) = FantomasTools.Client.ASTViewer.State.init sourceCode

    let cmd =
        Cmd.batch [
            redirectCmd
            Cmd.map TriviaMsg triviaCmd
            Cmd.map FSharpTokensMsg fsharpTokensCmd
            Cmd.map ASTMsg astCmd
        ]

    let model =
        { ActiveTab = currentTab
          SourceCode = sourceCode
          TriviaModel = triviaModel
          FSharpTokensModel = fsharpTokensModel
          ASTModel = astModel }

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
    | FSharpTokensMsg ftMsg ->
        let (fModel, fCmd) = FantomasTools.Client.FSharpTokens.State.update model.SourceCode ftMsg model.FSharpTokensModel
        { model with FSharpTokensModel = fModel }, Cmd.map FSharpTokensMsg fCmd
    | ASTMsg aMsg ->
        let (aModel, aCmd) = FantomasTools.Client.ASTViewer.State.update model.SourceCode aMsg model.ASTModel
        { model with ASTModel = aModel }, Cmd.map ASTMsg aCmd
    | _ ->
        failwith "not implemented"