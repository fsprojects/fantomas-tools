namespace FantomasTools.Client

/// Messages sent from the individual tab update loop to the main update loop
type BubbleMessage =
    | SetFsi of bool
    | SetDefines of string
    | SetSourceCode of string
    | SetResultCode of string
    | SetDiagnostics of Diagnostic array
    | HighLight of Range
    | SetIsLoading of bool

/// This is the shared data among all the tabs.
type BubbleModel =
    {
        SourceCode: string
        IsFsi: bool
        /// The result of the used tool.
        /// Used in AST and Fantomas tab.
        Defines: string
        ResultCode: string
        Diagnostics: Diagnostic array
        // Is any http request in progress that will load data for the main right panel?
        IsLoading: bool
        HighLight: Range
    }
