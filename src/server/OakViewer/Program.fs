open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open SuaveExtensions
open OakViewer.GetOak

[<EntryPoint>]
let main argv =
    let mapOakResponseToWebPart (response: GetOakResponse) : WebPart =
        match response with
        | GetOakResponse.Ok body -> (applicationText >=> OK body)
        | GetOakResponse.BadRequest errors -> (applicationText >=> BAD_REQUEST errors)

    let getOakWebPart =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let astResponse = getOak json
                return! (mapOakResponseToWebPart astResponse) ctx
            })

    let routes =
        [ GET >=> path "/oak-viewer/version" >=> textPlain >=> OK(getVersion ())
          POST >=> path "/oak-viewer/get-oak" >=> getOakWebPart ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 8904us

    startFantomasTool port routes

    0
