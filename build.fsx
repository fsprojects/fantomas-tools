#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open System.IO
open System.Threading
open CliWrap
open CliWrap.EventStream
open FSharp.Control.Reactive
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fake.Tools

let fablePort = 9060
let fsharpTokensPort = 7899
let astPort = 7412
let triviaPort = 9856
let fantomasPreviewPort = 11084
let fantomasV2Port = 2568
let fantomasV3Port = 9007
let fantomasV4Port = 10707

let clientDir =
    __SOURCE_DIRECTORY__ </> "src" </> "client"

let setClientDir =
    (fun (opt: Yarn.YarnParams) -> { opt with WorkingDirectory = clientDir })

let serverDir =
    __SOURCE_DIRECTORY__ </> "src" </> "server"

let artifactDir = __SOURCE_DIRECTORY__ </> "artifacts"

Target.create "Fantomas-Git" (fun _ ->
    let targetDir = ".deps" @@ "fantomas"

    if Directory.Exists(targetDir) then
        Git.Branches.pull targetDir "origin" "4.6"
    else
        Git.Repository.cloneSingleBranch "." "https://github.com/fsprojects/fantomas.git" "4.6" targetDir

    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "tool" "restore"
    |> ignore

    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "paket" "restore"
    |> ignore

    DotNet.build
        (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release })
        "./.deps/fantomas/src/Fantomas/Fantomas.fsproj")

Target.create "Clean" (fun _ ->
    Shell.rm_rf artifactDir

    !!(serverDir + "/*/bin")
    ++ (serverDir + "/*/obj")
    ++ (clientDir + "/src/bin")
    ++ (clientDir + "/build")
    |> Seq.iter Shell.rm_rf)

Target.create "Build" (fun _ ->
    [ "FSharpTokens"
      "ASTViewer"
      "TriviaViewer"
      "FantomasOnlineV2"
      "FantomasOnlineV3"
      "FantomasOnlineV4"
      "FantomasOnlinePreview" ]
    |> List.iter (fun project ->
        DotNet.build
            (fun config -> { config with Configuration = DotNet.BuildConfiguration.Release })
            (sprintf "%s/%s/%s.fsproj" serverDir project project)))

Target.create "Watch" (fun target ->
    let cts =
        CancellationTokenSource.CreateLinkedTokenSource(target.Context.CancellationToken)

    let localhostBackend port subPath =
        sprintf "http://localhost:%i/%s" port subPath

    Environment.setEnvironVar "NODE_ENV" "development"
    Environment.setEnvironVar "VITE_FSHARP_TOKENS_BACKEND" (localhostBackend fsharpTokensPort "fsharp-tokens")
    Environment.setEnvironVar "VITE_AST_BACKEND" (localhostBackend astPort "ast-viewer")
    Environment.setEnvironVar "VITE_TRIVIA_BACKEND" (localhostBackend triviaPort "trivia-viewer")
    Environment.setEnvironVar "VITE_FANTOMAS_V2" (localhostBackend fantomasV2Port "fantomas/v2")
    Environment.setEnvironVar "VITE_FANTOMAS_V3" (localhostBackend fantomasV3Port "fantomas/v3")
    Environment.setEnvironVar "VITE_FANTOMAS_V4" (localhostBackend fantomasV4Port "fantomas/v4")
    Environment.setEnvironVar "VITE_FANTOMAS_PREVIEW" (localhostBackend fantomasPreviewPort "fantomas/preview")

    let mapEvents (name: string) (observable: IObservable<CommandEvent>) =
        Observable.map (fun event -> name, event) observable

    let frontend =
        Cli
            .Wrap(DotNet.Options.Create().DotNetCliPath)
            .WithArguments("fable watch ./fsharp/FantomasTools.fsproj --outDir ./src/bin --run vite")
            .WithWorkingDirectory(clientDir)
            .Observe(cts.Token)
        |> mapEvents "vite/fable"

    let runLambda (directory: string) =
        Cli
            .Wrap(DotNet.Options.Create().DotNetCliPath)
            .WithArguments("watch run")
            .WithWorkingDirectory(serverDir </> directory)
            .Observe(cts.Token)

        |> mapEvents directory

    let fsharpTokens = runLambda "FSharpTokens"
    let astViewer = runLambda "ASTViewer"
    let triviaViewer = runLambda "TriviaViewer"
    let fantomasV2 = runLambda "FantomasOnlineV2"
    let fantomasV3 = runLambda "FantomasOnlineV3"
    let fantomasV4 = runLambda "FantomasOnlineV4"
    let fantomasPreview = runLambda "FantomasOnlinePreview"

    let subscription =
        Observable.mergeArray
            [| frontend
               fsharpTokens
               astViewer
               triviaViewer
               fantomasV2
               fantomasV3
               fantomasV4
               fantomasPreview |]
        |> Observable.observeOn System.Reactive.Concurrency.ThreadPoolScheduler.Instance
        |> Observable.subscribe (fun (name, event: CommandEvent) ->
            let info (msg: string) =
                Trace.logToConsole ($"{name}: {msg}", Trace.EventLogEntryType.Information)

            let error (msg: string) =
                Trace.logToConsole ($"{name}: {msg}", Trace.EventLogEntryType.Error)

            match event with
            | :? StartedCommandEvent -> info "started"
            | :? StandardOutputCommandEvent as output -> info output.Text
            | :? StandardErrorCommandEvent as e -> error e.Text
            | :? ExitedCommandEvent -> info "exited"
            | _ -> Trace.logToConsole ($"{name}: unexpected event, {event}", Trace.EventLogEntryType.Other))

    Trace.logToConsole ("Starting watch mode, press any key to exit", Trace.EventLogEntryType.Information)
    let _ = Console.ReadKey()
    subscription.Dispose()
    cts.Cancel())

Target.create "PublishLambdas" (fun _ ->
    [ "FantomasOnlineV2"
      "FantomasOnlineV3"
      "FantomasOnlineV4"
      "FantomasOnlinePreview"
      "ASTViewer"
      "FSharpTokens"
      "TriviaViewer" ]
    |> List.map (fun project ->
        async {
            let output = artifactDir </> project

            do
                DotNet.publish
                    (fun config ->
                        { config with
                            Configuration = DotNet.BuildConfiguration.Release
                            OutputPath = Some output })
                    (sprintf "%s/%s/%s.fsproj" serverDir project project)
        })
    |> Async.Parallel
    |> Async.Ignore
    |> Async.RunSynchronously)

Target.create "YarnInstall" (fun _ -> Yarn.install setClientDir)

Target.create "NETInstall" (fun _ -> DotNet.restore id "fantomas-tools.sln")

let setViteToProduction () =
    Environment.setEnvironVar "NODE_ENV" "production"

    let mainStageUrl =
        "https://arlp8cgo97.execute-api.eu-west-1.amazonaws.com/fantomas-main-stage-1c52a6a"

    Environment.setEnvironVar "VITE_FSHARP_TOKENS_BACKEND" $"{mainStageUrl}/fsharp-tokens"
    Environment.setEnvironVar "VITE_AST_BACKEND" $"{mainStageUrl}/ast-viewer"
    Environment.setEnvironVar "VITE_TRIVIA_BACKEND" $"{mainStageUrl}/trivia-viewer"
    Environment.setEnvironVar "VITE_FANTOMAS_V2" $"{mainStageUrl}/fantomas/v2"
    Environment.setEnvironVar "VITE_FANTOMAS_V3" $"{mainStageUrl}/fantomas/v3"
    Environment.setEnvironVar "VITE_FANTOMAS_V4" $"{mainStageUrl}/fantomas/v4"
    Environment.setEnvironVar "VITE_FANTOMAS_PREVIEW" $"{mainStageUrl}/fantomas/preview"

Target.create "BundleFrontend" (fun _ ->
    setViteToProduction ()
    Yarn.exec "build" setClientDir)

Target.create "RunWithLambdas" (fun target ->
    setViteToProduction ()

    DotNet.exec
        (fun opt -> { opt with WorkingDirectory = clientDir })
        "fable"
        "watch ./fsharp/FantomasTools.fsproj --outDir ./src/bin --run vite"
    |> printfn "%A")

Target.create "Format" (fun _ ->
    let result = DotNet.exec id "fantomas" "src -r"

    if not result.OK then
        printfn "Errors while formatting all files: %A" result.Messages)

Target.create "FormatChanged" (fun _ ->
    Fake.Tools.Git.FileStatus.getChangedFilesInWorkingCopy "." "HEAD"
    |> Seq.choose (fun (_, file) ->
        let ext = Path.GetExtension(file)

        if
            file.StartsWith("src")
            && (ext = ".fs" || ext = ".fsi")
        then
            Some file
        else
            None)
    |> Seq.map (fun file ->
        async {
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

Target.create "Install" ignore
Target.create "CI" ignore
Target.create "PR" ignore

open Fake.Core.TargetOperators

"Clean" ==> "Build"

"YarnInstall" ==> "BundleFrontend"

"Fantomas-Git" ==> "NETInstall"

// "Install" ==> "Watch"

"Install" <== [ "YarnInstall"; "NETInstall" ]

"CI"
<== [ "BundleFrontend"
      "PublishLambdas"
      "Clean"
      "Fantomas-Git" (*; "CheckFormat" *)  ]

"PR"
<== [ "BundleFrontend"
      "Build"
      "Clean"
      "Fantomas-Git" (* "CheckFormat" *)  ]

Target.runOrDefault "Build"
