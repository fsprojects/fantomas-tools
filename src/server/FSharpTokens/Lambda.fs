module FSharpTokens.Lambda

open System.Net
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open AWSLambdaExtensions
open HttpConstants
open FSharpTokens.GetTokens

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let version = getVersion ()
    mkAPIGatewayProxyResponse (HttpStatusCode.OK, HeaderValues.TextPlain, version)

let GetTokens (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    match getTokens request.Body with
    | GetTokensResponse.Tokens body -> HttpStatusCode.OK, HeaderValues.ApplicationJson, body
    | GetTokensResponse.BadRequest body -> HttpStatusCode.BadRequest, HeaderValues.ApplicationText, body
    |> mkAPIGatewayProxyResponse
