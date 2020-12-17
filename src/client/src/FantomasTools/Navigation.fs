module FantomasTools.Client.Navigation

open FantomasTools.Client
open FantomasTools.Client.Model
open Elmish
open Feliz.Router

let cmdForCurrentTab tab model =
    if not (System.String.IsNullOrWhiteSpace model.SourceCode) then
        match tab with
        | HomeTab -> Cmd.none
        | TokensTab ->
            Cmd.ofMsg (FSharpTokens.Model.GetTokens)
            |> Cmd.map Msg.FSharpTokensMsg
        | ASTTab ->
            Cmd.ofMsg (ASTViewer.Model.DoParse)
            |> Cmd.map Msg.ASTMsg
        | TriviaTab ->
            Cmd.ofMsg (Trivia.Model.GetTrivia)
            |> Cmd.map Msg.TriviaMsg
        | FantomasTab mode when (mode <> model.FantomasModel.Mode) ->
            Cmd.batch
                [ Cmd.map FantomasMsg (FantomasOnline.State.getOptionsCmd mode)
                  Cmd.map FantomasMsg (FantomasOnline.State.getVersionCmd mode) ]

        | FantomasTab _ -> Cmd.none
    else
        Cmd.none

let toHash =
    function
    | HomeTab -> "#/"
    | TriviaTab -> "#/trivia"
    | TokensTab -> "#/tokens"
    | ASTTab -> "#/ast"
    | FantomasTab (FantomasOnline.Model.V2) -> "#/fantomas/v2"
    | FantomasTab (FantomasOnline.Model.V3) -> "#/fantomas/v3"
    | FantomasTab (FantomasOnline.Model.V4) -> "#/fantomas/v4"
    | FantomasTab (FantomasOnline.Model.Preview) -> "#/fantomas/preview"

let parseUrl segments =
    match segments with
    | [ "tokens" ]
    | [ "tokens"; Route.Query [ "data", _ ] ] -> ActiveTab.TokensTab
    | [ "ast" ]
    | [ "ast"; Route.Query [ "data", _ ] ] -> ActiveTab.ASTTab
    | [ "trivia" ]
    | [ "trivia"; Route.Query [ "data", _ ] ] -> ActiveTab.TriviaTab
    | [ "fantomas"; "v2" ]
    | [ "fantomas"; "v2"; Route.Query [ "data", _ ] ] ->
        ActiveTab.FantomasTab(FantomasTools.Client.FantomasOnline.Model.V2)
    | [ "fantomas"; "v3" ]
    | [ "fantomas"; "v3"; Route.Query [ "data", _ ] ] ->
        ActiveTab.FantomasTab(FantomasTools.Client.FantomasOnline.Model.V3)
    | [ "fantomas"; "v4" ]
    | [ "fantomas"; "v4"; Route.Query [ "data", _ ] ] ->
        ActiveTab.FantomasTab(FantomasTools.Client.FantomasOnline.Model.V4)
    | [ "fantomas"; "preview" ]
    | [ "fantomas"; "preview"; Route.Query [ "data", _ ] ] ->
        ActiveTab.FantomasTab(FantomasTools.Client.FantomasOnline.Model.Preview)
    | _ -> ActiveTab.HomeTab
