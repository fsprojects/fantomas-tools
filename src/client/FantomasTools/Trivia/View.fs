module FantomasTools.Client.Trivia.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
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

    TabPane.tabPane
        [ TabPane.TabId(!^(tabToId tabType))
          TabPane.Custom [ ClassName tabClassName ] ] [ tabContent ]

let private byTriviaNodes model dispatch =
    tab model.ActiveTab ByTriviaNodes (ByTriviaNodes.view model dispatch)

let private byTrivia model dispatch =
    tab model.ActiveTab ByTrivia (ByTrivia.view model dispatch)

let private results model dispatch =
    let tabHeader label tabType =
        let isActive = tabType = model.ActiveTab
        NavItem.navItem
            [ NavItem.Custom
                [ OnClick(fun _ -> dispatch (Msg.SelectTab tabType))
                  ClassName "pointer" ] ]
            [ NavLink.navLink
                [ NavLink.Active isActive
                  NavLink.Custom [ ClassName "rounded-0" ] ] [ str label ] ]

    div
        [ ClassName "h-100 d-flex flex-column"
          Id "results" ]
        [ Nav.nav
            [ Nav.Tabs true
              Nav.Pills true
              Nav.Custom [ ClassName "border-bottom border-primary" ] ]
              [ tabHeader "By trivia nodes" ByTriviaNodes
                tabHeader "By trivia" ByTrivia ]
          TabContent.tabContent
              [ TabContent.Custom [ ClassName "flex-grow-1" ]
                TabContent.ActiveTab(!^(tabToId model.ActiveTab)) ]
              [ byTriviaNodes model dispatch
                byTrivia model dispatch ] ]

let private settings (model: Model) dispatch =
    let toggleButton msg active label =
        let className =
            if active then "rounded-0 text-white" else "rounded-0"

        Button.button
            [ Button.Custom
                [ ClassName className
                  OnClick(fun _ -> dispatch msg) ]
              Button.Outline(not active) ] [ str label ]

    Form.form
        [ Form.Custom
            [ Id "trivia-settings"
              OnSubmit(fun ev ->
                  ev.preventDefault ()
                  dispatch GetTrivia) ] ]
        [ FormGroup.formGroup [ FormGroup.Custom [ ClassName "flex-1" ] ]
              [ Input.input
                  [ Input.Custom
                      [ Placeholder "Enter your defines separated with a space"
                        OnClick(fun ev -> ev.Value |> Msg.UpdateDefines |> dispatch) ] ] ]
          FormGroup.formGroup []
              [ ButtonGroup.buttonGroup [ ButtonGroup.Custom [ ClassName "btn-group-toggle rounded-0 mw120" ] ]
                    [ toggleButton (SetFsiFile false) (not model.IsFsi) "*.fs"
                      toggleButton (SetFsiFile true) model.IsFsi "*.fsi" ] ]
          Button.button
              [ Button.Color Primary
                Button.Custom [ ClassName "rounded-0" ] ]
              [ i [ ClassName "fas fa-code mr-1" ] []
                str "Get trivia" ] ]

let view model dispatch =
    let inner =
        if model.IsLoading then FantomasTools.Client.Loader.loader else results model dispatch

    fragment []
        [ inner
          FantomasTools.Client.VersionBar.versionBar (sprintf "FSC - %s" model.FSCVersion)
          settings model dispatch ]
