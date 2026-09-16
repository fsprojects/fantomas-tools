namespace FantomasTools.Client

open Thoth.Json

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

module BubbleModel =
    let empty =
        {
            SourceCode = ""
            IsFsi = false
            Defines = ""
            ResultCode = ""
            Diagnostics = Array.empty
            HighLight = Range.Zero
        }

    /// What a link says about the shared state. Everything but the code is optional, because a link
    /// made by an older version of the tool did not have to carry it, and those links still work.
    let decoder: Decoder<BubbleModel> =
        Decode.object (fun get ->
            let sourceCode = get.Required.Field "code" Decode.string
            let isFsi = get.Optional.Field "isFsi" Decode.bool |> Option.defaultValue false
            let defines = get.Optional.Field "defines" Decode.string |> Option.defaultValue ""

            { empty with
                SourceCode = sourceCode
                IsFsi = isFsi
                Defines = defines
            }
        )
