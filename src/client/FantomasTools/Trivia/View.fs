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

let private tab activeTab tabType tabContent =
    let tabClassName =
        match activeTab with
        | t when (t = tabType) -> "active show"
        | _ -> System.String.Empty
        |> sprintf "fade h-100 %s"

    TabPane.tabPane [ TabPane.TabId(!^(tabToId tabType))
                      TabPane.Custom [ ClassName tabClassName ] ] [
        tabContent
    ]

let private byTriviaNodes model dispatch =
    tab model.ActiveTab ByTriviaNodes (ByTriviaNodes.view model dispatch)

let private byTrivia model dispatch =
    tab model.ActiveTab ByTrivia (ByTrivia.view model dispatch)

let private results model dispatch =
    let tabHeader label tabType =
        let isActive = tabType = model.ActiveTab
        NavItem.navItem [ NavItem.Custom [ OnClick(fun _ -> dispatch (Msg.SelectTab tabType))
                                           ClassName "pointer" ] ] [
            NavLink.navLink [ NavLink.Active isActive
                              NavLink.Custom [ ClassName "rounded-0" ] ] [
                str label
            ]
        ]

    div [ ClassName "h-100 d-flex flex-column"
          Id "results" ] [
        Nav.nav [ Nav.Tabs true
                  Nav.Pills true
                  Nav.Custom [ ClassName "border-bottom border-primary" ] ] [
            tabHeader "By trivia nodes" ByTriviaNodes
            tabHeader "By trivia" ByTrivia
        ]
        TabContent.tabContent [ TabContent.Custom [ ClassName "flex-grow-1" ]
                                TabContent.ActiveTab(!^(tabToId model.ActiveTab)) ] [
            byTriviaNodes model dispatch
            byTrivia model dispatch
        ]
    ]

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        match model.Error with
        | None -> results model dispatch
        | Some errors ->
            FantomasTools.Client.Editor.editorInTab [ EditorProp.Language "fsharp"
                                                      EditorProp.IsReadOnly true
                                                      EditorProp.Value errors ]

let commands dispatch =
    Button.button [ Button.Color Primary
                    Button.Custom [ ClassName "rounded-0"
                                    OnClick(fun _ -> dispatch GetTrivia) ] ] [
        i [ ClassName "fas fa-code mr-1" ] []
        str "Get trivia"
    ]

let settings (model: Model) dispatch =
    fragment [] [
        FantomasTools.Client.VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input
            (DefinesUpdated >> dispatch)
            "Defines"
            "Enter your defines separated with a space"
            model.Defines
    ]
