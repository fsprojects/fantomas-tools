open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open SuaveExtensions
open ASTViewer.GetAST

[<EntryPoint>]
let main argv =
    let mapASTResponseToWebPart (response: ASTResponse) : WebPart =
        match response with
        | ASTResponse.Ok body -> (applicationJson >=> OK body)
        | ASTResponse.TooLarge -> (applicationText >=> REQUEST_ENTITY_TOO_LARGE "File was too large")
        | ASTResponse.InternalError error -> (applicationText >=> INTERNAL_SERVER_ERROR error)

    let untypedAst =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let astResponse = getUntypedAST json
                return! (mapASTResponseToWebPart astResponse) ctx
            })

    let routes =
        [ GET >=> path "/ast-viewer/version" >=> textPlain >=> OK(getVersion ())
          POST >=> path "/ast-viewer/untyped-ast" >=> untypedAst ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 7412us

    startFantomasTool port routes

    0
