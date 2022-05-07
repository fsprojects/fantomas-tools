module ASTViewer.Lambda

open System.Net
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open AWSLambdaExtensions
open ASTViewer.GetAST
open HttpConstants

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let version = getVersion ()
    mkAPIGatewayProxyResponse (HttpStatusCode.OK, HeaderValues.TextPlain, version)

let private mapASTResponse response =
    match response with
    | ASTResponse.Ok json -> HttpStatusCode.OK, HeaderValues.ApplicationJson, json
    | ASTResponse.TooLarge -> HttpStatusCode.RequestEntityTooLarge, HeaderValues.ApplicationText, "File was too large"
    | ASTResponse.InternalError error -> HttpStatusCode.InternalServerError, HeaderValues.ApplicationText, error

let PostUntypedAST (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let astResponse = getUntypedAST request.Body

    mapASTResponse astResponse
    |> mkAPIGatewayProxyResponse
