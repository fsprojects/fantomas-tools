module FantomasTools.Client.Navigation

open Elmish
open Elmish.UrlParser
open Elmish.Navigation

open FantomasTools.Client.Model

let private route : Parser<ActiveTab->_,_> =
    UrlParser.oneOf [
        map ActiveTab.HomeTab (s "")
        map ActiveTab.TriviaTab (s "trivia")
        map ActiveTab.TokensTab (s "tokens")
        map ActiveTab.ASTTab (s "ast")
        map ActiveTab.FantomasTab (s "fantomas")
    ]
let parser : (Browser.Types.Location -> ActiveTab option) = parseHash route

let urlUpdate (result:Option<ActiveTab>) model =
    match result with
    | Some tab ->
        let cmd =
            if not (System.String.IsNullOrWhiteSpace model.SourceCode) then
                match tab with
                | TriviaTab -> Cmd.ofMsg (FantomasTools.Client.Trivia.Model.GetTrivia) |> Cmd.map Msg.TriviaMsg
                | TokensTab -> Cmd.ofMsg (FantomasTools.Client.FSharpTokens.Model.GetTokens) |> Cmd.map Msg.FSharpTokensMsg
                | ASTTab -> Cmd.ofMsg (FantomasTools.Client.ASTViewer.Model.DoParse) |> Cmd.map Msg.ASTMsg
                | _ -> Cmd.none
            else
                Cmd.none

        { model with ActiveTab = tab }, cmd
    | None ->
        ( { model with ActiveTab = HomeTab }, Navigation.modifyUrl "#" ) // no matching route - go home

let toHash =
    function
    | HomeTab -> "#/"
    | TriviaTab -> "#/trivia"
    | TokensTab -> "#/tokens"
    | ASTTab -> "#/ast"
    | FantomasTab -> "#/fantomas"