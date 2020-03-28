module FantomasTools.Client.Model

type ActiveTab =
    | TokensTab
    | ASTTab
    | TriviaTab
    | FantomasTab

type Model =
    { ActiveTab: ActiveTab
      SourceCode: string
      TriviaModel : FantomasTools.Client.Trivia.Model.Model }

type Msg =
    | SelectTab of ActiveTab
    | UpdateSourceCode of string
    | TriviaMsg of FantomasTools.Client.Trivia.Model.Msg
