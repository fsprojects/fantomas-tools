module FantomasTools.Client.Model

open FantomasTools.Client

type ActiveTab =
    | HomeTab
    | ASTTab
    | OakTab
    | FantomasTab of FantomasTools.Client.FantomasOnline.Model.FantomasMode

type Model =
    { ActiveTab: ActiveTab
      SettingsOpen: bool
      Bubble: BubbleModel
      OakModel: OakViewer.Model.Model
      ASTModel: ASTViewer.Model.Model
      FantomasModel: FantomasOnline.Model.Model }

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | OakMsg of OakViewer.Model.Msg
    | ASTMsg of ASTViewer.Model.Msg
    | FantomasMsg of FantomasOnline.Model.Msg
    | ToggleSettings
