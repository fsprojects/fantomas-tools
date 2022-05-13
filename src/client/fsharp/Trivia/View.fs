module FantomasTools.Client.Trivia.View

open FantomasTools.Client.Editor
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.Trivia
open FantomasTools.Client.Trivia.Model
open Reactstrap

let private tabToId tab =
    match tab with
    | ByTriviaNodes -> "trivia-nodes"
    | ByTrivia -> "trivia"
    | ByTriviaNodeCandidates -> "trivia-nodes-candidates"

let private tab activeTab tabType tabContent =
    let tabClassName =
        match activeTab with
        | t when (t = tabType) -> "active show"
        | _ -> System.String.Empty

    TabPane.tabPane [
        TabPane.TabId(!^(tabToId tabType))
        TabPane.Custom [ ClassName tabClassName ]
    ] [ tabContent ]

let private byTriviaNodes model dispatch =
    tab model.ActiveTab ByTriviaNodes (ByTriviaNodes.view model dispatch)

let private byTriviaNodeCandidates model dispatch =
    tab model.ActiveTab ByTriviaNodeCandidates (ByTriviaNodeCandidates.view model dispatch)

let private byTrivia model dispatch =
    tab model.ActiveTab ByTrivia (ByTrivia.view model dispatch)

let private results model dispatch =
    let tabHeader label tabType =
        let isActive = tabType = model.ActiveTab

        NavItem.navItem [
            NavItem.Custom [
                OnClick(fun _ -> dispatch (Msg.SelectTab tabType))
                ClassName "pointer"
            ]
        ] [
            NavLink.navLink [
                NavLink.Active isActive
                NavLink.Custom [ ClassName "rounded-0" ]
            ] [ str label ]
        ]

    div [ Id "results" ] [
        Nav.nav [
            Nav.Tabs true
            Nav.Pills true
            Nav.Custom [
                ClassName "border-bottom border-primary"
            ]
        ] [
            tabHeader "Trivia nodes" ByTriviaNodes
            tabHeader "Trivia node candidates" ByTriviaNodeCandidates
            tabHeader "Trivia" ByTrivia
        ]
        TabContent.tabContent [
            TabContent.Custom [ Id "trivia-result-content" ]
            TabContent.ActiveTab(!^(tabToId model.ActiveTab))
        ] [
            byTriviaNodes model dispatch
            byTriviaNodeCandidates model dispatch
            byTrivia model dispatch
        ]
    ]

let view model dispatch =
    if model.IsLoading then
        Loader.loader
    else
        match model.Error with
        | None -> results model dispatch
        | Some errors -> Editor true [ MonacoEditorProp.DefaultValue errors ]

let commands dispatch =
    Button.button [
        Button.Color Primary
        Button.Custom [
            ClassName "rounded-0"
            OnClick(fun _ -> dispatch GetTrivia)
        ]
    ] [
        i [ ClassName "fas fa-code mr-1" ] []
        str "Get trivia"
    ]

let settings (model: Model) dispatch =
    fragment [] [
        VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            "trivia-defines"
            (DefinesUpdated >> dispatch)
            "Defines"
            "Enter your defines separated with a space"
            model.Defines
        SettingControls.toggleButton
            (fun _ -> dispatch (SetFsiFile true))
            (fun _ -> dispatch (SetFsiFile false))
            "*.fsi"
            "*.fs"
            "File extension"
            model.IsFsi
    ]
