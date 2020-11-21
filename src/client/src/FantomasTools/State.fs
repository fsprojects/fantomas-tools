module FantomasTools.Client.State

open FantomasTools.Client
open Browser
open Elmish
open FantomasTools.Client.Model
open Thoth.Json
open Feliz.Router

let private getCodeFromUrl () =
    UrlTools.restoreModelFromUrl (Decode.object (fun get -> get.Required.Field "code" Decode.string)) ""

let init _ =
    let sourceCode = getCodeFromUrl ()

    //    let currentTab, redirectCmd =
//        match Navigation.parseUrl (Router.currentUrl()) with
//        | Some tab -> tab, Cmd.none
//        | None -> ActiveTab.HomeTab, Elmish.Navigation.Navigation.modifyUrl (Navigation.toHash ActiveTab.HomeTab)
    let currentTab = Navigation.parseUrl (Router.currentUrl ())

    let (triviaModel, triviaCmd) = Trivia.State.init (currentTab = TriviaTab)

    let (fsharpTokensModel, fsharpTokensCmd) = FSharpTokens.State.init (currentTab = TokensTab)

    let (astModel, astCmd) = ASTViewer.State.init (currentTab = ASTTab)

    let (fantomasModel, fantomasCmd) =
        let tab =
            match currentTab with
            | ActiveTab.FantomasTab ft -> ft
            | _ -> FantomasTools.Client.FantomasOnline.Model.Preview

        FantomasOnline.State.init tab

    let model =
        { ActiveTab = currentTab
          SourceCode = sourceCode
          SettingsOpen = false
          TriviaModel = triviaModel
          FSharpTokensModel = fsharpTokensModel
          ASTModel = astModel
          FantomasModel = fantomasModel }

    let initialCmd = Navigation.cmdForCurrentTab currentTab model
    //
    let cmd =
        Cmd.batch [ Cmd.map TriviaMsg triviaCmd
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
        | ASTTab ->
            Cmd.ofMsg (FantomasTools.Client.ASTViewer.Model.DoParse)
            |> Cmd.map ASTMsg
        | TriviaTab ->
            Cmd.ofMsg (FantomasTools.Client.Trivia.Model.GetTrivia)
            |> Cmd.map TriviaMsg
        | FantomasTab _ ->
            Cmd.ofMsg (FantomasTools.Client.FantomasOnline.Model.Format)
            |> Cmd.map FantomasMsg
        | _ -> Cmd.none
    else
        Cmd.none

let update msg model =
    match msg with
    | SelectTab tab ->
        let nextModel =
            match tab with
            | ActiveTab.FantomasTab ft when (ft <> model.FantomasModel.Mode) ->
                { model with
                      ActiveTab = tab
                      FantomasModel = { model.FantomasModel with Mode = ft } }
            | _ -> { model with ActiveTab = tab }

        // model, Navigation.Navigation.newUrl (Navigation.toHash tab)
//        let cmd =
//            match tab with
//            | ActiveTab.HomeTab -> Cmd.navigate("")
//            | ActiveTab.TokensTab -> Cmd.navigate("tokens")
//            | ActiveTab.ASTTab -> Cmd.navigate("ast")
//            | ActiveTab.TriviaTab -> Cmd.navigate("trivia")
//            | ActiveTab.FantomasTab (FantomasTools.Client.FantomasOnline.Model.V2) -> Cmd.navigate("fantomas", "v2")
//            | ActiveTab.FantomasTab (FantomasTools.Client.FantomasOnline.Model.V3) -> Cmd.navigate("fantomas", "v3")
//            | ActiveTab.FantomasTab (FantomasTools.Client.FantomasOnline.Model.V4) -> Cmd.navigate("fantomas", "v4")
//            | ActiveTab.FantomasTab (FantomasTools.Client.FantomasOnline.Model.Preview) -> Cmd.navigate("fantomas", "preview")

        let cmd = Navigation.cmdForCurrentTab tab model

        nextModel, cmd
    | UpdateSourceCode code -> { model with SourceCode = code }, Cmd.none
    | ToggleSettings ->
        let m =
            { model with
                  SettingsOpen = not model.SettingsOpen }

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
        let cmd =
            //            let version =
//                match mode with
//                | FantomasOnline.Model.V2 -> "v2"
//                | FantomasOnline.Model.V3 -> "v3"
//                | FantomasOnline.Model.V4 -> "v4"
//                | FantomasOnline.Model.Preview -> "preview"
//
//            // preserve options from hash
//            let queryString =
//                let parts = window.location.hash.Split('?')
//                if Seq.length parts = 2 then
//                    parts.[1].Split('&')
//                    |> Seq.map(fun (kv:string) ->
//                        let pieces = kv.Split('=')
//                        pieces.[0], pieces.[1])
//                    |> Seq.toList
//                else
//                    []
//
//            Cmd.navigate("fantomas", version, queryString)
            let changeVersion (hashWithoutQuery: string) =
                let version m =
                    match m with
                    | FantomasOnline.Model.V2 -> "v2"
                    | FantomasOnline.Model.V3 -> "v3"
                    | FantomasOnline.Model.V4 -> "v4"
                    | FantomasOnline.Model.Preview -> "preview"

                let oldVersion = version model.FantomasModel.Mode
                let newVersion = version mode
                hashWithoutQuery.Replace(oldVersion, newVersion)

            Cmd.ofSub
                (fun dispatch ->
                    UrlTools.updateUrlBy changeVersion
                    dispatch (SelectTab(ActiveTab.FantomasTab(mode))))

        model, cmd
    | FantomasMsg fMsg ->
        let isActiveTab =
            match model.ActiveTab with
            | FantomasTab _ -> true
            | _ -> false

        let (fModel, fCmd) =
            FantomasOnline.State.update isActiveTab model.SourceCode fMsg model.FantomasModel

        { model with FantomasModel = fModel }, Cmd.map FantomasMsg fCmd
