module FantomasTools.Client.State

open FantomasTools.Client
open Elmish
open FantomasTools.Client.Model
open Thoth.Json
open Feliz.Router

let private getCodeFromUrl () =
    UrlTools.restoreModelFromUrl (Decode.object (fun get -> get.Required.Field "code" Decode.string)) ""

let private getIsFsiFileFromUrl () =
    UrlTools.restoreModelFromUrl (Decode.object (fun get -> get.Required.Field "isFsi" Decode.bool)) false

let init _ =
    let sourceCode = getCodeFromUrl ()
    let isFsiFile = getIsFsiFileFromUrl ()
    let currentTab = Navigation.parseUrl (Router.currentUrl ())

    let astModel, astCmd = ASTViewer.State.init (currentTab = ASTTab)
    let oakModel, oakCmd = OakViewer.State.init (currentTab = OakTab)

    let fantomasModel, fantomasCmd =
        let tab =
            match currentTab with
            | ActiveTab.FantomasTab ft -> ft
            | _ -> FantomasTools.Client.FantomasOnline.Model.Main

        FantomasOnline.State.init tab

    let model =
        { ActiveTab = currentTab
          SourceCode = sourceCode
          SettingsOpen = false
          IsFsi = isFsiFile
          OakModel = oakModel
          ASTModel = astModel
          FantomasModel = fantomasModel }

    let initialCmd = Navigation.cmdForCurrentTab currentTab model
    //
    let cmd =
        Cmd.batch
            [ Cmd.map ASTMsg astCmd
              Cmd.map OakMsg oakCmd
              Cmd.map FantomasMsg fantomasCmd
              initialCmd ]

    model, cmd

let private reload model =
    if not model.SettingsOpen then
        match model.ActiveTab with
        | ASTTab -> Cmd.ofMsg FantomasTools.Client.ASTViewer.Model.DoParse |> Cmd.map ASTMsg
        | FantomasTab _ ->
            Cmd.ofMsg FantomasTools.Client.FantomasOnline.Model.Format
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

        let cmd = Navigation.cmdForCurrentTab tab model

        nextModel, cmd
    | UpdateSourceCode code -> { model with SourceCode = code }, Cmd.none
    | ToggleSettings ->
        let m =
            { model with
                SettingsOpen = not model.SettingsOpen }

        m, reload m
    | OakMsg(OakViewer.Model.Msg.SetFsiFile isFsiFile) -> { model with IsFsi = isFsiFile }, Cmd.none
    | OakMsg oMsg ->
        let oModel, oCmd =
            OakViewer.State.update model.SourceCode model.IsFsi oMsg model.OakModel

        { model with OakModel = oModel }, Cmd.map OakMsg oCmd
    | ASTMsg(ASTViewer.Model.Msg.SetFsiFile isFsiFile) -> { model with IsFsi = isFsiFile }, Cmd.none
    | ASTMsg aMsg ->
        let aModel, aCmd =
            ASTViewer.State.update model.SourceCode model.IsFsi aMsg model.ASTModel

        { model with ASTModel = aModel }, Cmd.map ASTMsg aCmd
    | FantomasMsg(FantomasOnline.Model.Msg.SetFsiFile isFsiFile) -> { model with IsFsi = isFsiFile }, Cmd.none
    | FantomasMsg(FantomasOnline.Model.ChangeMode mode) ->
        let cmd =
            let changeVersion (hashWithoutQuery: string) =
                let version m =
                    match m with
                    | FantomasOnline.Model.V4 -> "v4"
                    | FantomasOnline.Model.V5 -> "v5"
                    | FantomasOnline.Model.Main -> "main"
                    | FantomasOnline.Model.Preview -> "preview"

                let oldVersion = version model.FantomasModel.Mode
                let newVersion = version mode
                hashWithoutQuery.Replace(oldVersion, newVersion)

            Cmd.ofSub (fun dispatch ->
                UrlTools.updateUrlBy changeVersion
                dispatch (SelectTab(ActiveTab.FantomasTab(mode))))

        model, cmd
    | FantomasMsg fMsg ->
        let isActiveTab =
            match model.ActiveTab with
            | FantomasTab _ -> true
            | _ -> false

        let fModel, fCmd =
            FantomasOnline.State.update isActiveTab model.SourceCode model.IsFsi fMsg model.FantomasModel

        { model with FantomasModel = fModel }, Cmd.map FantomasMsg fCmd
