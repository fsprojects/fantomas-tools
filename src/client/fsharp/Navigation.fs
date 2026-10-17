module FantomasTools.Client.Navigation

open FantomasTools.Client
open FantomasTools.Client.Model
open Elmish
open FantomasTools.Client.Routing

let cmdForCurrentTab tab (model: Model) =
    let noSourceCode = System.String.IsNullOrWhiteSpace model.Bubble.SourceCode

    match tab with
    | HomeTab -> Cmd.none
    | ASTTab -> Cmd.ofMsg ASTViewer.Model.DoParse |> Cmd.map Msg.ASTMsg
    | OakTab -> Cmd.ofMsg OakViewer.Model.GetOak |> Cmd.map Msg.OakMsg
    | FantomasTab mode ->
        if noSourceCode then
            Cmd.none
        elif mode <> model.FantomasModel.Mode then
            Cmd.batch
                [
                    Cmd.map FantomasMsg (FantomasOnline.State.getOptionsCmd mode)
                    Cmd.map FantomasMsg (FantomasOnline.State.getVersionCmd mode)
                ]
        elif not (List.isEmpty model.FantomasModel.DefaultOptions) then
            Cmd.ofMsg FantomasOnline.Model.Format |> Cmd.map FantomasMsg
        else
            Cmd.none

let toHash =
    function
    | HomeTab -> "#/"
    | OakTab -> "#/oak"
    | ASTTab -> "#/ast"
    | FantomasTab FantomasOnline.Model.V6 -> "#/fantomas/v6"
    | FantomasTab FantomasOnline.Model.V7 -> "#/fantomas/v7"
    | FantomasTab FantomasOnline.Model.V8 -> "#/fantomas/v8"
    | FantomasTab FantomasOnline.Model.Main -> "#/fantomas/main"
    | FantomasTab FantomasOnline.Model.Preview -> "#/fantomas/preview"

/// The address of `tab`, with the query of the current address, which carries the code.
let href (tab: ActiveTab) : string =
    let hash = Browser.Dom.window.location.hash

    let query =
        if hash.Contains("?") then
            $"?%s{hash.Split('?').[1]}"
        else
            ""

    $"%s{toHash tab}%s{query}"

/// The shortcuts that go with Alt, by their letter, see App.fs.
let altShortcuts: (string * Shortcut) list =
    [
        "A", Shortcut.OpenAst
        "O", Shortcut.OpenOak
        "F", Shortcut.OpenFantomas
        "S", Shortcut.Settings
    ]

/// The letter that goes with Alt for `shortcut`.
let altLetter (shortcut: Shortcut) : string option =
    altShortcuts |> List.tryFind (fun (_, s) -> s = shortcut) |> Option.map fst

let parseUrl segments =
    match segments with
    | [ "ast" ]
    | [ "ast"; Route.Query [ "data", _ ] ] -> ActiveTab.ASTTab
    | [ "oak" ]
    | [ "oak"; Route.Query [ "data", _ ] ] -> ActiveTab.OakTab
    | [ "fantomas"; "v6" ]
    | [ "fantomas"; "v6"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.V6)
    | [ "fantomas"; "v7" ]
    | [ "fantomas"; "v7"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.V7)
    | [ "fantomas"; "v8" ]
    | [ "fantomas"; "v8"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.V8)
    | [ "fantomas"; "main" ]
    | [ "fantomas"; "main"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.Main)
    | [ "fantomas"; "preview" ]
    | [ "fantomas"; "preview"; Route.Query [ "data", _ ] ] -> ActiveTab.FantomasTab(FantomasOnline.Model.Preview)
    | _ -> ActiveTab.HomeTab
