open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open SuaveExtensions
open TriviaViewer.GetTrivia

[<EntryPoint>]
let main argv =
    let mapTriviaResponseToWebPart (response: GetTriviaResponse) : WebPart =
        match response with
        | GetTriviaResponse.Ok body -> (applicationJson >=> OK body)
        | GetTriviaResponse.BadRequest errors -> (applicationText >=> BAD_REQUEST errors)

    let getTriviaWebPart =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let astResponse = getTrivia json
                return! (mapTriviaResponseToWebPart astResponse) ctx
            })

    let routes =
        [ GET >=> path "/trivia-viewer/version" >=> textPlain >=> OK(getVersion ())
          POST >=> path "/trivia-viewer/get-trivia" >=> getTriviaWebPart ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 9856us

    startFantomasTool port routes

    0
