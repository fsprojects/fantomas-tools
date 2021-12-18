open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open SuaveExtensions
open FSharpTokens.GetTokens

[<EntryPoint>]
let main argv =
    let mapGetTokensResponseToWebPart (response: GetTokensResponse) : WebPart =
        match response with
        | GetTokensResponse.Tokens body -> (applicationJson >=> OK body)
        | GetTokensResponse.BadRequest err -> (applicationText >=> BAD_REQUEST err)

    let routes =
        [ GET
          >=> path "/fsharp-tokens/version"
          >=> textPlain
          >=> OK(getVersion ())
          POST
          >=> path "/fsharp-tokens/get-tokens"
          >=> request (fun req ->
              getTokens req.BodyText
              |> mapGetTokensResponseToWebPart) ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 7899us

    startFantomasTool port routes

    0
