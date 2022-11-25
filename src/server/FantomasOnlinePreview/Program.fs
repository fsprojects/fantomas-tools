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
        | FormatResponse.InternalError error -> (applicationText >=> INTERNAL_SERVER_ERROR error)

    let formatWebPart =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let! formatResponse = FantomasOnlinePreview.FormatCode.formatCode json
                return! (mapFormatResponseToWebPart formatResponse) ctx
            })

    let routes =
        [ GET >=> path "/fantomas/preview/version" >=> textPlain >=> OK(FantomasOnlinePreview.FormatCode.getVersion ())
          GET
          >=> path "/fantomas/preview/options"
          >=> applicationJson
          >=> OK(FantomasOnlinePreview.FormatCode.getOptions ())
          POST >=> path "/fantomas/preview/format" >=> formatWebPart ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 12007us

    startFantomasTool port routes

    0
