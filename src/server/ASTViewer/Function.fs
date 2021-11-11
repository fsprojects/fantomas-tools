module ASTViewer.Lambda

open System.Net
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open AWSLambdaExtensions
open ASTViewer.GetAST
open HttpConstants
open Microsoft.Net.Http.Headers

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let PostUntypedAST (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    async {
        let! internalResponse = getUntypedAST request.Body

        let statusCode, contentType, body =
            match internalResponse with
            | ASTResponse.Ok json -> int HttpStatusCode.OK, HeaderValues.ApplicationJson, json
            | ASTResponse.InvalidAST error -> int HttpStatusCode.BadRequest, HeaderValues.ApplicationText, error
            | ASTResponse.TooLarge ->
                int HttpStatusCode.RequestEntityTooLarge, HeaderValues.ApplicationText, "File was too large"
            | ASTResponse.InternalError error ->
                int HttpStatusCode.InternalServerError, HeaderValues.ApplicationText, error

        return
            APIGatewayProxyResponse(
                StatusCode = statusCode,
                Body = body,
                Headers = createHeaders [ HeaderNames.ContentType, contentType ]
            )
    }
    |> Async.StartAsTask

let GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    let version = getVersion ()

    APIGatewayProxyResponse(
        StatusCode = int HttpStatusCode.OK,
        Body = version,
        Headers = createHeaders [ HeaderNames.ContentType, HeaderValues.TextPlain ]
    )
