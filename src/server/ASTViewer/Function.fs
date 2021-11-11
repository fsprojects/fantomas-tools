module ASTViewer.Lambda

open System.Collections.Generic
open System.Net
open Microsoft.Net.Http.Headers
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open ASTViewer.GetAST
open Suave

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let createHeaders headers =
    Seq.fold
        (fun (acc: Dictionary<string, string>) (key, value) ->
            acc.[key] <- value
            acc)
        (Dictionary<string, string>())
        headers

type Function() =
    member _.GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
        let version = getVersion ()

        APIGatewayProxyResponse(
            StatusCode = int HttpStatusCode.OK,
            Body = version,
            Headers = createHeaders [ HeaderNames.ContentType, "text/plain" ]
        )

    member _.PostUntypedAST (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
        async {
            let! internalResponse = getUntypedAST request.Body

            let statusCode, contentType, body =
                match internalResponse with
                | ASTResponse.Ok json -> int HttpStatusCode.OK, "application/json", json
                | ASTResponse.InvalidAST error -> int HttpStatusCode.BadRequest, "application/text", error
                | ASTResponse.TooLarge ->
                    int HttpStatusCode.RequestEntityTooLarge, "application/text", "File was too large"
                | ASTResponse.InternalError error -> int HttpStatusCode.InternalServerError, "application/text", error

            return
                APIGatewayProxyResponse(
                    StatusCode = statusCode,
                    Body = body,
                    Headers = createHeaders [ HeaderNames.ContentType, contentType ]
                )
        }
        |> Async.StartAsTask

open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers
open Suave.RequestErrors
open Suave.Response
open Suave.CORS

type HttpRequest with
    member this.BodyText = System.Text.Encoding.UTF8.GetString this.rawForm

let applicationJson = setMimeType "application/json"
let applicationText = setMimeType "application/text"
let private getBytes (v: string) = System.Text.Encoding.UTF8.GetBytes v
let REQUEST_ENTITY_TOO_LARGE (body: string) = response HTTP_413 (getBytes body)
let INTERNAL_SERVER_ERROR (body: string) = response HTTP_500 (getBytes body)

let setCORSHeaders =
    addHeader "Access-Control-Allow-Origin" "*"
    >=> addHeader "Access-Control-Allow-Headers" "*"
    >=> addHeader "Access-Control-Allow-Methods" "*"

[<EntryPoint>]
let main argv =
    let mapASTResponseToWebPart (response: ASTResponse) : WebPart =
        match response with
        | ASTResponse.Ok body -> (applicationJson >=> OK body)
        | ASTResponse.InvalidAST errors -> (applicationText >=> BAD_REQUEST errors)
        | ASTResponse.TooLarge -> (applicationText >=> REQUEST_ENTITY_TOO_LARGE "File was too large")
        | ASTResponse.InternalError error -> (applicationText >=> INTERNAL_SERVER_ERROR error)

    let untypedAst =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let! astResponse = getUntypedAST json
                return! (mapASTResponseToWebPart astResponse) ctx
            })

    let typedAst =
        request (fun req ctx ->
            async {
                let json = req.BodyText
                let! astResponse = getTypedAST json
                return! (mapASTResponseToWebPart astResponse) ctx
            })

    let webPart =
        setCORSHeaders
        >=> choose
                [ OPTIONS >=> no_content
                  GET >=> path "/ast-viewer/version" >=> setMimeType "text/plain" >=> OK(getVersion ())
                  POST >=> path "/ast-viewer/untyped-ast" >=> untypedAst
                  POST >=> path "/ast-viewer/typed-ast" >=> typedAst
                  NOT_FOUND "Not found" ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 7412us

    startWebServer { defaultConfig with bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ] } webPart

    0
