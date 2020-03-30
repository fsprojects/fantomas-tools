module FantomasTools.Client.FSharpTokens.State

open System
open Fable.Core
open Fable.Core.JsInterop
open Elmish
open Fetch
open Thoth.Json
open FantomasTools.Client.FSharpTokens.Model
open FantomasTools.Client.FSharpTokens.Decoders
open FantomasTools.Client.FSharpTokens.Encoders
open FantomasTools.Client

let private backendRoot =
#if DEBUG
    "http://localhost:4563"
#else
    "https://azfun-fsharptokens.azurewebsites.net"
#endif


let private getTokens (request: FSharpTokens.Shared.GetTokensRequest): JS.Promise<string> =
    let url = sprintf "%s/%s" backendRoot "api/get-tokens"
    let json = Encode.toString 4 (encodeGetTokensRequest request)

    fetch url
        [ RequestProperties.Body(!^json)
          RequestProperties.Method HttpMethod.POST ]
    |> Promise.bind (fun res -> res.text())

let private getVersion() =
    let url = sprintf "%s/%s" backendRoot "api/version"
    Fetch.fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun (json: string) ->
        match Decode.fromString Decode.string json with
        | Ok v -> v
        | Error e -> failwithf "%A" e)

let private initialModel =
    { Defines = ""
      Tokens = [||]
      ActiveLine = None
      ActiveTokenIndex = None
      IsLoading = false
      Version = "??" }

let private decodeGetTokensRequest: Decoder<FSharpTokens.Shared.GetTokensRequest> =
    Decode.object (fun get ->
        { Defines = get.Required.Field "defines" (Decode.list Decode.string)
          SourceCode = get.Required.Field "sourceCode" Decode.string })

let private splitDefines (value: string) =
    value.Split([| ' '; ';' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

let private scrollTo (index: int): unit = import "scrollTo" "../../js/scrollTo.js"

let private modelToParseRequest sourceCode (model: Model): FSharpTokens.Shared.GetTokensRequest =
    let defines = splitDefines model.Defines
    { Defines = defines
      SourceCode = sourceCode }

let private updateUrl code (model: Model) _ =
    let json = Encode.toString 2 (encodeUrlModel code model)
    UrlTools.updateUrlWithData json

let init code =
    let model = UrlTools.restoreModelFromUrl (decodeUrlModel initialModel) initialModel

    let cmd =
        let fetchCmd = if String.IsNullOrWhiteSpace code then Cmd.none else Cmd.ofMsg GetTokens
        let versionCmd = Cmd.OfPromise.either getVersion () VersionFound NetworkException
        Cmd.batch [ versionCmd; fetchCmd ]

    model, cmd

let update code msg model =
    match msg with
    | GetTokens ->
        let cmd =
            let requestCmd = Cmd.OfPromise.perform getTokens (modelToParseRequest code model) TokenReceived
            let updateUrlCmd = Cmd.ofSub (updateUrl code model)
            Cmd.batch [ requestCmd; updateUrlCmd ]

        { model with IsLoading = true }, cmd
    | TokenReceived(tokensText) ->
        match Decoders.decodeTokens tokensText with
        | Ok tokens ->
            let cmd =
                if (Array.length tokens) = 1 then Cmd.OfFunc.result (LineSelected 1) else Cmd.none

            { model with
                  Tokens = tokens
                  IsLoading = false }, cmd
        | Error error ->
            printfn "%A" error
            { model with IsLoading = false }, Cmd.none
    | LineSelected lineNumber ->
        { model with ActiveLine = Some lineNumber }, Cmd.none

    | TokenSelected tokenIndex ->
        let highlightCmd =
            model.ActiveLine
            |> Option.map (fun activeLine ->
                let token =
                    model.Tokens
                    |> Array.filter (fun t -> t.LineNumber = activeLine)
                    |> Array.item tokenIndex
                    |> fun t -> t.TokenInfo
                Cmd.ofSub (FantomasTools.Client.Editor.selectRange activeLine token.LeftColumn activeLine token.RightColumn))
            |> Option.defaultValue Cmd.none
        let scrollCmd = Cmd.OfFunc.result (PlayScroll tokenIndex)

        { model with ActiveTokenIndex = Some tokenIndex }, Cmd.batch [ highlightCmd ; scrollCmd ]
    | PlayScroll index ->
        model, Cmd.ofSub (fun _ -> scrollTo index)
    | DefinesUpdated defines ->
        { model with Defines = defines }, Cmd.none
    | VersionFound v -> { model with Version = v }, Cmd.none
    | NetworkException e ->
        JS.console.error e
        model, Cmd.none