namespace FantomasTools.Client

/// Messages sent from the individual tab update loop to the main update loop
type BubbleMessage =
    | SetFsi of bool
    | SetDefines of string
    | SetSourceCode of string
    | SetResultCode of string
    | SetDiagnostics of Diagnostic array
    | HighLight of Range

/// This is the shared data among all the tabs.
type BubbleModel =
    {
        SourceCode: string
        IsFsi: bool
        Defines: string
        /// The result of the used tool.
        /// Used in AST and Fantomas tab.
        ResultCode: string
        Diagnostics: Diagnostic array
        HighLight: Range
    }
