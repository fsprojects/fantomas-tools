module FantomasTools.Client.View

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Model
open Reactstrap

let private navigation =
    let title = sprintf "Fantomas tools"

    Navbar.navbar
        [ Navbar.Light true
          Navbar.Custom [ ClassName "bg-light" ] ]
        [ NavbarBrand.navbarBrand [ NavbarBrand.Custom [ ClassName "py-0" ] ] [ str title ]
          div [ ClassName "navbar-text py1" ]
              [ Button.button
                  [ Button.Custom
                      [ Href "https://github.com/nojaf/trivia-tool"
                        Target "_blank"
                        ClassName "text-white" ]
                    Button.Color Dark ]
                    [ i [ ClassName "fab fa-github mr-1 mt-1" ] []
                      str "GitHub" ] ] ]

let private editor model dispatch =
    Col.col
        [ Col.Xs(Col.mkCol !^5)
          Col.Custom [ ClassName "border-right h-100 d-flex flex-column" ] ]
        [ div
            [ Id "source"
              ClassName "flex-grow-1" ]
              [ Editor.editor
                  [ Editor.OnChange(UpdateSourceCode >> dispatch)
                    Editor.Value model.SourceCode ] ] ]

let private homeTab =
    Jumbotron.jumbotron []
        [ h1 [ ClassName "display-3" ] [ str "Fantomas tool" ]
          p [ ClassName "lead" ] [ str "Welcome at the Fantomas Tools!" ]
          p []
              [ str
                  "if you plan on using these tools extensively, consider cloning the repository and run everything locally." ] ]

let private tabs model dispatch =
    let activeTab =
        match model.ActiveTab with
        | HomeTab -> homeTab
        | TriviaTab ->
            let triviaDispatch tMsg = dispatch (TriviaMsg tMsg)
            Trivia.View.view model.TriviaModel triviaDispatch
        | TokensTab ->
            let tokensDispatch tMsg = dispatch (FSharpTokensMsg tMsg)
            FSharpTokens.View.view model.FSharpTokensModel tokensDispatch
        | ASTTab ->
            let astDispatch aMsg = dispatch (ASTMsg aMsg)
            ASTViewer.View.view model.ASTModel astDispatch
        | FantomasTab _ ->
            let fantomasDispatch fMsg = dispatch (FantomasMsg fMsg)
            FantomasOnline.View.view model.FantomasModel fantomasDispatch

    let onNavItemClick tab (ev: Event) =
        ev.preventDefault()
        dispatch (SelectTab tab)

    let navItem tab label isActive =
        NavItem.navItem [ NavItem.Custom [ OnClick(onNavItemClick tab); Key label ] ]
            [ NavLink.navLink
                [ NavLink.Custom [ Href(Navigation.toHash tab) ]
                  NavLink.Active isActive ] [ str label ] ]

    let isFantomasTab = function | FantomasTab _ -> true | _ -> false

    let navItems =
        [ navItem HomeTab "Home" (model.ActiveTab = HomeTab)
          navItem TokensTab "FSharp Tokens" (model.ActiveTab = HomeTab)
          navItem ASTTab "AST" (model.ActiveTab = HomeTab)
          navItem TriviaTab "Trivia" (model.ActiveTab = HomeTab)
          navItem (FantomasTab FantomasTools.Client.FantomasOnline.Model.Preview) "Fantomas" (isFantomasTab model.ActiveTab) ]

    div [ ClassName "col-7 h-100 d-flex flex-column" ]
        [ Nav.nav
            [ Nav.Tabs true
              Nav.Custom [ ClassName "" ] ] [ ofList navItems ]
          div [ Id "tab-content" ] [ activeTab ] ]

let view model dispatch =
    div [ ClassName "d-flex flex-column h-100" ]
        [ navigation
          main [ ClassName "flex-grow-1" ]
              [ Row.row [ Row.Custom [ ClassName "h-100 no-gutters" ] ]
                    [ editor model dispatch
                      tabs model dispatch ] ] ]
