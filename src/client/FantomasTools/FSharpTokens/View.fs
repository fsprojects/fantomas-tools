module FantomasTools.Client.FSharpTokens.View

open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.FSharpTokens.Model
open Reactstrap

let private tokenNameClass token =
    sprintf "is-%s" (token.TokenInfo.TokenName.ToLower())

let private lineToken dispatch index (token: Token) =
    div [ ClassName "token"
          Key(index.ToString())
          OnClick(fun _ -> dispatch (TokenSelected index)) ] [
        span [ ClassName(sprintf "tag %s" (tokenNameClass token)) ] [
            str token.TokenInfo.TokenName
        ]
    ]

let private line dispatch activeLine (lineNumber, tokens) =
    let tokens =
        tokens
        |> Array.mapi (fun idx token -> lineToken dispatch idx token)

    let className =
        match activeLine with
        | Some al when (al = lineNumber) -> "line active"
        | _ -> "line"

    div [ ClassName className
          Key(sprintf "line-%d" lineNumber)
          OnClick(fun _ -> LineSelected lineNumber |> dispatch) ] [
        div [ ClassName "line-number" ] [
            ofInt lineNumber
        ]
        div [ ClassName "tokens" ] [
            ofArray tokens
        ]
    ]

let private tokens model dispatch =
    let lines =
        model.Tokens
        |> Array.groupBy (fun t -> t.LineNumber)
        |> Array.map (line dispatch model.ActiveLine)

    div [ Id "tokens" ] [
        div [ Class "lines" ] [ ofArray lines ]
    ]

let private tokenDetailRow label content =
    tr [] [
        td [] [ strong [] [ str label ] ]
        td [] [ content ]
    ]

let private tokenDetail dispatch index token =
    let className = tokenNameClass token |> sprintf "tag is-large %s"

    let { TokenName = tokenName; LeftColumn = leftColumn; RightColumn = rightColumn; ColorClass = colorClass;
          CharClass = charClass; Tag = tag; FullMatchedLength = fullMatchedLength }
        =
        token.TokenInfo

    div [ ClassName "detail"
          Key(index.ToString()) ] [
        h3 [ ClassName className
             OnClick(fun _ -> Msg.TokenSelected index |> dispatch) ] [
            str token.TokenInfo.TokenName
            small [] [ sprintf "(%d)" index |> str ]
        ]
        table [ ClassName "table table-striped table-hover mb-0" ] [
            tbody [] [
                tokenDetailRow "TokenName" (str tokenName)
                tokenDetailRow "LeftColumn" (ofInt leftColumn)
                tokenDetailRow "RightColumn" (ofInt rightColumn)
                tokenDetailRow "Content" (pre [] [ code [] [ str token.Content ] ])
                tokenDetailRow "ColorClass" (str colorClass)
                tokenDetailRow "CharClass" (str charClass)
                tokenDetailRow "Tag" (ofInt tag)
                tokenDetailRow "FullMatchedLength"
                    (span [ ClassName "has-text-weight-semibold" ] [
                        ofInt fullMatchedLength
                     ])
            ]
        ]
    ]

let private details model dispatch =
    model.ActiveLine
    |> Option.map (fun activeLine ->
        let details =
            model.Tokens
            |> Array.filter (fun t -> t.LineNumber = activeLine)
            |> Array.mapi (tokenDetail dispatch)

        div [ Id "details" ] [
            h4 [ ClassName "ml-2" ] [
                str "Details of line "
                span [ Class "has-text-grey" ] [
                    ofInt activeLine
                ]
            ]
            div [ Class "detail-container" ] [
                ofArray details
            ]
        ])
    |> ofOption

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        div [ ClassName "tab-result" ] [
            tokens model dispatch
            details model dispatch
        ]

let commands dispatch =
    Button.button [ Button.Color Primary
                    Button.Custom [ OnClick(fun _ -> dispatch GetTokens) ] ] [
        str "Get tokens"
    ]

let settings model dispatch =
    fragment [] [
        FantomasTools.Client.VersionBar.versionBar (sprintf "FSC - %s" model.Version)
        SettingControls.input (DefinesUpdated >> dispatch) "Defines" "Enter your defines separated with a space"
            model.Defines
    ]
