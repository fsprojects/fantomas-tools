module FantomasTools.Client.State

open FantomasTools.Client
open Browser
open Fable.Core
open Elmish
open FantomasTools.Client.Model
open Thoth.Json

let private getCodeFromUrl () =
    UrlTools.restoreModelFromUrl (Decode.object (fun get -> get.Required.Field "code" Decode.string)) ""

let init _ =
    let sourceCode = getCodeFromUrl ()
    let currentTab, redirectCmd =
        JS.console.log (Navigation.parser Dom.window.location)
        match Navigation.parser Dom.window.location with
        | Some tab -> tab, Cmd.none
        | None -> ActiveTab.HomeTab, Elmish.Navigation.Navigation.modifyUrl (Navigation.toHash ActiveTab.HomeTab)

    let (triviaModel, triviaCmd) = Trivia.State.init sourceCode
    let (fsharpTokensModel, fsharpTokensCmd) = FSharpTokens.State.init sourceCode
    let (astModel, astCmd) = ASTViewer.State.init sourceCode
    let (fantomasModel, fantomasCmd) = FantomasOnline.State.init (FantomasTools.Client.FantomasOnline.Model.Preview)

    let cmd =
        Cmd.batch [
            redirectCmd
            Cmd.map TriviaMsg triviaCmd
            Cmd.map FSharpTokensMsg fsharpTokensCmd
            Cmd.map ASTMsg astCmd
            Cmd.map FantomasMsg fantomasCmd
        ]

    let model =
        { ActiveTab = currentTab
          SourceCode = sourceCode
          TriviaModel = triviaModel
          FSharpTokensModel = fsharpTokensModel
          ASTModel = astModel
          FantomasModel = fantomasModel }

    model, cmd

let update msg model =
    match msg with
    | SelectTab tab ->
        model, Navigation.Navigation.newUrl (Navigation.toHash tab)
    | UpdateSourceCode code ->
        { model with SourceCode = code }, Cmd.none
    | TriviaMsg tMsg ->
        let (tModel, tCmd) = Trivia.State.update model.SourceCode tMsg model.TriviaModel
        { model with TriviaModel = tModel }, Cmd.map TriviaMsg tCmd
    | FSharpTokensMsg ftMsg ->
        let (fModel, fCmd) = FSharpTokens.State.update model.SourceCode ftMsg model.FSharpTokensModel
        { model with FSharpTokensModel = fModel }, Cmd.map FSharpTokensMsg fCmd
    | ASTMsg aMsg ->
        let (aModel, aCmd) = ASTViewer.State.update model.SourceCode aMsg model.ASTModel
        { model with ASTModel = aModel }, Cmd.map ASTMsg aCmd
    | FantomasMsg fMsg ->
        let (fModel, fCmd) = FantomasOnline.State.update model.SourceCode fMsg model.FantomasModel
        { model with FantomasModel = fModel }, Cmd.map FantomasMsg fCmd