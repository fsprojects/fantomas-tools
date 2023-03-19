module FantomasTools.Client.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Model
open FantomasTools.Client.Editor

let private baseUrl: string = emitJsExpr () "import.meta.env.BASE_URL"

let navigation dispatch =
    let title = "Fantomas tools"

    nav [ ClassName $"{Style.Navbar} {Style.BgLight}" ] [
        div [ ClassName $"{Style.ContainerFluid}" ] [
            a [
                ClassName $"{Style.NavbarBrand} {Style.Py0} {Style.My1} {Style.H1}"
                Href baseUrl
                Target "_self"
            ] [ img [ Src "./fantomas_logo.png"; ClassName $"{Style.Me3}" ]; str title ]
            div [ ClassName $"{Style.NavbarText} {Style.Py1}" ] [
                a [
                    ClassName $"{Style.Btn} {Style.BtnOutlineSuccess} {Style.Me2}"
                    Href "https://github.com/sponsors/nojaf"
                    Target "_blank"
                    Id "sponsor-button"
                ] [
                    i [ ClassName $"far fa-heart {Style.Me1} {Style.Mt1} {Style.TextDanger}" ] []
                    str "Sponsor"
                ]
                a [
                    ClassName $"{Style.Btn} {Style.BtnDark} {Style.TextWhite} {Style.Me2}"
                    Href "https://github.com/fsprojects/fantomas-tools"
                    Target "_blank"
                ] [ i [ ClassName $"fab fa-github {Style.Me1} {Style.Mt1}" ] []; str "GitHub" ]
                a [
                    ClassName $"{Style.Btn} {Style.TextWhite} {Style.Me2}"
                    Props.Style [ Background "red"; BorderColor "red" ]
                    Href "https://www.youtube.com/playlist?list=PLvw_J2kfZCX3Mf6tEbIPZXbzJOD1VGl4K"
                    Target "_blank"
                ] [ i [ ClassName $"fab fa-youtube {Style.Me1} {Style.Mt1}" ] []; str "YouTube" ]
                a [
                    ClassName $"{Style.Btn} {Style.TextWhite} {Style.Me2}"
                    Href "https://fsprojects.github.io/fantomas/reference/fsharp-compiler-syntax.html"
                    Target "_blank"
                    Props.Style [ Background "grey"; BorderColor "grey" ]
                ] [
                    i [ ClassName $"fa fa-book {Style.Me1} {Style.Mt1}" ] []
                    str "Fantomas.FCS Docs"
                ]
                a [
                    ClassName $"{Style.Btn} {Style.BtnSecondary} {Style.TextWhite} {Style.Me2} {Style.Pointer}"
                    OnClick(fun _ -> dispatch ToggleSettings)
                ] [ i [ ClassName "fas fa-sliders-h" ] [] ]
            ]
        ]
    ]

let editor model dispatch =
    div [
        ClassName $"{Style.Col5} {Style.BorderEnd} {Style.H100} {Style.DFlex} {Style.FlexColumn}"
    ] [
        div [ Id "source"; ClassName Style.FlexGrow1 ] [
            Editor [
                MonacoEditorProp.OnChange(UpdateSourceCode >> dispatch)
                MonacoEditorProp.DefaultValue model.SourceCode
                MonacoEditorProp.Options(MonacoEditorProp.rulerOption model.FantomasModel.MaxLineLength)
            ]
        ]
    ]

let private homeTab =
    div [ ClassName $"{Style.BgLight} {Style.P5}" ] [
        div [ ClassName $"{Style.DFlex} {Style.AlignItemsCenter} {Style.Mb4}" ] [
            img [ Src "./logo.png" ]
            h1 [ ClassName $"{Style.Display3} {Style.Ms4}" ] [ str "Fantomas tool" ]
        ]
        p [ ClassName Style.Lead ] [ str "Welcome to the Fantomas Tools!" ]
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
        div [ ClassName Style.Inner ] [
            div [ ClassName Style.TextEnd ] [
                button [
                    Type "Button"
                    ClassName Style.BtnClose
                    OnClick(fun _ -> dispatch ToggleSettings)
                ] []
            ]
            h1 [ ClassName Style.TextCenter ] [ str "Settings" ]
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

        let isActiveClass = if isActive then Style.Active else ""

        div [ ClassName Style.NavItem; Key label ] [
            a [ Href href; ClassName $"{Style.NavLink} {isActiveClass}" ] [ str label ]
        ]

    let isFantomasTab =
        function
        | FantomasTab _ -> true
        | _ -> false

    let navItems =
        [ navItem HomeTab "Home" (model.ActiveTab = HomeTab)
          navItem ASTTab "AST" (model.ActiveTab = ASTTab)
          navItem OakTab "Oak" (model.ActiveTab = OakTab)
          navItem
              (FantomasTab FantomasTools.Client.FantomasOnline.Model.Main)
              "Fantomas"
              (isFantomasTab model.ActiveTab) ]

    div [ ClassName $"{Style.Col7} {Style.H100}" ] [
        settings model dispatch settingsForTab
        div [ ClassName $"{Style.Nav} {Style.NavTabs}" ] [ ofList navItems ]
        div [ Id "tab-content" ] [ activeTab; div [ Id "commands" ] [ commands ] ]
    ]
