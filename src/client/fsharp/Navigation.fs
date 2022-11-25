module FantomasTools.Client.Navigation

open FantomasTools.Client
open FantomasTools.Client.Model
open Elmish
open Feliz.Router

let cmdForCurrentTab tab model =
    if not (System.String.IsNullOrWhiteSpace model.SourceCode) then
        match tab with
        | HomeTab -> Cmd.none
        | ASTTab -> Cmd.ofMsg ASTViewer.Model.DoParse |> Cmd.map Msg.ASTMsg
        | OakTab -> Cmd.ofMsg OakViewer.Model.GetOak |> Cmd.map Msg.OakMsg
        | TriviaTab -> Cmd.ofMsg Trivia.Model.GetTrivia |> Cmd.map Msg.TriviaMsg
        | FantomasTab mode when (mode <> model.FantomasModel.Mode) ->
            Cmd.batch
                [ Cmd.map FantomasMsg (FantomasOnline.State.getOptionsCmd mode)
                  Cmd.map FantomasMsg (FantomasOnline.State.getVersionCmd mode) ]

        | FantomasTab _ when not (List.isEmpty model.FantomasModel.DefaultOptions) ->
            Cmd.ofMsg FantomasOnline.Model.Format |> Cmd.map FantomasMsg
        | FantomasTab _ -> Cmd.none
    else
        Cmd.none

let toHash =
    function
    | HomeTab -> "#/"
    | OakTab -> "#/oak"
    | TriviaTab -> "#/trivia"
    | ASTTab -> "#/ast"
    | FantomasTab FantomasOnline.Model.V4 -> "#/fantomas/v4"
    | FantomasTab FantomasOnline.Model.V5 -> "#/fantomas/v5"
    | FantomasTab FantomasOnline.Model.Main -> "#/fantomas/main"

let parseUrl segments =
    match segments with
    | [ "ast" ]
    | [ "ast"; Route.Query [ "data", _ ] ] -> ActiveTab.ASTTab
    | [ "oak" ]
    | [ "oak"; Route.Query [ "data", _ ] ] -> ActiveTab.OakTab
    | [ "trivia" ]
    | [ "trivia"; Route.Query [ "data", _ ] ] -> ActiveTab.TriviaTab
    | [ "fantomas"; "v4" ]
    | [ "fantomas"; "v4"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.V4)
    | [ "fantomas"; "main" ]
    | [ "fantomas"; "main"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.Main)
    | _ -> ActiveTab.HomeTab
