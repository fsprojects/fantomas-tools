module FantomasTools.Client.Navigation

open FantomasTools.Client
open FantomasTools.Client.Model
open Elmish
open Feliz.Router

let cmdForCurrentTab tab (model: Model) =
    let noSourceCode = System.String.IsNullOrWhiteSpace model.Bubble.SourceCode

    match tab with
    | HomeTab -> Cmd.none
    | ASTTab ->
        Fable.Core.JS.console.log "ast tab Navigation.fs"
        Cmd.ofMsg ASTViewer.Model.DoParse |> Cmd.map Msg.ASTMsg
    | OakTab -> Cmd.ofMsg OakViewer.Model.GetOak |> Cmd.map Msg.OakMsg
    | FantomasTab mode ->
        if noSourceCode then
            Cmd.none
        elif mode <> model.FantomasModel.Mode then
            Cmd.batch
                [ Cmd.map FantomasMsg (FantomasOnline.State.getOptionsCmd mode)
                  Cmd.map FantomasMsg (FantomasOnline.State.getVersionCmd mode) ]
        elif not (List.isEmpty model.FantomasModel.DefaultOptions) then
            Cmd.ofMsg FantomasOnline.Model.Format |> Cmd.map FantomasMsg
        else
            Cmd.none

let toHash =
    function
    | HomeTab -> "#/"
    | OakTab -> "#/oak"
    | ASTTab -> "#/ast"
    | FantomasTab FantomasOnline.Model.V4 -> "#/fantomas/v4"
    | FantomasTab FantomasOnline.Model.V5 -> "#/fantomas/v5"
    | FantomasTab FantomasOnline.Model.V6 -> "#/fantomas/v6"
    | FantomasTab FantomasOnline.Model.Main -> "#/fantomas/main"
    | FantomasTab FantomasOnline.Model.Preview -> "#/fantomas/preview"

let parseUrl segments =
    match segments with
    | [ "ast" ]
    | [ "ast"; Route.Query [ "data", _ ] ] -> ActiveTab.ASTTab
    | [ "oak" ]
    | [ "oak"; Route.Query [ "data", _ ] ] -> ActiveTab.OakTab
    | [ "fantomas"; "v4" ]
    | [ "fantomas"; "v4"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.V4)
    | [ "fantomas"; "main" ]
    | [ "fantomas"; "main"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.Main)
    | [ "fantomas"; "preview" ]
    | [ "fantomas"; "preview"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.Preview)
    | _ -> ActiveTab.HomeTab
