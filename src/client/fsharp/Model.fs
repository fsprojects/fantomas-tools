module FantomasTools.Client.Model

open FantomasTools.Client

type ActiveTab =
    | HomeTab
    | ASTTab
    | OakTab
    | FantomasTab of FantomasTools.Client.FantomasOnline.Model.FantomasMode

type Model =
    {
        ActiveTab: ActiveTab
        SettingsOpen: bool
        Bubble: BubbleModel
        OakModel: OakViewer.Model.Model
        ASTModel: ASTViewer.Model.Model
        FantomasModel: FantomasOnline.Model.Model
    }

/// What a keyboard shortcut does, see App.fs.
[<RequireQualifiedAccess; Struct>]
type Shortcut =
    /// Do what the button of the active tab does.
    | Run
    | OpenAst
    | OpenOak
    /// Open the Fantomas tab at the version it was last on, or on it, the next version.
    | OpenFantomas
    /// Open or close the settings of the active tab.
    | Settings

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | OakMsg of OakViewer.Model.Msg
    | ASTMsg of ASTViewer.Model.Msg
    | FantomasMsg of FantomasOnline.Model.Msg
    | ToggleSettings
    | Pressed of Shortcut
