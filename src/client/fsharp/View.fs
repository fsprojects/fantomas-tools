module FantomasTools.Client.View

open Browser.Types
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client.FantomasOnline.Model
open FantomasTools.Client.Model
open FantomasTools.Client.Editor

let private baseUrl: string = emitJsExpr () "import.meta.env.BASE_URL"

let navigation model dispatch =
    let title = "Fantomas tools"

    nav [] [
        a [ Href baseUrl; Target "_self" ] [ img [ Src "./fantomas_logo.png"; ClassName $"{Style.Me3}" ]; str title ]
        a [
            Class "btn"
            Id "mobile-menu-toggle"
            OnClick(fun _ -> dispatch ToggleSettings)
        ] [ i [ ClassName "fas fa-sliders-h" ] [] ]
        div [] [
            button [ OnClick(fun _ -> Fable.Core.JS.console.log model) ] [ str "Print state" ]
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
        InputEditor
            (UpdateSourceCode >> dispatch)
            model.Bubble.SourceCode
            model.FantomasModel.MaxLineLength
            model.Bubble.HighLight
    ]

let private homeTab =
    div [ Id "home-tab" ] [
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

let tabs model =
    let navItem tab label isActive =
        let href =
            let page = Navigation.toHash tab

            let query =
                let hash = Browser.Dom.window.location.hash

                if hash.Contains("?") then
                    $"?%s{hash.Split('?').[1]}"
                else
                    ""

            $"%s{page}%s{query}"

        let isActiveClass = if isActive then Style.Active else ""

        let onClick (ev: Event) =
            if isActive then
                ev.preventDefault ()

        li [ ClassName isActiveClass ] [ a [ Href href; OnClick onClick ] [ str label ] ]

    let isFantomasTab =
        function
        | FantomasTab _ -> true
        | _ -> false

    ul [ Id "tabs" ] [
        navItem HomeTab "Home" (model.ActiveTab = HomeTab)
        navItem ASTTab "AST" (model.ActiveTab = ASTTab)
        navItem OakTab "Oak" (model.ActiveTab = OakTab)
        navItem (FantomasTab Main) "Fantomas" (isFantomasTab model.ActiveTab)
        li [] []
    ]

let diagnostics (bubble: BubbleModel) =
    if Array.isEmpty bubble.Diagnostics then
        null
    else
        let diagnostics =
            bubble.Diagnostics
            |> Array.mapi (fun idx diag ->
                li [ Key !!idx ] [
                    strong [] [
                        str
                            $"(%i{diag.Range.StartLine},%i{diag.Range.StartColumn}) (%i{diag.Range.EndLine}, %i{diag.Range.EndColumn})"
                    ]
                    span [ ClassName $"{Style.Badge} {diag.Severity}" ] [ str diag.Severity ]
                    span [ ClassName $"{Style.Badge} error-number"; Title "ErrorNumber" ] [ ofInt diag.ErrorNumber ]
                    span [ ClassName $"{Style.Badge} subcategory"; Title "SubCategory" ] [ str diag.SubCategory ]
                    p [] [ str diag.Message ]
                ])

        ul [ Id "diagnostics" ] [ ofArray diagnostics ]

let rightPane (model: Model) dispatch =
    let resultEditor, activeTab, settingsForTab, commands =
        match model.ActiveTab with
        | HomeTab -> HiddenEditor(), homeTab, null, null
        | ASTTab ->
            let astDispatch aMsg = dispatch (ASTMsg aMsg)

            let resultEditor =
                if model.Bubble.IsLoading then
                    HiddenEditor()
                else

                match model.ASTModel.State with
                | AstViewerTabState.Initial -> HiddenEditor()
                | AstViewerTabState.Result parsed ->
                    EditorAux
                        (ASTViewer.View.cursorChanged
                            (ASTViewer.Model.Msg.Bubble >> Msg.ASTMsg >> dispatch)
                            model.ASTModel)
                        true
                        model.Bubble.Diagnostics.Length
                        [ MonacoEditorProp.Value parsed.String ]
                | AstViewerTabState.Error errors -> ReadOnlyEditor [ MonacoEditorProp.Value errors ]

            resultEditor,
            null,
            ASTViewer.View.settings model.Bubble model.ASTModel.Version astDispatch,
            ASTViewer.View.commands astDispatch
        | OakTab ->
            let oakDispatch oMsg = dispatch (OakMsg oMsg)

            HiddenEditor(),
            OakViewer.View.view model.OakModel oakDispatch,
            OakViewer.View.settings model.Bubble model.OakModel oakDispatch,
            OakViewer.View.commands oakDispatch

        | FantomasTab _ ->
            let fantomasDispatch fMsg = dispatch (FantomasMsg fMsg)

            let resultEditor =
                if model.Bubble.IsLoading then
                    HiddenEditor()
                else

                Editor []

            resultEditor,
            FantomasOnline.View.view model.Bubble.IsFsi model.FantomasModel,
            FantomasOnline.View.settings model.Bubble.IsFsi model.FantomasModel fantomasDispatch,
            FantomasOnline.View.commands model.Bubble model.FantomasModel fantomasDispatch

    let showEditor =
        match model.ActiveTab with
        | ASTTab ->
            match model.ASTModel.State with
            | AstViewerTabState.Initial _ -> false
            | _ -> true
        | FantomasTab _ ->
            match model.FantomasModel.State with
            | FormatResult _
            | FormatError _ -> true
            | _ -> false
        | _ -> false

    // TODO: view diagnostics
    div [ Id "tools"; ClassName(if showEditor then "show-editor" else "") ] [
        pre [ Props.Style [ Display DisplayOptions.None ] ] [ str (Fable.Core.JS.JSON.stringify model) ]
        settings model dispatch settingsForTab
        tabs model
        if model.Bubble.IsLoading then
            str "loading"
        // Loader.tabLoading
        else
            activeTab
        resultEditor
        if not (isNull commands) then
            div [ Id "commands" ] [ commands ]
        if model.ActiveTab <> HomeTab then
            diagnostics model.Bubble
    ]
