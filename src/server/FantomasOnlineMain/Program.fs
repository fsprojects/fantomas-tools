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
                let! formatResponse = FantomasOnlineMain.FormatCode.formatCode json
                return! (mapFormatResponseToWebPart formatResponse) ctx
            })

    let routes =
        [ GET >=> path "/fantomas/main/version" >=> textPlain >=> OK(FantomasOnlineMain.FormatCode.getVersion ())
          GET
          >=> path "/fantomas/main/options"
          >=> applicationJson
          >=> OK(FantomasOnlineMain.FormatCode.getOptions ())
          POST >=> path "/fantomas/main/format" >=> formatWebPart ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 11084us

    startFantomasTool port routes

    0
