namespace OakViewer

type ParseRequest =
    { SourceCode: string
      Defines: string array
      IsFsi: bool
      IsStroustrup: bool }
