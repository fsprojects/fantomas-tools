open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open SuaveExtensions
open FantomasOnline.Server.Shared.Http

[<EntryPoint>]
let main argv =
    let mapFormatResponseToWebPart (response: FormatResponse) : WebPart =
        match response with
        | FormatResponse.Ok body -> (applicationJson >=> OK body)
        | FormatResponse.BadRequest error -> (applicationText >=> BAD_REQUEST error)
        | FormatResponse.Failed error ->
            let statusCode, json = describeFailure error

            if statusCode = 400 then
                (applicationJson >=> BAD_REQUEST json)
            else
                (applicationJson >=> INTERNAL_SERVER_ERROR json)

    let formatWebPart =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let! formatResponse = FantomasOnlineV7.FormatCode.formatCode json
                return! (mapFormatResponseToWebPart formatResponse) ctx
            }
        )

    let routes =
        [
            GET >=> path "/fantomas/v7/version" >=> textPlain >=> OK(FantomasOnlineV7.FormatCode.getVersion ())
            GET >=> path "/fantomas/v7/options" >=> applicationJson >=> OK(FantomasOnlineV7.FormatCode.getOptions ())
            POST >=> path "/fantomas/v7/format" >=> formatWebPart
        ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 10707us

    startFantomasTool port routes

    0
