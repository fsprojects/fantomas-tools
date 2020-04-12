module Program

open Pulumi
open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
open Pulumi.Azure.Core
open Pulumi.Azure.Storage
open Pulumi.FSharp
open System.IO

let infra () =
    let stackName = Pulumi.Deployment.Instance.StackName

    // Create an Azure Resource Group
    let resourceGroupArgs = ResourceGroupArgs(Name = input (sprintf "rg-fantomas-%s" stackName))
    let resourceGroup = ResourceGroup(sprintf "rg-fantomas-%s" stackName, args = resourceGroupArgs)

    // Create an Azure Storage Account
    let storageAccount =
        Account
            ("storagefantomas",
             AccountArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "storfantomas%s" stackName),
                  AccountReplicationType = input "LRS", AccountTier = input "Standard"))

    // container for zips
    let zipContainer =
        Container
            ("zips",
             ContainerArgs
                 (Name = input "zips", StorageAccountName = io storageAccount.Name,
                  ContainerAccessType = input "private"))

    // Create Application Insights
    let applicationsInsight =
        Insights
            ("ai-fantomas",
             InsightsArgs
                 (ResourceGroupName = io resourceGroup.Name, Name = input (sprintf "ai-fantomas-%s" stackName),
                  ApplicationType = input "web"))


    let appServicePlan =
        Plan
            ("azfun-fantomas",
             PlanArgs
                 (ResourceGroupName = io resourceGroup.Name, Kind = input "FunctionApp",
                  Sku = input (PlanSkuArgs(Tier = input "Dynamic", Size = input "Y1")),
                  Name = input (sprintf "azfun-fantomas-plan-%s" stackName)))

    let genericSiteConfig =
        input
            (FunctionAppSiteConfigArgs
                (Http2Enabled = input true,
                 Cors = input
                            (FunctionAppSiteConfigCorsArgs(AllowedOrigins = inputList [ input "https://nojaf.com" ]))))

    let artifactsFolder = @"C:\Users\nojaf\Projects\fantomas-tools\artifacts"

    let toPascalCase (v: string) =
        v.Split('-')
        |> Array.map (fun piece ->
            if String.length piece > 3
            then piece.[0].ToString().ToUpper() + piece.Substring(1)
            else piece.ToUpper())
        |> String.concat ""

    let functionHostNames =
        [ "fantomas-online-latest"
          "fantomas-online-previous"
          "fantomas-online-preview"
          "ast-viewer"
          "fsharp-tokens"
          "trivia-viewer" ]
        |> List.map (fun funcName ->
            let path = Path.Combine(artifactsFolder, (toPascalCase funcName))
            let archive: AssetOrArchive = FileArchive(path) :> AssetOrArchive
            let blob =
                Blob
                    (sprintf "%s-zip" funcName,
                     BlobArgs
                         (StorageAccountName = io storageAccount.Name, StorageContainerName = io zipContainer.Name,
                          Type = input "Block", Source = input archive))

            let codeBlobUrl = SharedAccessSignature.SignedBlobReadUrl(blob, storageAccount)

            let functionAppSettings =
                inputMap
                    [ "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
                      "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey
                      "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl ]

            let funcApp =
                FunctionApp
                    (sprintf "azfun-%s-plan" funcName,
                     FunctionAppArgs
                         (ResourceGroupName = io resourceGroup.Name,
                          Name = input (sprintf "azfun-%s-%s" funcName stackName),
                          AppServicePlanId = io appServicePlan.Id,
                          StorageConnectionString = io storageAccount.PrimaryConnectionString,
                          AppSettings = functionAppSettings, SiteConfig = genericSiteConfig, HttpsOnly = input true,
                          Version = input "~3"))

            (sprintf "%s-app-host-name" funcName, funcApp.DefaultHostname :> obj))

    dict
        [ yield ("connectionString", storageAccount.PrimaryConnectionString :> obj)
          yield! functionHostNames ]

[<EntryPoint>]
let main _ = Deployment.run infra
