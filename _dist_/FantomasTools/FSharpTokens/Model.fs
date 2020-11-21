module FantomasTools.Client.FSharpTokens.Model

type Msg =
    | GetTokens
    | TokenReceived of string
    | LineSelected of int
    | TokenSelected of int
    | PlayScroll of int
    | DefinesUpdated of string
    | VersionFound of string
    | NetworkException of exn

type TokenInfo =
    { ColorClass: string
      CharClass: string
      FSharpTokenTriggerClass: string
      TokenName: string
      LeftColumn: int
      RightColumn: int
      Tag: int
      FullMatchedLength: int }

type Token =
    { TokenInfo: TokenInfo
      LineNumber: int
      Content: string }

type Model =
    { Defines: string
      Tokens: Token array
      ActiveLine: int option
      ActiveTokenIndex: int option
      IsLoading: bool
      Version: string }
