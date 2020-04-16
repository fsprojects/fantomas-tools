module FantomasTools.Client.Model

open FantomasTools.Client

type ActiveTab =
    | HomeTab
    | TokensTab
    | ASTTab
    | TriviaTab
    | FantomasTab of FantomasTools.Client.FantomasOnline.Model.FantomasMode

type Model =
    { ActiveTab: ActiveTab
      SourceCode: string
      TriviaModel: Trivia.Model.Model
      FSharpTokensModel: FSharpTokens.Model.Model
      ASTModel: ASTViewer.Model.Model
      FantomasModel: FantomasOnline.Model.Model }

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | TriviaMsg of Trivia.Model.Msg
    | FSharpTokensMsg of FSharpTokens.Model.Msg
    | ASTMsg of ASTViewer.Model.Msg
    | FantomasMsg of FantomasOnline.Model.Msg
