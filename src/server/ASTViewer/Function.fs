module ASTViewer.Lambda

open System.Collections.Generic
open System.Net
open Microsoft.Net.Http.Headers
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open ASTViewer.GetAST

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

type Function() =
    member _.GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
        let version = getVersion ()

        let headers = Dictionary<string, string>()
        headers.[HeaderNames.ContentType] <- "text/plain"

        APIGatewayProxyResponse(StatusCode = int HttpStatusCode.OK, Body = version, Headers = headers)

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers

[<EntryPoint>]
let main argv =
    let webPart =
        choose [ GET >=> path "/version" >=> setMimeType "text/plain" >=> OK(getVersion ()) ]

    let port =
        match List.ofArray argv with
        | [ "--port"; port ] -> System.UInt16.Parse port
        | _ -> 7412us

    startWebServer { defaultConfig with bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ] } webPart

    0
