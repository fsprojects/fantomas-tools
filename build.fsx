#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fake.Tools
open System.IO

module Azure =
    let az parameters =
        let azPath = ProcessUtils.findPath [] "az"
        CreateProcess.fromRawCommand azPath parameters
        |> Proc.run
        |> ignore

module Func =
    type HostOptions =
        { Cors: string
          Port: int
          WorkingDirectory: string }

    let host (hostOptions: HostOptions): unit =
        let funcPath = ProcessUtils.findPath [] "func"

        let parameters =
            [ "start"
              "--csharp"
              "--cors"
              hostOptions.Cors
              "--port"
              hostOptions.Port.ToString()
              "--worker-runtime"
              "dotnetIsolated" ]

        CreateProcess.fromRawCommand funcPath parameters
        |> CreateProcess.withWorkingDirectory hostOptions.WorkingDirectory
        |> Proc.run
        |> ignore


let fablePort = 9060
let fsharpTokensPort = 7899
let astPort = 7412
let triviaPort = 9856
let fantomasPreviewPort = 11084
let fantomasV2Port = 2568
let fantomasV3Port = 9007
let fantomasV4Port = 10707

let clientDir = __SOURCE_DIRECTORY__ </> "src" </> "client"
let setClientDir = (fun (opt: Yarn.YarnParams) -> { opt with WorkingDirectory = clientDir })
let serverDir = __SOURCE_DIRECTORY__ </> "src" </> "server"
let artifactDir = __SOURCE_DIRECTORY__ </> "artifacts"
let infrastructureDir = __SOURCE_DIRECTORY__ </> "infrastructure"

Target.create "Fantomas-Git" (fun _ ->
    let targetDir = ".deps" @@ "fantomas"

    if System.IO.Directory.Exists(targetDir) then
        Git.Branches.pull targetDir "origin" "master"
    else
        Git.Repository.cloneSingleBranch "." "https://github.com/fsprojects/fantomas.git" "master" targetDir

    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "tool" "restore" |> ignore
    DotNet.build (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release }) "./.deps/fantomas/src/Fantomas/Fantomas.fsproj"
)

Target.create "Clean" (fun _ ->
    Shell.rm_rf artifactDir
    !!(serverDir + "/*/bin")
    ++(serverDir + "/*/obj") 
    ++(clientDir + "/src/bin")
    ++(clientDir + "/build")
    |> Seq.iter Shell.rm_rf
)

Target.create "Build" (fun _ ->
    [ "FSharpTokens"; "ASTViewer"; "TriviaViewer"; "FantomasOnlineV2"; "FantomasOnlineV3"; "FantomasOnlineV4"; "FantomasOnlinePreview" ]
    |> List.iter (fun project ->
        DotNet.build (fun config -> { config with Configuration = DotNet.BuildConfiguration.Release })
            (sprintf "%s/%s/%s.fsproj" serverDir project project)))

let watchMode getBackendUrl getCorsUrl =
    Environment.setEnvironVar "NODE_ENV" "development"
    Environment.setEnvironVar "VITE_FSHARP_TOKENS_BACKEND" (getBackendUrl fsharpTokensPort)
    Environment.setEnvironVar "VITE_AST_BACKEND" (getBackendUrl astPort)
    Environment.setEnvironVar "VITE_TRIVIA_BACKEND" (getBackendUrl triviaPort)
    Environment.setEnvironVar "VITE_FANTOMAS_V2" (getBackendUrl fantomasV2Port)
    Environment.setEnvironVar "VITE_FANTOMAS_V3" (getBackendUrl fantomasV3Port)
    Environment.setEnvironVar "VITE_FANTOMAS_V4" (getBackendUrl fantomasV4Port)
    Environment.setEnvironVar "VITE_FANTOMAS_PREVIEW" (getBackendUrl fantomasPreviewPort)
    Environment.setEnvironVar "VITE_FRONTEND_PORT" (fablePort.ToString())

    let vite = async { Yarn.exec "start" (setClientDir) }
    let fable = async { DotNet.exec (fun config -> { config with WorkingDirectory = clientDir }) "fable" "watch ./fsharp/FantomasTools.fsproj --outDir ./src/bin" |> ignore }
    let cors = getCorsUrl fablePort //sprintf "https://localhost:%i" fablePort

    let hostAzureFunction name port =
        async {
            Func.host
                { Cors = cors
                  Port = port
                  WorkingDirectory = serverDir </> name }
        }

    let fsharpTokens = hostAzureFunction "FSharpTokens" fsharpTokensPort
    let astViewer = hostAzureFunction "ASTViewer" astPort
    let triviaViewer = hostAzureFunction "TriviaViewer" triviaPort
    let fantomasV2 = hostAzureFunction "FantomasOnlineV2" fantomasV2Port
    let fantomasV3 = hostAzureFunction "FantomasOnlineV3" fantomasV3Port
    let fantomasV4 = hostAzureFunction "FantomasOnlineV4" fantomasV4Port
    let fantomasPreview = hostAzureFunction "FantomasOnlinePreview" fantomasPreviewPort

    Async.Parallel [ vite; fable; fsharpTokens; astViewer; triviaViewer; fantomasV2; fantomasV3; fantomasV4; fantomasPreview ]
    |> Async.Ignore
    |> Async.RunSynchronously

Target.create "Watch" (fun _ ->
    let localhostBackend port = sprintf "http://localhost:%i" port
    let cors = sprintf "https://localhost:%i"
    watchMode localhostBackend cors)

Target.create "GitPod" (fun _ ->
    let gitpodWorkspaceUrl = System.Environment.GetEnvironmentVariable "GITPOD_WORKSPACE_URL"
    let gitpod = gitpodWorkspaceUrl.Replace("https://", System.String.Empty)
    let getBackendUrl port = sprintf "https://%i-%s" port gitpod
    let getCorsUrl = getBackendUrl
    watchMode getBackendUrl getCorsUrl
)

Target.create "DeployFunctions" (fun _ ->
    ["FantomasOnlineV2"; "FantomasOnlineV3"; "FantomasOnlineV4"; "FantomasOnlinePreview"; "ASTViewer"; "FSharpTokens"; "TriviaViewer"]
    |> List.iter (fun project ->
        let output = artifactDir </> project
        DotNet.publish
            (fun config -> { config with
                                Configuration = DotNet.BuildConfiguration.Release
                                OutputPath = Some output })
            (sprintf "%s/%s/%s.fsproj" serverDir project project)
    )
)

Target.create "YarnInstall" (fun _ -> Yarn.install setClientDir)

Target.create "NETInstall" (fun _ -> DotNet.restore id "fantomas-tools.sln")

Target.create "BundleFrontend" (fun _ ->
    Environment.setEnvironVar "NODE_ENV" "production"
    Environment.setEnvironVar "VITE_FSHARP_TOKENS_BACKEND" "https://azfun-fsharp-tokens-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_AST_BACKEND" "https://azfun-ast-viewer-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_TRIVIA_BACKEND" "https://azfun-trivia-viewer-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_FANTOMAS_V2" "https://azfun-fantomas-online-v2-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_FANTOMAS_V3" "https://azfun-fantomas-online-v3-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_FANTOMAS_V4" "https://azfun-fantomas-online-v4-main.azurewebsites.net"
    Environment.setEnvironVar "VITE_FANTOMAS_PREVIEW" "https://azfun-fantomas-online-preview-main.azurewebsites.net"

    Yarn.exec "build" setClientDir
)

Target.create "Format" (fun _ ->
    let result = DotNet.exec id "fantomas" "src -r"
    if not result.OK then
        printfn "Errors while formatting all files: %A" result.Messages)

Target.create "FormatChanged" (fun _ ->
    Fake.Tools.Git.FileStatus.getChangedFilesInWorkingCopy "." "HEAD"
    |> Seq.choose (fun (_, file) ->
        let ext = Path.GetExtension(file)

        if file.StartsWith("src")
           && (ext = ".fs" || ext = ".fsi") then
            Some file
        else
            None)
    |> Seq.map (fun file -> async {
        let result = DotNet.exec id "fantomas" file
        if not result.OK then
            printfn "Problem when formatting %s:\n%A" file result.Errors
    })
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore)

Target.create "CheckFormat" (fun _ ->
    let result =
        DotNet.exec id "fantomas" "src -r --check"

    if result.ExitCode = 0 then
        Trace.log "No files need formatting"
    elif result.ExitCode = 99 then
        failwith "Some files need formatting, check output for more info"
    else
        Trace.logf "Errors while formatting: %A" result.Errors)

Target.create "CleanInfrastructure" (fun _ ->
    Shell.rm_rf (infrastructureDir </> "bin")
    Shell.rm_rf (infrastructureDir </> "obj")
)

Target.create "Install" ignore
Target.create "CI" ignore
Target.create "PR" ignore

open Fake.Core.TargetOperators

"Clean" ==> "Build"

"YarnInstall" ==> "BundleFrontend"

"Fantomas-Git" ==> "NETInstall"

"Install" <== [ "YarnInstall"; "NETInstall" ]

"CI"
    <== [ "CleanInfrastructure"; "BundleFrontend"; "DeployFunctions"; "Clean"; "Fantomas-Git"; "CheckFormat" ]

"PR" 
    <== [ "BundleFrontend"; "Build"; "Clean"; "Fantomas-Git"; "CheckFormat" ]

Target.runOrDefault "Build"
