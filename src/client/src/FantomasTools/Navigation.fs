module FantomasTools.Client.Navigation

open FantomasTools.Client
open FantomasTools.Client.Model

open Elmish
//open Elmish.Navigation
//open Elmish.UrlParser
//open FantomasTools.Client
open Feliz.Router
//open FantomasTools.Client.FantomasOnline.Model
//open FantomasTools.Client.Model
//
//let private route: Parser<ActiveTab -> _, _> =
//    oneOf [ map ActiveTab.HomeTab (s "")
//            map ActiveTab.TriviaTab (s "trivia")
//            map ActiveTab.TokensTab (s "tokens")
//            map ActiveTab.ASTTab (s "ast")
//            map (ActiveTab.FantomasTab(V2)) (s "fantomas" </> s "v2")
//            map (ActiveTab.FantomasTab(V3)) (s "fantomas" </> s "v3")
//            map (ActiveTab.FantomasTab(V4)) (s "fantomas" </> s "v4")
//            map (ActiveTab.FantomasTab(Preview)) (s "fantomas" </> s "preview") ]
//
//let parser: Browser.Types.Location -> ActiveTab option = parseHash route
//
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
            Cmd.map FantomasMsg (FantomasOnline.State.getOptionsCmd mode)
        //        | FantomasTab  when (not (List.isEmpty model.FantomasModel.DefaultOptions)) ->
//            Cmd.ofMsg (Format) |> Cmd.map Msg.FantomasMsg
        | FantomasTab _ -> Cmd.ofMsg (FantomasMsg(FantomasOnline.Model.Msg.Format))
    else
        Cmd.none
//
//let urlUpdate (result: Option<ActiveTab>) model =
//    match result with
//    | Some tab ->
//        let cmd = cmdForCurrentTab tab model
//
//        let fantomasModel, fantomasCmd =
//            match tab with
//            | FantomasTab (ft) when (ft <> model.FantomasModel.Mode) -> FantomasOnline.State.init ft
//            | _ -> model.FantomasModel, Cmd.none
//
//        { model with
//              ActiveTab = tab
//              FantomasModel = fantomasModel },
//        Cmd.batch [ cmd
//                    Cmd.map FantomasMsg fantomasCmd ]
//    | None -> ({ model with ActiveTab = HomeTab }, Navigation.modifyUrl "#") // no matching route - go home
//

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
