module ASTViewer.Lambda

open FSharp.Compiler.CodeAnalysis
open Amazon.Lambda.APIGatewayEvents
open Amazon.Lambda.Core

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

type ASTRequest() = class end
//    { Method: string
//      Path: string }

type ASTResponse =
    { Body: string
      StatusCode: int }

let getVersion () : string =
    let assembly = typeof<FSharpChecker>.Assembly
    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let getAst (_input: ASTRequest) : ASTResponse =
//    let { Method = method; Path = path } = input
//    match method, path with
//    | "GET", "/api/version" ->
    let version = getVersion ()
    { Body = version; StatusCode = 200 }

type Function() =
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="_context"></param>
    /// <returns></returns>
    member __.FunctionHandler (input: APIGatewayProxyRequest) (_context: ILambdaContext) =
        // let body = input.Body
        let internalResponse = getAst (ASTRequest())
        let response : APIGatewayProxyResponse =
            APIGatewayProxyResponse(StatusCode = internalResponse.StatusCode,
                                    Body = internalResponse.Body)
        response

 open Suave

[<EntryPoint>]
let main argv =    
    // TODO: call shared function
    startWebServer
        defaultConfig
        (fun ctx ->
            async {
                let path = ctx.request.path
                printfn "path:  %s" path

                let body =
                    ctx.request.rawForm
                    |> System.Text.Encoding.UTF8.GetString



                let internalRequest : ASTRequest =  ASTRequest()
                let internalResponse : ASTResponse = getAst internalRequest
                let responseBody =
                    internalResponse.Body
                    |> System.Text.Encoding.UTF8.GetBytes
                let response =
                    { status = HttpCode.HTTP_200.status
                      headers = []
                      content = HttpContent.Bytes responseBody
                      writePreamble = true }

                return Some { ctx with response = response }
            })

    0