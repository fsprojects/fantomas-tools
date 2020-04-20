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
        match Navigation.parser Dom.window.location with
        | Some tab -> tab, Cmd.none
        | None -> ActiveTab.HomeTab, Elmish.Navigation.Navigation.modifyUrl (Navigation.toHash ActiveTab.HomeTab)

    let (triviaModel, triviaCmd) =
        Trivia.State.init (currentTab = TriviaTab)

    let (fsharpTokensModel, fsharpTokensCmd) =
        FSharpTokens.State.init (currentTab = TokensTab)

    let (astModel, astCmd) =
        ASTViewer.State.init (currentTab = ASTTab)

    let (fantomasModel, fantomasCmd) =
        FantomasOnline.State.init (FantomasTools.Client.FantomasOnline.Model.Preview)

    let model =
        { ActiveTab = currentTab
          SourceCode = sourceCode
          SettingsOpen = false
          TriviaModel = triviaModel
          FSharpTokensModel = fsharpTokensModel
          ASTModel = astModel
          FantomasModel = fantomasModel }

    let initialCmd =
        Navigation.cmdForCurrentTab currentTab model

    let cmd =
        Cmd.batch
            [ redirectCmd
              Cmd.map TriviaMsg triviaCmd
              Cmd.map FSharpTokensMsg fsharpTokensCmd
              Cmd.map ASTMsg astCmd
              Cmd.map FantomasMsg fantomasCmd
              initialCmd ]

    model, cmd

let private reload model =
    if not model.SettingsOpen then
        match model.ActiveTab with
        | TokensTab ->
            Cmd.ofMsg (FantomasTools.Client.FSharpTokens.Model.GetTokens)
            |> Cmd.map FSharpTokensMsg
        | _ -> Cmd.none
    else
        Cmd.none

let update msg model =
    match msg with
    | SelectTab tab -> model, Navigation.Navigation.newUrl (Navigation.toHash tab)
    | UpdateSourceCode code -> { model with SourceCode = code }, Cmd.none
    | ToggleSettings ->
        let m = { model with SettingsOpen = not model.SettingsOpen }
        m, reload m
    | TriviaMsg tMsg ->
        let (tModel, tCmd) =
            Trivia.State.update model.SourceCode tMsg model.TriviaModel

        { model with TriviaModel = tModel }, Cmd.map TriviaMsg tCmd
    | FSharpTokensMsg ftMsg ->
        let (fModel, fCmd) =
            FSharpTokens.State.update model.SourceCode ftMsg model.FSharpTokensModel

        { model with
              FSharpTokensModel = fModel },
        Cmd.map FSharpTokensMsg fCmd
    | ASTMsg aMsg ->
        let (aModel, aCmd) =
            ASTViewer.State.update model.SourceCode aMsg model.ASTModel

        { model with ASTModel = aModel }, Cmd.map ASTMsg aCmd
    | FantomasMsg (FantomasOnline.Model.ChangeMode mode) ->
        let url =
            // preserve options from hash
            let hash =
                let parts = window.location.hash.Split('?')
                if Seq.length parts = 2 then sprintf "?%s" (parts.[1]) else System.String.Empty

            Navigation.toHash (FantomasTab(mode)) + hash

        model, Navigation.Navigation.newUrl url
    | FantomasMsg fMsg ->
        let isActiveTab =
            match model.ActiveTab with
            | FantomasTab _ -> true
            | _ -> false

        let (fModel, fCmd) =
            FantomasOnline.State.update isActiveTab model.SourceCode fMsg model.FantomasModel

        { model with FantomasModel = fModel }, Cmd.map FantomasMsg fCmd
