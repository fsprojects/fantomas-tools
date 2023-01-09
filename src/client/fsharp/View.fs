module FantomasTools.Client.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Model
open FantomasTools.Client.Editor
open Reactstrap

let navigation dispatch =
    let title = "Fantomas tools"

    Navbar.navbar [ Navbar.Light true; Navbar.Custom [ ClassName "bg-light" ] ] [
        NavbarBrand.navbarBrand [ NavbarBrand.Custom [ ClassName "py-0 my-0 h1" ] ] [
            img [ Src "./fantomas_logo.png"; ClassName "mr-3" ]
            str title
        ]
        div [ ClassName "navbar-text py1" ] [
            Button.button [
                Button.Custom [
                    Href "https://github.com/sponsors/nojaf"
                    Target "_blank"
                    Id "sponsor-button"
                ]
                Button.Color Success
                Button.Outline true
            ] [ i [ ClassName "far fa-heart mr-1 mt-1 text-danger" ] []; str "Sponsor" ]
            Button.button [
                Button.Custom [
                    Href "https://github.com/fsprojects/fantomas-tools"
                    Target "_blank"
                    ClassName "text-white ml-2"
                ]
                Button.Color Dark
            ] [ i [ ClassName "fab fa-github mr-1 mt-1" ] []; str "GitHub" ]
            Button.button [
                Button.Custom [
                    Href "https://www.youtube.com/playlist?list=PLvw_J2kfZCX3Mf6tEbIPZXbzJOD1VGl4K"
                    Target "_blank"
                    ClassName "text-white ml-2"
                    Style [ Background "red"; BorderColor "red" ]
                ]
            ] [ i [ ClassName "fab fa-youtube mr-1 mt-1" ] []; str "YouTube" ]
            Button.button [
                Button.Custom [
                    Href "https://fsprojects.github.io/fantomas/reference/fsharp-compiler-syntax.html"
                    Target "_blank"
                    ClassName "text-white ml-2"
                    Style [ Background "grey"; BorderColor "grey" ]
                ]
            ] [ i [ ClassName "fa fa-book mr-1 mt-1" ] []; str "Fantomas.FCS Docs" ]
            Button.button [
                Button.Custom [ ClassName "ml-2 pointer"; OnClick(fun _ -> dispatch ToggleSettings) ]
            ] [ i [ ClassName "fas fa-sliders-h" ] [] ]
        ]
    ]

let editor model dispatch =
    Col.col [
        Col.Xs(Col.mkCol !^ 5)
        Col.Custom [ ClassName "border-right h-100 d-flex flex-column" ]
    ] [
        div [ Id "source"; ClassName "flex-grow-1" ] [
            Editor false [
                MonacoEditorProp.OnChange(UpdateSourceCode >> dispatch)
                MonacoEditorProp.DefaultValue model.SourceCode
                MonacoEditorProp.Options(MonacoEditorProp.rulerOption model.FantomasModel.MaxLineLength)
            ]
        ]
    ]
//
let private homeTab =
    Jumbotron.jumbotron [ Jumbotron.Custom [ ClassName "bg-light" ] ] [
        div [ ClassName "d-flex align-items-center mb-4" ] [
            img [ Src "./logo.png" ]
            h1 [ ClassName "display-3 ml-4" ] [ str "Fantomas tool" ]
        ]
        p [ ClassName "lead" ] [ str "Welcome to the Fantomas Tools!" ]
        p [] [
            str "if you plan on using these tools extensively, consider cloning the "
            a [ Href "https://github.com/fsprojects/fantomas-tools"; Target "_blank" ] [ str "repository" ]
            str " and run everything locally."
        ]
        p [] [
            str "Discover more about Fantomas in our "
            a [
                Href "https://fsprojects.github.io/fantomas/docs/index.html"
                Target "_blank"
            ] [ str "documentation" ]
            str "."
        ]
    ]

let private settings model dispatch inner =
    let className = sprintf "settings %s" (if model.SettingsOpen then "open" else "")

    div [
        ClassName className
        OnClick(fun ev ->
            if ev.target?className = "settings open" then
                dispatch ToggleSettings)
    ] [
        div [ ClassName "inner" ] [
            h1 [ ClassName "text-center" ] [
                i [ ClassName "fas fa-times close"; OnClick(fun _ -> dispatch ToggleSettings) ] []
                str "Settings"
            ]
            inner
        ]
    ]

let tabs (model: Model) dispatch =
    let activeTab, settingsForTab, commands =
        match model.ActiveTab with
        | HomeTab -> homeTab, null, null
        | ASTTab ->
            let astDispatch aMsg = dispatch (ASTMsg aMsg)

            ASTViewer.View.view model.ASTModel astDispatch,
            ASTViewer.View.settings model.IsFsi model.ASTModel astDispatch,
            ASTViewer.View.commands astDispatch
        | OakTab ->
            let oakDispatch oMsg = dispatch (OakMsg oMsg)

            OakViewer.View.view model.OakModel oakDispatch,
            OakViewer.View.settings model.IsFsi model.OakModel oakDispatch,
            OakViewer.View.commands oakDispatch

        | FantomasTab _ ->
            let fantomasDispatch fMsg = dispatch (FantomasMsg fMsg)

            FantomasOnline.View.view model.IsFsi model.FantomasModel,
            FantomasOnline.View.settings model.IsFsi model.FantomasModel fantomasDispatch,
            FantomasOnline.View.commands model.SourceCode model.IsFsi model.FantomasModel fantomasDispatch

    let navItem tab label isActive =
        let href =
            let page = Navigation.toHash tab

            let query =
                let hash = Browser.Dom.window.location.hash

                if hash.Contains("?") then
                    sprintf "?%s" (hash.Split('?').[1])
                else
                    ""

            sprintf "%s%s" page query

        NavItem.navItem [ NavItem.Custom [ Key label ] ] [
            NavLink.navLink [ NavLink.Custom [ Href href ]; NavLink.Active isActive ] [ str label ]
        ]

    let isFantomasTab =
        function
        | FantomasTab _ -> true
        | _ -> false

    let navItems = [
        navItem HomeTab "Home" (model.ActiveTab = HomeTab)
        navItem ASTTab "AST" (model.ActiveTab = ASTTab)
        navItem OakTab "Oak" (model.ActiveTab = OakTab)
        navItem (FantomasTab FantomasTools.Client.FantomasOnline.Model.Main) "Fantomas" (isFantomasTab model.ActiveTab)
    ]

    div [ ClassName "col-7 h-100" ] [
        settings model dispatch settingsForTab
        Nav.nav [ Nav.Tabs true; Nav.Custom [ ClassName "" ] ] [ ofList navItems ]
        div [ Id "tab-content" ] [ activeTab; div [ Id "commands" ] [ commands ] ]
    ]
