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
        { model with ActiveTab = tab }, Cmd.none
    | None ->
        ( { model with ActiveTab = HomeTab }, Navigation.modifyUrl "#" ) // no matching route - go home

let toHash =
    function
    | HomeTab -> "#/"
    | TriviaTab -> "#/trivia"
    | TokensTab -> "#/tokens"
    | ASTTab -> "#/ast"
    | FantomasTab -> "#/fantomas"