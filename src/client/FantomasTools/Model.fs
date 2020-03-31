module FantomasTools.Client.Model

type ActiveTab =
    | HomeTab
    | TokensTab
    | ASTTab
    | TriviaTab
    | FantomasTab

type Model =
    { ActiveTab: ActiveTab
      SourceCode: string
      TriviaModel : FantomasTools.Client.Trivia.Model.Model
      FSharpTokensModel : FantomasTools.Client.FSharpTokens.Model.Model
      ASTModel: FantomasTools.Client.ASTViewer.Model.Model }

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | TriviaMsg of FantomasTools.Client.Trivia.Model.Msg
    | FSharpTokensMsg of FantomasTools.Client.FSharpTokens.Model.Msg
    | ASTMsg of FantomasTools.Client.ASTViewer.Model.Msg
