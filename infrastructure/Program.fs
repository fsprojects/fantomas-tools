module Program

open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
open Pulumi.FSharp
open Pulumi.Azure.Core
open Pulumi.Azure.Storage

let infra() =
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

    let genericFunctionAppSettings =
        inputMap
            [ "FUNCTIONS_WORKER_RUNTIME", input "DotNet"
              "APPINSIGHTS_INSTRUMENTATIONKEY", io applicationsInsight.InstrumentationKey ]
    let genericSiteConfig =
        input
            (FunctionAppSiteConfigArgs
                (Http2Enabled = input true,
                 Cors =
                     input
                         (FunctionAppSiteConfigCorsArgs
                             (AllowedOrigins =
                                 inputList
                                     [ input "https://nojaf.be" // temporary
                                       input "http://localhost:8080" ]))))

    let fantomasOnlineApp =
        FunctionApp
            ("azfun-fantomas-online-plan",
             FunctionAppArgs
                 (ResourceGroupName = io resourceGroup.Name,
                  Name = input (sprintf "azfun-fantomas-online-%s" stackName), AppServicePlanId = io appServicePlan.Id,
                  StorageConnectionString = io storageAccount.PrimaryConnectionString,
                  AppSettings = genericFunctionAppSettings, SiteConfig = genericSiteConfig, HttpsOnly = input true,
                  Version = input "~3"))

    let fantomasOnlinePreviewApp() = failwith "meh"
    let fantomasOnlineVersionTwoApp() = failwith "meh"
    let fsharpTokensApp() = failwith "meh"
    let astViewerApp() = failwith "meh"
    let triviaApp() = failwith "meh"

    // Export the connection string for the storage account
    dict
        [ ("connectionString", storageAccount.PrimaryConnectionString :> obj)
          ("fantomasOnlineAppHostName", fantomasOnlineApp.DefaultHostname :> obj) ]

[<EntryPoint>]
let main _ = Deployment.run infra
