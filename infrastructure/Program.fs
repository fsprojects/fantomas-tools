module Program

open System.IO
open System.Net.Http
open System.Net.Http.Headers
open Pulumi
open Pulumi.FSharp
open Pulumi.Aws
open Thoth.Json.Net
open Humanizer

let private commitDecoder: Decoder<string * string> =
    Decode.object (fun get ->
        let sha = get.Required.Field "sha" Decode.string

        let timestamp =
            get.Required.At [ "commit"; "author"; "date" ] Decode.string

        sha, timestamp)

let private getLastCommit () =
    async {
        use httpClient = new HttpClient()

        let request =
            new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/fsprojects/fantomas/commits")

        request.Headers.CacheControl <- CacheControlHeaderValue.Parse("no-cache")

        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36"
        )

        let! response = httpClient.SendAsync(request) |> Async.AwaitTask

        let! body =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask

        let decodeResult =
            match Decode.fromString (Decode.list commitDecoder) body with
            | Ok (t :: _)
            | Ok [ t ] ->
                printfn "Last commit: %A" t
                Some t
            | Ok [] -> None
            | Error e ->
                printfn "Could not get last commit from GitHub: %A" e
                None

        return decodeResult
    }

let (</>) a b = Path.Combine(a, b)

type LambdaProject =
    { Name: string
      FileArchive: string
      HandlerPrefix: string
      Lambdas: LambdaInfo list
      FunctionPrefix: string }

and LambdaInfo =
    { Name: string
      Verb: string
      Route: string }

let allLambdas =
    let mkLambdaInfo name verb route =
        { Name = name
          Verb = verb
          Route = route }

    let mkLambdaProject (name: string) lambdas =
        let archive =
            __SOURCE_DIRECTORY__
            </> ".."
            </> "artifacts"
            </> name

        { Name = name
          FileArchive = archive
          HandlerPrefix = name.Kebaberize()
          Lambdas = lambdas
          FunctionPrefix = $"{name}::{name}.Lambda" }

    [ mkLambdaProject
        "FSharpTokens"
        [ mkLambdaInfo "GetVersion" "GET" "/fsharp-tokens/version"
          mkLambdaInfo "GetTokens" "POST" "/fsharp-tokens/get-tokens" ]
      mkLambdaProject
          "ASTViewer"
          [ mkLambdaInfo "GetVersion" "GET" "/ast-viewer/version"
            mkLambdaInfo "PostUntypedAST" "POST" "/ast-viewer/untyped-ast"
            mkLambdaInfo "PostTypedAST" "POST" "/ast-viewer/typed-ast" ]
      mkLambdaProject
          "TriviaViewer"
          [ mkLambdaInfo "GetVersion" "GET" "/trivia-viewer/version"
            mkLambdaInfo "GetTrivia" "POST" "/trivia-viewer/get-trivia" ]
      mkLambdaProject
          "FantomasOnlineV2"
          [ mkLambdaInfo "GetVersion" "GET" "/fantomas/v2/version"
            mkLambdaInfo "GetOptions" "GET" "/fantomas/v2/options"
            mkLambdaInfo "PostFormat" "POST" "/fantomas/v2/format" ]
      mkLambdaProject
          "FantomasOnlineV3"
          [ mkLambdaInfo "GetVersion" "GET" "/fantomas/v3/version"
            mkLambdaInfo "GetOptions" "GET" "/fantomas/v3/options"
            mkLambdaInfo "PostFormat" "POST" "/fantomas/v3/format" ]
      mkLambdaProject
          "FantomasOnlineV4"
          [ mkLambdaInfo "GetVersion" "GET" "/fantomas/v4/version"
            mkLambdaInfo "GetOptions" "GET" "/fantomas/v4/options"
            mkLambdaInfo "PostFormat" "POST" "/fantomas/v4/format" ]
      mkLambdaProject
          "FantomasOnlinePreview"
          [ mkLambdaInfo "GetVersion" "GET" "/fantomas/preview/version"
            mkLambdaInfo "GetOptions" "GET" "/fantomas/preview/options"
            mkLambdaInfo "PostFormat" "POST" "/fantomas/preview/format" ] ]

let infra () =
    async {
        let lambdaRole =
            Iam.Role(
                "FantomasLambdaRole",
                Iam.RoleArgs(
                    AssumeRolePolicy =
                        input
                            """{
                               	"Version": "2012-10-17",
                               	"Statement": [{
                               		"Action": "sts:AssumeRole",
                               		"Principal": {
                               			"Service": "lambda.amazonaws.com"
                               		},
                               		"Effect": "Allow",
                               		"Sid": ""
                               	}]
                               }"""
                )
            )

        let _policy =
            let args =
                Iam.RolePolicyArgs(
                    Policy =
                        input
                            """{
	                                "Version": "2012-10-17",
	                                "Statement": [{
		                                "Effect": "Allow",
		                                "Action": [
			                                "logs:CreateLogGroup",
			                                "logs:CreateLogStream",
			                                "logs:PutLogEvents"
		                                ],
		                                "Resource": "arn:aws:logs:*:*:*"
	                                }]
                                }""",
                    Role = io lambdaRole.Id
                )

            Iam.RolePolicy("fantomas-log-policy", args)

        let gateway =
            let cors =
                ApiGatewayV2.Inputs.ApiCorsConfigurationArgs(
                    AllowHeaders = inputList [ input "*" ],
                    AllowMethods = inputList [ input "*" ],
                    AllowOrigins =
                        inputList [ input "https://fsprojects.github.io"
                                    input "http://localhost:9060" ]
                )

            let args =
                ApiGatewayV2.ApiArgs(ProtocolType = input "HTTP", CorsConfiguration = input cors)

            ApiGatewayV2.Api("fantomas-gateway", args)

        let _mainStage =
            let args =
                ApiGatewayV2.StageArgs(ApiId = io gateway.Id, AutoDeploy = input true)

            ApiGatewayV2.Stage("fantomas-main-stage", args)

        let lambdaIds =
            allLambdas
            |> List.collect (fun lambdaProject ->
                lambdaProject.Lambdas
                |> List.map (fun lambdaInfo ->
                    let lambdaFunctionName =
                        $"{lambdaProject.Name}{lambdaInfo.Name}"
                            .Kebaberize()

                    let lambda =
                        let args =
                            Lambda.FunctionArgs(
                                Handler = input $"{lambdaProject.FunctionPrefix}::{lambdaInfo.Name}",
                                Runtime = inputUnion2Of2 Lambda.Runtime.DotnetCore3d1,
                                Code = input (FileArchive(lambdaProject.FileArchive) :> Archive),
                                Role = io lambdaRole.Arn,
                                Timeout = input 30,
                                MemorySize = input 256
                            )

                        Lambda.Function(lambdaFunctionName, args)

                    let _log =
                        CloudWatch.LogGroup(
                            $"{lambdaFunctionName}-log-group",
                            CloudWatch.LogGroupArgs(
                                RetentionInDays = input 30,
                                Name = io (lambda.Id.Apply(fun id -> $"/aws/lambda/{id}"))
                            )
                        )

                    let _lambdaPermission =
                        Lambda.Permission(
                            $"{lambdaFunctionName}-lambda-permissions",
                            Lambda.PermissionArgs(
                                Function = io lambda.Name,
                                Principal = input "apigateway.amazonaws.com",
                                Action = input "lambda:InvokeFunction",
                                SourceArn = io (Output.Format($"{gateway.ExecutionArn}/*"))
                            )
                        )

                    let lambdaIntegration =
                        let args =
                            ApiGatewayV2.IntegrationArgs(
                                ApiId = io gateway.Id,
                                IntegrationType = input "AWS_PROXY",
                                IntegrationMethod = input "POST",
                                IntegrationUri = io lambda.Arn
                            )

                        ApiGatewayV2.Integration($"{lambdaFunctionName}-integration", args)

                    let _apiRoute =
                        let args =
                            ApiGatewayV2.RouteArgs(
                                ApiId = io gateway.Id,
                                RouteKey = input $"{lambdaInfo.Verb} {lambdaInfo.Route}",
                                Target = io (lambdaIntegration.Id.Apply(fun id -> $"integrations/{id}"))
                            )

                        ApiGatewayV2.Route($"{lambdaFunctionName}-route", args)

                    $"{lambdaProject.Name}_{lambdaInfo.Name}", null))

        return dict lambdaIds
    }

[<EntryPoint>]
let main _ = Deployment.runAsync infra
