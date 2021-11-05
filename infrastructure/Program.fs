module Program

open System.Net.Http
open System.Net.Http.Headers
open Pulumi
open Pulumi.Aws.S3
open Pulumi.FSharp
open System.IO
open Thoth.Json.Net

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
            | Ok ([ t ]) ->
                printfn "Last commit: %A" t
                Some t
            | Ok [] -> None
            | Error e ->
                printfn "Could not get last commit from GitHub: %A" e
                None

        return decodeResult
    }

(*
{
  "Information": [
    "This file provides default values for the deployment wizard inside Visual Studio and the AWS Lambda commands added to the .NET Core CLI.",
    "To learn more about the Lambda commands with the .NET Core CLI execute the following command at the command line in the project root directory.",
    "dotnet lambda help",
    "All the command line options for the Lambda command can be specified in this file."
  ],
  "profile": "",
  "region": "",
  "configuration": "Release",
  "framework": "netcoreapp3.1",
  "function-runtime": "dotnetcore3.1",
  "function-memory-size": 256,
  "function-timeout": 30,
  "function-handler": "ASTViewer::ASTViewer.Lambda.Function::FunctionHandler"
}
*)

let infra () =
    async {

        let lambdaRole =
            Aws.Iam.Role(
                "FantomasLambdaRole",
                Aws.Iam.RoleArgs(
                    AssumeRolePolicy =
                        input
                            """{
                            "Version": "2012-10-17",
                            "Statement": [
                                {
                                "Action": "sts:AssumeRole",
                                "Principal": {
                                "Service": "lambda.amazonaws.com"
                                },
                                "Effect": "Allow",
                                "Sid": ""
                                }
                                ]
                            }"""
                )
            )

        let policy =
            let args =
                Aws.Iam.RolePolicyArgs(
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
                                                                }
                                                                """,
                    Role = io lambdaRole.Id
                )

            Aws.Iam.RolePolicy("fantomas-log-policy", args)


        // Create an AWS resource (S3 Bucket)
        let lambda =
            let args =
                Aws.Lambda.FunctionArgs(
                    Handler = input "ASTViewer::ASTViewer.Lambda+Function::GetVersion",
                    Runtime = inputUnion2Of2 Aws.Lambda.Runtime.DotnetCore3d1,
                    Code =
                        input (
                            FileArchive(
                                @"C:\Users\fverdonck\Projects\fantomas-tools\src\server\ASTViewer\bin\Debug\netcoreapp3.1\publish"
                            )
                            :> Archive
                        ),
                    Role = io lambdaRole.Arn
                )

            Aws.Lambda.Function("ast-viewer", args)

        let log =
            Aws.CloudWatch.LogGroup(
                "fantomas-log-group",
                Aws.CloudWatch.LogGroupArgs(
                    RetentionInDays = input 30,
                    Name = io (lambda.Id.Apply(fun id -> $"/aws/lambda/{id}"))
                )
            )


        let gateway =
            let cors =
                Aws.ApiGatewayV2.Inputs.ApiCorsConfigurationArgs(
                    AllowHeaders = inputList [ input "*" ],
                    AllowMethods = inputList [ input "*" ],
                    AllowOrigins = inputList [ input "*" ]
                )

            let args =
                Aws.ApiGatewayV2.ApiArgs(ProtocolType = input "HTTP", CorsConfiguration = input cors)

            Aws.ApiGatewayV2.Api("fantomas-gateway", args)

        let lambdaPermission =
            Aws.Lambda.Permission(
                "lambda-permissions",
                Aws.Lambda.PermissionArgs(
                    Function = io lambda.Name,
                    Principal = input "apigateway.amazonaws.com",
                    Action = input "lambda:InvokeFunction",
                    SourceArn = io (Output.Format($"{gateway.ExecutionArn}/*"))
                )
            )

        let mainStage =
            let args =
                Aws.ApiGatewayV2.StageArgs(ApiId = io gateway.Id, AutoDeploy = input true)

            Aws.ApiGatewayV2.Stage("fantomas-main-stage", args)

        let lambdaIntegration =
            let args =
                Aws.ApiGatewayV2.IntegrationArgs(
                    ApiId = io gateway.Id,
                    IntegrationType = input "AWS_PROXY",
                    IntegrationMethod = input "POST",
                    IntegrationUri = io lambda.Arn
                )

            Aws.ApiGatewayV2.Integration("ast-viewer-integration", args)

        let apiRoute =
            let args =
                Aws.ApiGatewayV2.RouteArgs(
                    ApiId = io gateway.Id,
                    RouteKey = input "GET /version",
                    Target = io (lambdaIntegration.Id.Apply(fun id -> $"integrations/{id}"))
                )

            Aws.ApiGatewayV2.Route("ast-viewer-route", args)
        // Export the name of the bucket
        return dict [ ("lambdaId", lambda.Id :> obj) ]

    //        let stackName = Deployment.Instance.StackName
//
//        // Create an Azure Resource Group
//        let resourceGroupArgs = ResourceGroupArgs(Name = input (sprintf "rg-fantomas-%s" stackName))
//        let resourceGroup = ResourceGroup(sprintf "rg-fantomas-%s" stackName, args = resourceGroupArgs)
//
//        // Create an Azure Storage Account
//        let storageAccount =
//            Account
//                ("storagefantomas",
//                 AccountArgs
//                     (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "storfantomas%s" stackName),
//                      AccountReplicationType = input "LRS", AccountTier = input "Standard"))
//
//        // Table Storage for Benchmarks
//        let _benchmarkTable =
//            Table("benchmarks", TableArgs(StorageAccountName = io storageAccount.Name,
//                                          Name = input "FantomasBenchmarks"))
//
//        // container for zips
//        let zipContainer =
//            Container
//                ("zips",
//                 ContainerArgs
//                     (Name = input "zips", StorageAccountName = io storageAccount.Name,
//                      ContainerAccessType = input "private"))
//
//        // Create Application Insights
//        let applicationsInsight =
//            Insights
//                ("ai-fantomas",
//                 InsightsArgs
//                     (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "ai-fantomas-%s" stackName),
//                      ApplicationType = input "web"))
//
//        let appServicePlan =
//            Plan
//                ("azfun-fantomas",
//                 PlanArgs
//                     (ResourceGroupName = io resourceGroup.Name, Kind = input "FunctionApp",
//                      Sku = input (PlanSkuArgs(Tier = input "Dynamic", Size = input "Y1")),
//                      Name = input (sprintf "azfun-fantomas-plan-%s" stackName)))
//
//        let genericSiteConfig =
//            input
//                (FunctionAppSiteConfigArgs
//                    (Http2Enabled = input true,
//                     Cors = input
//                                (FunctionAppSiteConfigCorsArgs(AllowedOrigins = inputList [ input "https://fsprojects.github.io" ]))))
//
//        let artifactsFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts")
//
//        printfn "Current directory: %s" artifactsFolder
//
//        let toPascalCase (v: string) =
//            v.Split('-')
//            |> Array.map (fun piece ->
//                if piece = "fsharp" then
//                    "FSharp"
//                elif String.length piece > 3 then
//                    piece.[0].ToString().ToUpper() + piece.Substring(1)
//                else
//                    piece.ToUpper())
//            |> String.concat ""
//
//        let! lastCommit = getLastCommit ()
//        let lastCommitAppSettings =
//            match lastCommit with
//            | Some (sha, timestamp) -> [ "LAST_COMMIT_SHA", input sha
//                                         "LAST_COMMIT_TIMESTAMP", input timestamp ]
//            | None -> []
//
//        let functionHostNames =
//            [ "fantomas-online-v2"
//              "fantomas-online-v3"
//              "fantomas-online-v4"
//              "fantomas-online-preview"
//              "ast-viewer"
//              "fsharp-tokens"
//              "trivia-viewer" ]
//            |> List.map (fun funcName ->
//                let path = Path.Combine(artifactsFolder, (toPascalCase funcName))
//                let archive: AssetOrArchive = FileArchive(path) :> AssetOrArchive
//
//                let blob =
//                    Blob
//                        (sprintf "%s-zip" funcName,
//                         BlobArgs
//                             (StorageAccountName = io storageAccount.Name, StorageContainerName = io zipContainer.Name,
//                              Type = input "Block", Source = input archive))
//
//                let codeBlobUrl = SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount)
//
//                let functionAppSettings =
//                    inputMap
//                        [ "FUNCTIONS_WORKER_RUNTIME", input "dotnet-isolated"
//                          "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
//                          "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl
//                          yield! lastCommitAppSettings ]
//
//                let funcApp =
//                    FunctionApp
//                        (sprintf "azfun-%s-plan" funcName,
//                         FunctionAppArgs
//                             (ResourceGroupName = io resourceGroup.Name,
//                              Name = input (sprintf "azfun-%s-%s" funcName stackName),
//                              AppServicePlanId = io appServicePlan.Id,
//                              StorageConnectionString = io storageAccount.PrimaryConnectionString,
//                              AppSettings = functionAppSettings, SiteConfig = genericSiteConfig, HttpsOnly = input true,
//                              Version = input "~3"))
//
//                (sprintf "%s-app-host-name" funcName, funcApp.DefaultHostname :> obj))
//
//        return dict [ yield! functionHostNames ]
    }

[<EntryPoint>]
let main _ = Deployment.runAsync infra
