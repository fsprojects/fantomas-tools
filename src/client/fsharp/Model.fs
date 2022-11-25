module FantomasTools.Client.Model

open FantomasTools.Client

type ActiveTab =
    | HomeTab
    | ASTTab
    | OakTab
    | TriviaTab
    | FantomasTab of FantomasTools.Client.FantomasOnline.Model.FantomasMode

type Model =
    { ActiveTab: ActiveTab
      SourceCode: string
      SettingsOpen: bool
      IsFsi: bool
      OakModel: OakViewer.Model.Model
      TriviaModel: Trivia.Model.Model
      ASTModel: ASTViewer.Model.Model
      FantomasModel: FantomasOnline.Model.Model }

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | OakMsg of OakViewer.Model.Msg
    | TriviaMsg of Trivia.Model.Msg
    | ASTMsg of ASTViewer.Model.Msg
    | FantomasMsg of FantomasOnline.Model.Msg
    | ToggleSettings
