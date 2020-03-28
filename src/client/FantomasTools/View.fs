module FantomasTools.Client.View

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Reactstrap
open FantomasTools.Client
open FantomasTools.Client.Model

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
        [ Col.Xs(Col.mkCol !^4)
          Col.Custom [ ClassName "border-right h-100 d-flex flex-column" ] ]
        [ div
            [ Id "source"
              ClassName "flex-grow-1" ]
              [ Editor.editor
                  [ Editor.OnChange(UpdateSourceCode >> dispatch)
                    Editor.Value model.SourceCode ] ] ]

let private tabs model dispatch =
    let activeTab =
        match model.ActiveTab with
        | TriviaTab ->
            let triviaDispatch tMsg = dispatch (TriviaMsg tMsg)
            FantomasTools.Client.Trivia.View.view model.TriviaModel triviaDispatch
        | _ ->
            str "other tab not present yet"

    let onNavItemClick tab (ev:Event) =
        ev.preventDefault()
        dispatch (SelectTab tab)

    let navItem tab label =
        let isActive = model.ActiveTab = tab
        NavItem.navItem [ NavItem.Custom [ OnClick (onNavItemClick tab) ] ] [
            NavLink.navLink [ NavLink.Custom [ Href "#"]; NavLink.Active isActive ] [ str label ]
        ]

    let navItems =
        [
            navItem TokensTab "FSharp Tokens"
            navItem ASTTab "AST"
            navItem TriviaTab "Trivia"
            navItem FantomasTab "Fantomas"
        ]

    Col.col [ Col.Xs(Col.mkCol !^8) ] [
        Nav.nav
            [ Nav.Tabs true
              Nav.Custom [ ClassName "" ] ] [ ofList navItems ]
        div [ Id "tab-content" ] [ activeTab ]
    ]


let view model dispatch =



    div [ ClassName "d-flex flex-column h-100" ]
        [ navigation
          main [ ClassName "flex-grow-1" ]
              [ Row.row [ Row.Custom [ ClassName "h-100 no-gutters" ] ]
                    [ editor model dispatch
                      tabs model dispatch ] ] ]

