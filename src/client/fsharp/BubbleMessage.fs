namespace FantomasTools.Client

/// Messages sent from the individual tab update loop to the main update loop
type BubbleMessage =
    | SetFsi of bool
    | SetDefines of string
    | SetSourceCode of string
    | SetResultCode of string
    | SetDiagnostics of Diagnostic array
    | HighLight of Range

type BubbleModel =
    {
        SourceCode: string
        IsFsi: bool
        /// The result of the used tool.
        /// Used in AST and Fantomas tab.
        Defines: string
        ResultCode: string
        Diagnostics: Diagnostic array
    }
