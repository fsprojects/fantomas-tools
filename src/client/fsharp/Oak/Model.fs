module FantomasTools.Client.OakViewer.Model

type Msg =
    | GetOak
    | OakReceived of string
    | DefinesUpdated of string
    | FSCVersionReceived of string
    | SetFsiFile of bool
    | SetStroustrup of bool
    | Error of string
    | HighLight of FantomasTools.Client.Editor.HighLightRange

type Model =
    { Oak: string
      Error: string option
      IsLoading: bool
      Defines: string
      Version: string
      IsStroustrup: bool }
