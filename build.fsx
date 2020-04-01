#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.JavaScript
open Fake.Tools

module Func =
    type HostOptions =
        { Cors: string
          Port: int
          WorkingDirectory: string }
    let host (hostOptions: HostOptions): unit =
        let funcPath = ProcessUtils.findPath [] "func"
        let parameters = [ "start"
                           "--cors"; hostOptions.Cors
                           "--port"; hostOptions.Port.ToString() ]

        CreateProcess.fromRawCommand funcPath parameters
        |> CreateProcess.withWorkingDirectory hostOptions.WorkingDirectory
        |> Proc.run
        |> ignore


let fablePort = 9060
let fsharpTokensPort = 7899
let astPort = 7412
let triviaPort = 9856
let fantomasMasterPort = 1256
let fantomasV2Port = 2568
let fantomasStablePort = 9091

let localhostBackend port = sprintf "http://localhost:%i" port

let clientDir = __SOURCE_DIRECTORY__  </> "src" </> "client"
let serverDir = __SOURCE_DIRECTORY__ </> "src" </> "server"

Target.create "Fantomas-Git" (fun _ ->
    let targetDir = ".deps" @@ "fantomas"
    Fake.IO.Shell.cleanDir targetDir
    Git.Repository.cloneSingleBranch "." "https://github.com/fsprojects/fantomas.git" "master" targetDir
    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "tool" "restore" |> ignore
    DotNet.build (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release }) "./.deps/fantomas/src/Fantomas/Fantomas.fsproj"
)

Target.create "Build" ignore

Target.create "Watch" (fun _ ->

   Environment.setEnvironVar "FSHARP_TOKENS_BACKEND" (localhostBackend fsharpTokensPort)
   Environment.setEnvironVar "AST_BACKEND" (localhostBackend astPort)
   Environment.setEnvironVar "TRIVIA_BACKEND" (localhostBackend triviaPort)
   Environment.setEnvironVar "FRONTEND_PORT" (fablePort.ToString())

   let fable = async { Yarn.exec "start" (fun opt -> { opt with WorkingDirectory = clientDir }) }

   let cors = localhostBackend fablePort
   let hostAzureFunction name port = async { Func.host { Cors = cors; Port = port; WorkingDirectory = serverDir </> name } }

   let fsharpTokens = hostAzureFunction "FSharpTokens" fsharpTokensPort
   let astViewer = hostAzureFunction "ASTViewer" astPort
   let triviaViewer = hostAzureFunction "TriviaViewer" triviaPort

   Async.Parallel [ fable; fsharpTokens; astViewer; triviaViewer ]
   |> Async.Ignore
   |> Async.RunSynchronously
)

Target.runOrDefault "Build"