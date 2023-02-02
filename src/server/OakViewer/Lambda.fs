module OakViewer.Lambda

open System.Net
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open AWSLambdaExtensions
open HttpConstants
open OakViewer.GetOak

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let version = getVersion ()
    mkAPIGatewayProxyResponse (HttpStatusCode.OK, HeaderValues.TextPlain, version)

let GetOak (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let oakResponse = getOak request.Body

    let responseData =
        match oakResponse with
        | GetOakResponse.Ok body -> HttpStatusCode.OK, HeaderValues.ApplicationText, body
        | GetOakResponse.BadRequest body -> HttpStatusCode.BadRequest, HeaderValues.ApplicationText, body

    mkAPIGatewayProxyResponse responseData
