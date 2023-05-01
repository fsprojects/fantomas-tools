module FantomasTools.Client.State

open FantomasTools.Client
open Elmish
open FantomasTools.Client.Model
open Thoth.Json
open Feliz.Router

// TODO: decode and encode the bubble model as much as possible

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
          SettingsOpen = false
          Bubble =
            { SourceCode = sourceCode
              IsFsi = isFsiFile
              Defines = ""
              ResultCode = ""
              Diagnostics = Array.empty
              IsLoading = false }
          OakModel = oakModel
          ASTModel = astModel
          FantomasModel = fantomasModel }

    let cmd =
        Cmd.batch
            [ Cmd.map ASTMsg astCmd
              Cmd.map OakMsg oakCmd
              Cmd.map FantomasMsg fantomasCmd ]

    model, cmd

let private reload model =
    if not model.SettingsOpen then
        match model.ActiveTab with
        | ASTTab -> Cmd.ofMsg FantomasTools.Client.ASTViewer.Model.DoParse |> Cmd.map ASTMsg
        | FantomasTab _ ->
            Cmd.ofMsg FantomasTools.Client.FantomasOnline.Model.Format
            |> Cmd.map FantomasMsg
        | OakTab -> Cmd.ofMsg FantomasTools.Client.OakViewer.Model.GetOak |> Cmd.map OakMsg
        | _ -> Cmd.none
    else
        Cmd.none

let update msg model =
    sprintf "%A" msg
    |> fun msg ->
        let msg =
            if msg.Length > 300 then
                msg.Substring(0, 297) + "..."
            else
                msg

        Fable.Core.JS.console.log msg

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
    | UpdateSourceCode code ->
        let bubble = { model.Bubble with SourceCode = code }
        { model with Bubble = bubble }, Cmd.none
    | ToggleSettings ->
        let m =
            { model with
                SettingsOpen = not model.SettingsOpen }

        m, reload m

    | ASTMsg(ASTViewer.Model.Msg.Bubble bubbleMsg)
    | OakMsg(OakViewer.Model.Msg.Bubble bubbleMsg)
    | FantomasMsg(FantomasOnline.Model.Msg.Bubble bubbleMsg) ->
        let bubble, cmd =
            match bubbleMsg with
            | SetSourceCode code -> { model.Bubble with SourceCode = code }, Cmd.none
            | SetFsi isFsiFile -> { model.Bubble with IsFsi = isFsiFile }, Cmd.none
            | SetDefines defines -> { model.Bubble with Defines = defines }, Cmd.none
            | SetResultCode code -> { model.Bubble with ResultCode = code }, Cmd.none
            | SetDiagnostics diagnostics ->
                { model.Bubble with
                    Diagnostics = diagnostics },
                Cmd.none
            | HighLight hlr -> model.Bubble, Cmd.ofEffect (Editor.selectRange hlr)
            | SetIsLoading isLoading ->
                { model.Bubble with
                    IsLoading = isLoading },
                Cmd.none

        { model with Bubble = bubble }, cmd

    | OakMsg oMsg ->
        let oModel, oCmd = OakViewer.State.update model.Bubble oMsg model.OakModel

        { model with OakModel = oModel }, Cmd.map OakMsg oCmd

    | ASTMsg aMsg ->
        let aModel, aCmd = ASTViewer.State.update model.Bubble aMsg model.ASTModel

        { model with ASTModel = aModel }, Cmd.map ASTMsg aCmd

    | FantomasMsg(FantomasOnline.Model.ChangeMode mode) ->
        let cmd =
            let changeVersion (hashWithoutQuery: string) =
                let version m =
                    match m with
                    | FantomasOnline.Model.V4 -> "v4"
                    | FantomasOnline.Model.V5 -> "v5"
                    | FantomasOnline.Model.V6 -> "v6"
                    | FantomasOnline.Model.Main -> "main"
                    | FantomasOnline.Model.Preview -> "preview"

                let oldVersion = version model.FantomasModel.Mode
                let newVersion = version mode
                hashWithoutQuery.Replace(oldVersion, newVersion)

            Cmd.ofEffect (fun dispatch ->
                UrlTools.updateUrlBy changeVersion
                dispatch (SelectTab(ActiveTab.FantomasTab(mode))))

        model, cmd
    | FantomasMsg fMsg ->
        let isActiveTab =
            match model.ActiveTab with
            | FantomasTab _ -> true
            | _ -> false

        let fModel, fCmd =
            FantomasOnline.State.update isActiveTab model.Bubble fMsg model.FantomasModel

        { model with FantomasModel = fModel }, Cmd.map FantomasMsg fCmd
