#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fake.Tools

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
              "--cors"
              hostOptions.Cors
              "--port"
              hostOptions.Port.ToString() ]

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

let localhostBackend port = sprintf "http://localhost:%i" port

let clientDir = __SOURCE_DIRECTORY__ </> "src" </> "client"
let setClientDir = (fun (opt: Yarn.YarnParams) -> { opt with WorkingDirectory = clientDir })
let serverDir = __SOURCE_DIRECTORY__ </> "src" </> "server"
let artifactDir = __SOURCE_DIRECTORY__ </> "artifacts"

Target.create "Fantomas-Git" (fun _ ->
    let targetDir = ".deps" @@ "fantomas"
    Fake.IO.Shell.cleanDir targetDir
    Git.Repository.cloneSingleBranch "." "https://github.com/fsprojects/fantomas.git" "master" targetDir
    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "tool" "restore" |> ignore
    DotNet.build (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release })
        "./.deps/fantomas/src/Fantomas/Fantomas.fsproj"
    //DotNet.build (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release })
    //  "./.deps/fantomas/src/Fantomas.CoreGlobalTool/Fantomas.CoreGlobalTool.fsproj"
)

Target.create "Clean" (fun _ ->
    Shell.rm_rf artifactDir
    !!(serverDir + "/*/bin") |> Seq.iter Shell.rm_rf
    !!(serverDir + "/*/obj") |> Seq.iter Shell.rm_rf)

Target.create "Build" (fun _ ->
    [ "FSharpTokens"; "ASTViewer"; "TriviaViewer"; "FantomasOnlineV2"; "FantomasOnlineV3"; "FantomasOnlineV4"; "FantomasOnlinePreview" ]
    |> List.iter (fun project ->
        DotNet.build (fun config -> { config with Configuration = DotNet.BuildConfiguration.Release })
            (sprintf "%s/%s/%s.fsproj" serverDir project project)))

Target.create "Watch" (fun _ ->

    Environment.setEnvironVar "NODE_ENV" "development"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND" (localhostBackend fsharpTokensPort)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_AST_BACKEND" (localhostBackend astPort)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_TRIVIA_BACKEND" (localhostBackend triviaPort)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V2" (localhostBackend fantomasV2Port)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V3" (localhostBackend fantomasV3Port)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V4" (localhostBackend fantomasV4Port)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_PREVIEW" (localhostBackend fantomasPreviewPort)
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FRONTEND_PORT" (fablePort.ToString())

    let fable = async { Yarn.exec "start" (setClientDir) }
    let cors = sprintf "https://localhost:%i" fablePort

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

    Async.Parallel [ fable; fsharpTokens; astViewer; triviaViewer; fantomasV2; fantomasV3; fantomasV4; fantomasPreview ]
    |> Async.Ignore
    |> Async.RunSynchronously)

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

Target.create "BundleFrontend" (fun _ ->
    Environment.setEnvironVar "NODE_ENV" "production"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND" "https://azfun-fsharp-tokens-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_AST_BACKEND" "https://azfun-ast-viewer-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_TRIVIA_BACKEND" "https://azfun-trivia-viewer-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V2" "https://azfun-fantomas-online-v2-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V3" "https://azfun-fantomas-online-v3-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_V4" "https://azfun-fantomas-online-v4-main.azurewebsites.net"
    Environment.setEnvironVar "SNOWPACK_PUBLIC_FANTOMAS_PREVIEW" "https://azfun-fantomas-online-preview-main.azurewebsites.net"

    Yarn.exec "build" setClientDir
)

Target.create "Format" (fun _ ->
    DotNet.exec id "run" "-p .deps/fantomas/src/Fantomas.CoreGlobalTool/Fantomas.CoreGlobalTool.fsproj -c Release -- --recurse ./src"
    |> fun result ->
        if result.OK then result.Messages else result.Errors
        |> List.iter (printfn "%s"))

Target.create "CheckFormat" (fun _ ->
    DotNet.exec id "run" "-p .deps/fantomas/src/Fantomas.CoreGlobalTool/Fantomas.CoreGlobalTool.fsproj -c Release -- --check --recurse ./src"
    |> fun result ->
        if result.OK then result.Messages else result.Errors
        |> List.iter (printfn "%s")

        if result.ExitCode <> 0 then failwithf "Not everything was formatted")

Target.create "CI" ignore
Target.create "PR" ignore

open Fake.Core.TargetOperators

"Clean" ==> "Build"

"YarnInstall" ==> "BundleFrontend"

"Fantomas-Git" ==> "Clean" ==> "DeployFunctions" ==> "BundleFrontend" ==> "CI"

"Fantomas-Git" ==> "Clean" ==> "Build" ==> "BundleFrontend" ==> "PR"

Target.runOrDefault "Build"
