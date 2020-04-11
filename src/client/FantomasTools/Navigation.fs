module FantomasTools.Client.Navigation

open Elmish
open Elmish.Navigation
open Elmish.UrlParser
open FantomasTools.Client
open FantomasTools.Client.FantomasOnline.Model
open FantomasTools.Client.Model

let private route: Parser<ActiveTab -> _, _> =
    UrlParser.oneOf
        [ map ActiveTab.HomeTab (s "")
          map ActiveTab.TriviaTab (s "trivia")
          map ActiveTab.TokensTab (s "tokens")
          map ActiveTab.ASTTab (s "ast")
          map (ActiveTab.FantomasTab(Previous)) (s "fantomas" </> s "previous")
          map (ActiveTab.FantomasTab(Latest)) (s "fantomas" </> s "latest")
          map (ActiveTab.FantomasTab(Preview)) (s "fantomas" </> s "preview") ]

let parser: Browser.Types.Location -> ActiveTab option = parseHash route

let cmdForCurrentTab tab model =
    if not (System.String.IsNullOrWhiteSpace model.SourceCode) then
        match tab with
        | HomeTab -> Cmd.none
        | TokensTab -> Cmd.ofMsg (FSharpTokens.Model.GetTokens) |> Cmd.map Msg.FSharpTokensMsg
        | ASTTab -> Cmd.ofMsg (ASTViewer.Model.DoParse) |> Cmd.map Msg.ASTMsg
        | TriviaTab -> Cmd.ofMsg (Trivia.Model.GetTrivia) |> Cmd.map Msg.TriviaMsg
        | FantomasTab _ -> Cmd.none
    else
        Cmd.none

let urlUpdate (result: Option<ActiveTab>) model =
    match result with
    | Some tab ->
        let cmd = cmdForCurrentTab tab model
        let fantomasModel, fantomasCmd =
            match tab with
            | FantomasTab (ft) when (ft <> model.FantomasModel.Mode) ->
                FantomasOnline.State.init ft
            | _ ->
                model.FantomasModel, Cmd.none

        { model with
              ActiveTab = tab
              FantomasModel = fantomasModel },
        Cmd.batch
            [ cmd
              Cmd.map FantomasMsg fantomasCmd ]
    | None ->
        ({ model with ActiveTab = HomeTab }, Navigation.modifyUrl "#") // no matching route - go home

let toHash =
    function
    | HomeTab -> "#/"
    | TriviaTab -> "#/trivia"
    | TokensTab -> "#/tokens"
    | ASTTab -> "#/ast"
    | FantomasTab (Previous) -> "#/fantomas/previous"
    | FantomasTab (Latest) -> "#/fantomas/latest"
    | FantomasTab (Preview) -> "#/fantomas/preview"
