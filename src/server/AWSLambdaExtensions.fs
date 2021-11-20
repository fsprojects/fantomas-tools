module AWSLambdaExtensions

open System.Collections.Generic
open System.Net
open Amazon.Lambda.APIGatewayEvents
open Microsoft.Net.Http.Headers

let createHeaders headers =
    Seq.fold
        (fun (acc: Dictionary<string, string>) (key, value) ->
            acc.[key] <- value
            acc)
        (Dictionary<string, string>())
        headers

let mkAPIGatewayProxyResponse (statusCode: HttpStatusCode, contentTypeHeaderValue: string, body: string) =
    APIGatewayProxyResponse(
        StatusCode = int statusCode,
        Body = body,
        Headers = createHeaders [ HeaderNames.ContentType, contentTypeHeaderValue ]
    )
