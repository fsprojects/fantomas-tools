module FantomasTools.Client.View

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Model
open FantomasTools.Client.Editor

let private baseUrl: string = emitJsExpr () "import.meta.env.BASE_URL"

let navigation dispatch =
    let title = "Fantomas tools"

    nav [] [
        a [ Href baseUrl; Target "_self" ] [ img [ Src "./fantomas_logo.png"; ClassName $"{Style.Me3}" ]; str title ]
        a [
            Class "btn"
            Id "mobile-menu-toggle"
            OnClick(fun _ -> dispatch ToggleSettings)
        ] [ i [ ClassName "fas fa-sliders-h" ] [] ]
        div [] [
            a [
                Class "btn"
                Href "https://github.com/sponsors/nojaf"
                Target "_blank"
                Id "sponsor-btn"
            ] [
                i [ ClassName $"far fa-heart {Style.Me1} {Style.Mt1} {Style.TextDanger}" ] []
                str "Sponsor"
            ]
            a [
                Class "btn"
                Href "https://github.com/fsprojects/fantomas-tools"
                Target "_blank"
                Id "repository-btn"
            ] [ i [ ClassName $"fab fa-github {Style.Me1} {Style.Mt1}" ] []; str "GitHub" ]
            a [
                Class "btn"
                Id "youtube-btn"
                Href "https://www.youtube.com/playlist?list=PLvw_J2kfZCX3Mf6tEbIPZXbzJOD1VGl4K"
                Target "_blank"
            ] [ i [ ClassName $"fab fa-youtube {Style.Me1} {Style.Mt1}" ] []; str "YouTube" ]
            a [
                Class "btn"
                Id "docs-btn"
                Href "https://fsprojects.github.io/fantomas/reference/fsharp-compiler-syntax.html"
                Target "_blank"
            ] [
                i [ ClassName $"fa fa-book {Style.Me1} {Style.Mt1}" ] []
                span [ ClassName "short-text" ] [ str "Docs" ]
                span [ ClassName "long-text" ] [ str "Fantomas.FCS Docs" ]
            ]
            a [ Class "btn"; Id "menu-toggle"; OnClick(fun _ -> dispatch ToggleSettings) ] [
                i [ ClassName "fas fa-sliders-h" ] []
            ]
        ]
    ]

let editor (model: Model) dispatch =
    div [ Id "source" ] [
        Editor [
            MonacoEditorProp.OnChange(UpdateSourceCode >> dispatch)
            MonacoEditorProp.DefaultValue model.Bubble.SourceCode
            MonacoEditorProp.Options(MonacoEditorProp.rulerOption model.FantomasModel.MaxLineLength)
        ]
    ]

let private homeTab =
    div [ Id "home-tab"; ClassName Style.TabContent ] [
        div [ ClassName Style.Shine ] [ img [ Src "./logo.png" ] ]
        h1 [] [ str "Fantomas tools" ]
        p [] [ str "Welcome to the Fantomas tools!" ]
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
    let className = if model.SettingsOpen then "open" else ""

    div [
        Id "settings"
        ClassName className
        OnClick(fun ev ->
            if ev.target?className = "settings open" then
                dispatch ToggleSettings)
    ] [
        i [
            Id "close-menu-btn"
            ClassName "fa-solid fa-xmark"
            OnClick(fun _ -> dispatch ToggleSettings)
        ] []
        div [ ClassName Style.Inner ] [ h1 [] [ str "Settings" ]; inner ]
    ]

let private emptyEditor = Editor [ MonacoEditorProp.Height "0" ]

let tabs (model: Model) dispatch =

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

        let onClick (ev: Event) =
            if isActive then
                ev.preventDefault ()

        li [ ClassName isActiveClass ] [ a [ Href href; OnClick onClick ] [ str label ] ]

    let isFantomasTab =
        function
        | FantomasTab _ -> true
        | _ -> false

    let tabs =
        ul [ Id "tabs" ] [
            navItem HomeTab "Home" (model.ActiveTab = HomeTab)
            navItem ASTTab "AST" (model.ActiveTab = ASTTab)
            navItem OakTab "Oak" (model.ActiveTab = OakTab)
            navItem
                (FantomasTab FantomasTools.Client.FantomasOnline.Model.Main)
                "Fantomas"
                (isFantomasTab model.ActiveTab)
            li [] []
        ]

    let activeTab, settingsForTab, commands =
        match model.ActiveTab with
        | HomeTab -> homeTab, null, null
        | ASTTab ->
            let astDispatch aMsg = dispatch (ASTMsg aMsg)

            null,
            ASTViewer.View.settings model.Bubble model.ASTModel.Version astDispatch,
            ASTViewer.View.commands astDispatch
        | OakTab ->
            let oakDispatch oMsg = dispatch (OakMsg oMsg)

            OakViewer.View.view model.OakModel oakDispatch,
            OakViewer.View.settings model.Bubble model.OakModel oakDispatch,
            OakViewer.View.commands oakDispatch

        | FantomasTab _ ->
            let fantomasDispatch fMsg = dispatch (FantomasMsg fMsg)

            FantomasOnline.View.view model.Bubble.IsFsi model.FantomasModel,
            FantomasOnline.View.settings model.Bubble.IsFsi model.FantomasModel fantomasDispatch,
            FantomasOnline.View.commands model.Bubble model.FantomasModel fantomasDispatch

    // We always want to show the result editor, even if it's empty.
    // This is to improve the performance with the react editor and not constantly load and unload it.
    let resultEditor =
        if model.Bubble.IsLoading then
            emptyEditor
        else

        match model.ActiveTab with
        | HomeTab
        | OakTab -> emptyEditor
        | ASTTab ->
            match model.ASTModel.Parsed with
            | None -> emptyEditor
            | Some(Ok parsed) ->
                EditorAux (fun ev -> Fable.Core.JS.console.log ev) true [ MonacoEditorProp.DefaultValue parsed.String ]
            | Some(Result.Error errors) -> ReadOnlyEditor [ MonacoEditorProp.DefaultValue errors ]

        | FantomasTab _ -> null

    div [ Id "tools" ] [
        settings model dispatch settingsForTab
        tabs
        if model.Bubble.IsLoading then
            Loader.tabLoading
        else
            activeTab
        resultEditor
        if not (isNull commands) then
            div [ Id "commands" ] [ commands ]
    ]
