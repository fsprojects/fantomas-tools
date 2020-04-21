module FantomasTools.Client.View

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Model
open Reactstrap

let private navigation dispatch =
    let title = sprintf "Fantomas tools"

    Navbar.navbar
        [ Navbar.Light true
          Navbar.Custom [ ClassName "bg-light" ] ]
        [ NavbarBrand.navbarBrand [ NavbarBrand.Custom [ ClassName "py-0" ] ] [ str title ]
          div [ ClassName "navbar-text py1" ]
              [ Button.button
                  [ Button.Custom
                      [ Href "https://github.com/nojaf/fantomas-tools"
                        Target "_blank"
                        ClassName "text-white" ]
                    Button.Color Dark ]
                    [ i [ ClassName "fab fa-github mr-1 mt-1" ] []
                      str "GitHub" ]
                Button.button
                    [ Button.Custom
                        [ ClassName "ml-2 pointer"
                          OnClick(fun _ ->
                              printfn "click"
                              dispatch ToggleSettings) ] ] [ i [ ClassName "fas fa-sliders-h" ] [] ] ] ]

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

let private settings model dispatch inner =
    let className = sprintf "settings %s" (if model.SettingsOpen then "open" else "")
    div [ ClassName className ]
        [ div [ ClassName "inner" ]
              [ h1 [ ClassName "text-center" ]
                    [ i
                        [ ClassName "fas fa-times close"
                          OnClick(fun _ -> dispatch ToggleSettings) ] []
                      str "Settings" ]
                inner ] ]

let private tabs model dispatch =
    let activeTab, settingsForTab, commands =
        match model.ActiveTab with
        | HomeTab -> homeTab, null, null
        | TriviaTab ->
            let triviaDispatch tMsg = dispatch (TriviaMsg tMsg)
            Trivia.View.view model.TriviaModel triviaDispatch, null, null
        | TokensTab ->
            let tokensDispatch tMsg = dispatch (FSharpTokensMsg tMsg)
            FSharpTokens.View.view model.FSharpTokensModel tokensDispatch,
            FSharpTokens.View.settings model.FSharpTokensModel tokensDispatch,
            FSharpTokens.View.commands tokensDispatch
        | ASTTab ->
            let astDispatch aMsg = dispatch (ASTMsg aMsg)
            ASTViewer.View.view model.ASTModel astDispatch,
            ASTViewer.View.settings model.ASTModel astDispatch,
            ASTViewer.View.commands astDispatch
        | FantomasTab _ ->
            let fantomasDispatch fMsg = dispatch (FantomasMsg fMsg)
            FantomasOnline.View.view model.SourceCode model.FantomasModel fantomasDispatch, null, null

    let onNavItemClick tab (ev: Event) =
        ev.preventDefault ()
        dispatch (SelectTab tab)

    let navItem tab label isActive =
        NavItem.navItem
            [ NavItem.Custom
                [ OnClick(onNavItemClick tab)
                  Key label ] ]
            [ NavLink.navLink
                [ NavLink.Custom [ Href(Navigation.toHash tab) ]
                  NavLink.Active isActive ] [ str label ] ]

    let isFantomasTab =
        function
        | FantomasTab _ -> true
        | _ -> false

    let navItems =
        [ navItem HomeTab "Home" (model.ActiveTab = HomeTab)
          navItem TokensTab "FSharp Tokens" (model.ActiveTab = TokensTab)
          navItem ASTTab "AST" (model.ActiveTab = ASTTab)
          navItem TriviaTab "Trivia" (model.ActiveTab = TriviaTab)
          navItem (FantomasTab FantomasTools.Client.FantomasOnline.Model.Preview) "Fantomas"
              (isFantomasTab model.ActiveTab) ]

    div [ ClassName "col-7 h-100 d-flex flex-column" ]
        [ settings model dispatch settingsForTab
          Nav.nav
              [ Nav.Tabs true
                Nav.Custom [ ClassName "" ] ] [ ofList navItems ]
          div [ Id "tab-content" ] [
              activeTab
              div [ Id "commands" ] [ commands ]
          ] ]

let view model dispatch =
    div [ ClassName "d-flex flex-column h-100" ]
        [ navigation dispatch
          main [ ClassName "flex-grow-1" ]
              [ Row.row [ Row.Custom [ ClassName "h-100 no-gutters" ] ]
                    [ editor model dispatch
                      tabs model dispatch ] ] ]
