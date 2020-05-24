module Program

open System.Net.Http
open System.Net.Http.Headers
open Pulumi
open Pulumi.Azure.AppInsights
open Pulumi.Azure.AppService
open Pulumi.Azure.AppService.Inputs
open Pulumi.Azure.Core
open Pulumi.Azure.Storage
open Pulumi.FSharp
open System.IO
open Thoth.Json.Net

let private commitDecoder: Decoder<string*string> =
    Decode.object (fun get ->
        let sha = get.Required.Field "sha" Decode.string
        let timestamp = get.Required.At ["commit";"author";"date"] Decode.string
        sha, timestamp
    )

let private getLastCommit () =
    async {
        use httpClient = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/fsprojects/fantomas/commits")
        request.Headers.CacheControl <- CacheControlHeaderValue.Parse("no-cache")
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36")

        let! response =
            httpClient.SendAsync(request)
            |> Async.AwaitTask

        let! body =
            response.Content.ReadAsStringAsync()
            |> Async.AwaitTask

        let decodeResult =
            match Decode.fromString (Decode.list commitDecoder) body with
            | Ok (t::_)
            | Ok ([t]) ->
                printfn "Last commit: %A" t
                Some t
            | Ok [] -> None
            | Error e ->
                printfn "Could not get last commit from GitHub: %A" e
                None

        return decodeResult
    }

let infra () =
    async {
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
                                (FunctionAppSiteConfigCorsArgs(AllowedOrigins = inputList [ input "https://fsprojects.github.io" ]))))

        let artifactsFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "artifacts")

        printfn "Current directory: %s" artifactsFolder

        let toPascalCase (v: string) =
            v.Split('-')
            |> Array.map (fun piece ->
                if piece = "fsharp" then
                    "FSharp"
                elif String.length piece > 3 then
                    piece.[0].ToString().ToUpper() + piece.Substring(1)
                else
                    piece.ToUpper())
            |> String.concat ""

        let! lastCommit = getLastCommit ()
        let lastCommitAppSettings =
            match lastCommit with
            | Some (sha, timestamp) -> [ "LAST_COMMIT_SHA", input sha
                                         "LAST_COMMIT_TIMESTAMP", input timestamp ]
            | None -> []

        let functionHostNames =
            [ "fantomas-online-latest"
              "fantomas-online-previous"
              "fantomas-online-preview"
              "ast-viewer"
              "fsharp-tokens"
              "trivia-viewer" ]
            |> List.map (fun funcName ->
                let path = Path.Combine(artifactsFolder, (toPascalCase funcName))
                printfn "PATH: %s" path
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
                          "WEBSITE_RUN_FROM_PACKAGE", io codeBlobUrl
                          yield! lastCommitAppSettings ]

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

        return dict
                    [ yield ("connectionString", storageAccount.PrimaryConnectionString :> obj)
                      yield! functionHostNames ]
    }

[<EntryPoint>]
let main _ = Deployment.runAsync infra
