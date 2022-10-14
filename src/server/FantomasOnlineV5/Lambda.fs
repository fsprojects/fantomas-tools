﻿module FantomasOnlineV5.Lambda

open System.Net
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core
open AWSLambdaExtensions
open HttpConstants
open FantomasOnline.Server.Shared.Http
open FantomasOnlineV5.FormatCode

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

let GetVersion (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    mkAPIGatewayProxyResponse (HttpStatusCode.OK, HeaderValues.TextPlain, getVersion ())

let GetOptions (_request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    mkAPIGatewayProxyResponse (HttpStatusCode.OK, HeaderValues.ApplicationJson, getOptions ())

let PostFormat (request: APIGatewayProxyRequest) (_context: ILambdaContext) =
    async {
        let! response = formatCode request.Body
        return mapFormatResponseToAPIGatewayProxyResponse response
    }
    |> Async.StartAsTask
