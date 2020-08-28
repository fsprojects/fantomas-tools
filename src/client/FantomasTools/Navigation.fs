module FantomasTools.Client.Navigation

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model
open FantomasTools.Client.Model

let private route: Parser<ActiveTab -> _, _> =
    oneOf [ map ActiveTab.HomeTab (s "")
            map ActiveTab.TriviaTab (s "trivia")
            map ActiveTab.TokensTab (s "tokens")
            map ActiveTab.ASTTab (s "ast")
            map (ActiveTab.FantomasTab(V2)) (s "fantomas" </> s "v2")
            map (ActiveTab.FantomasTab(V3)) (s "fantomas" </> s "v3")
            map (ActiveTab.FantomasTab(V4)) (s "fantomas" </> s "v4")
            map (ActiveTab.FantomasTab(Preview)) (s "fantomas" </> s "preview") ]

let parser: Browser.Types.Location -> ActiveTab option = parseHash route

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
        | FantomasTab (_) when (not (List.isEmpty model.FantomasModel.DefaultOptions)) ->
            Cmd.ofMsg (Format) |> Cmd.map Msg.FantomasMsg
        | FantomasTab _ -> Cmd.none
    else
        Cmd.none

let urlUpdate (result: Option<ActiveTab>) model =
    match result with
    | Some tab ->
        let cmd = cmdForCurrentTab tab model

        let fantomasModel, fantomasCmd =
            match tab with
            | FantomasTab (ft) when (ft <> model.FantomasModel.Mode) -> FantomasOnline.State.init ft
            | _ -> model.FantomasModel, Cmd.none

        { model with
              ActiveTab = tab
              FantomasModel = fantomasModel },
        Cmd.batch [ cmd
                    Cmd.map FantomasMsg fantomasCmd ]
    | None -> ({ model with ActiveTab = HomeTab }, Navigation.modifyUrl "#") // no matching route - go home

let toHash =
    function
    | HomeTab -> "#/"
    | TriviaTab -> "#/trivia"
    | TokensTab -> "#/tokens"
    | ASTTab -> "#/ast"
    | FantomasTab (V2) -> "#/fantomas/v2"
    | FantomasTab (V3) -> "#/fantomas/v3"
    | FantomasTab (V4) -> "#/fantomas/v4"
    | FantomasTab (Preview) -> "#/fantomas/preview"
