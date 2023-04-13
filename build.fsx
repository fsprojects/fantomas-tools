#r "nuget: Fun.Build, 0.2.9"
#r "nuget: CliWrap, 3.5.0"
#r "nuget: Fake.IO.FileSystem, 5.23.0"

open System
open System.IO
open System.Threading.Tasks
open CliWrap
open CliWrap.Buffered
open Fun.Build
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

let fablePort = 9060
let astPort = 7412
let oakPort = 8904
let fantomasMainPort = 11084
let fantomasPreviewPort = 12007
let fantomasV4Port = 10707
let fantomasV5Port = 11009
let fantomasV6Port = 13042
let pwd = __SOURCE_DIRECTORY__
let fantomasDepDir = pwd </> ".deps" </> "fantomas"
let v6DepDir = pwd </> ".deps" </> "v6.0"
let clientDir = pwd </> "src" </> "client"
let serverDir = __SOURCE_DIRECTORY__ </> "src" </> "server"
let artifactDir = __SOURCE_DIRECTORY__ </> "artifacts"

let git (arguments: string) workingDir =
    async {
        let! result =
            Cli
                .Wrap("git")
                .WithArguments(arguments)
                .WithWorkingDirectory(workingDir)
                .ExecuteBufferedAsync()
                .Task
            |> Async.AwaitTask

        return (result.ExitCode, result.StandardOutput)
    }

let setEnv name value =
    Environment.SetEnvironmentVariable(name, value)

pipeline "Fantomas-Git" {
    stage "git" {
        paralle
        run (fun _ ->
            async {
                let branch = "main"

                if Directory.Exists(fantomasDepDir) then
                    let! exitCode, _ = git "pull" fantomasDepDir
                    return exitCode
                else
                    let! exitCode, _ =
                        git
                            $"clone -b {branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/fantomas"
                            __SOURCE_DIRECTORY__
                    return exitCode
            })
    // run (fun _ ->
    //     async {
    //         let branch = "v6.0"

    //         if Directory.Exists(v6DepDir) then
    //             let! exitCode, _ = git "pull" v6DepDir
    //             return exitCode
    //         else
    //             let! exitCode, _ =
    //                 git
    //                     $"clone -b {branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/v6.0"
    //                     __SOURCE_DIRECTORY__
    //             return exitCode
    //     })
    }
    stage "build" {
        paralle
        stage "build fantomas main" {
            workingDir fantomasDepDir
            run "dotnet fsi build.fsx -p Init"
            run "dotnet build src/Fantomas.Core"
        }
    // stage "build fantomas preview" {
    //     workingDir v6DepDir
    //     run "dotnet fsi build.fsx -p Init"
    //     run "dotnet build src/Fantomas.Core"
    // }
    }
    runIfOnlySpecified true
}

let publishLambda name =
    $"dotnet publish -c Release -o {artifactDir </> name} {serverDir}/{name}/{name}.fsproj"

let runLambda name =
    $"dotnet watch run --project {serverDir </> name </> name}.fsproj"

let setViteToProduction () =
    setEnv "NODE_ENV" "production"

    let mainStageUrl =
        "https://arlp8cgo97.execute-api.eu-west-1.amazonaws.com/fantomas-main-stage-1c52a6a"

    setEnv "VITE_AST_BACKEND" $"{mainStageUrl}/ast-viewer"
    setEnv "VITE_OAK_BACKEND" $"{mainStageUrl}/oak-viewer"
    setEnv "VITE_FANTOMAS_V4" $"{mainStageUrl}/fantomas/v4"
    setEnv "VITE_FANTOMAS_V5" $"{mainStageUrl}/fantomas/v5"
    setEnv "VITE_FANTOMAS_V6" $"{mainStageUrl}/fantomas/v6"
    setEnv "VITE_FANTOMAS_MAIN" $"{mainStageUrl}/fantomas/main"
    setEnv "VITE_FANTOMAS_PREVIEW" $"{mainStageUrl}/fantomas/preview"

pipeline "Build" {
    workingDir __SOURCE_DIRECTORY__
    stage "yarn install" {
        workingDir clientDir
        run "yarn"
    }
    stage "dotnet install" {
        run "dotnet tool restore"
        run "dotnet restore"
    }
    stage "check format F#" { run "dotnet fantomas src infrastructure build.fsx --check" }
    stage "check format JS" {
        workingDir clientDir
        run "yarn lint"
    }
    stage "clean" {
        run (fun _ ->
            async {
                Shell.rm_rf artifactDir
                !!(serverDir + "/*/bin")
                ++ (serverDir + "/*/obj")
                ++ (clientDir + "/src/bin")
                ++ (clientDir + "/build")
                |> Seq.iter Shell.rm_rf
                return 0
            })
    }
    stage "publish lambdas" {
        stage "parallel ones" {
            paralle
            run (publishLambda "FantomasOnlineV4")
            run (publishLambda "FantomasOnlineV5")
            run (publishLambda "FantomasOnlineV6")
            run (publishLambda "ASTViewer")
        }
        run (publishLambda "FantomasOnlineMain")
        run (publishLambda "FantomasOnlinePreview")
        run (publishLambda "OakViewer")
    }
    stage "bundle frontend" {
        workingDir clientDir
        run "dotnet tool restore"
        run (fun _ ->
            async {
                setViteToProduction ()
                return 0
            })
        run "yarn build"
        run (fun _ ->
            async {
                File.Create(clientDir </> "build" </> ".nojekyll").Close()
                Shell.cp_r (clientDir </> "build") (artifactDir </> "client")
                return 0
            })
    }
    runIfOnlySpecified false
}

let changedFiles () : Async<string array> =
    async {
        let! exitCode, stdout = git "status --porcelain" pwd
        if exitCode <> 0 then
            return failwithf $"could not check the git status: %s{stdout}"
        else
            return
                stdout.Split('\n')
                |> Array.choose (fun line ->
                    let line = line.Trim()
                    if (line.StartsWith("AM") || line.StartsWith("M")) then
                        Some(line.Replace("AM ", "").Replace("M ", ""))
                    else
                        None)
    }

let fsharpExtensions = set [| ".fs"; ".fsi"; ".fsx" |]
let jsExtensions = set [| ".js"; ".jsx" |]
let isFSharpFile path =
    FileInfo(path).Extension |> fsharpExtensions.Contains
let isJSFile path =
    FileInfo(path).Extension |> jsExtensions.Contains

pipeline "FormatChanged" {
    workingDir __SOURCE_DIRECTORY__
    stage "Format" {
        run (fun _ ->
            async {
                let! files = changedFiles ()
                let fantomasArgument = files |> Array.filter isFSharpFile |> String.concat " "

                let! fantomasExit =
                    if String.IsNullOrWhiteSpace fantomasArgument then
                        async.Return 0
                    else
                        Cli
                            .Wrap("dotnet")
                            .WithArguments($"fantomas {fantomasArgument}")
                            .ExecuteAsync()
                            .Task.ContinueWith(fun (t: Task<CommandResult>) -> t.Result.ExitCode)
                        |> Async.AwaitTask

                let prettierArgument =
                    files
                    |> Array.choose (fun path ->
                        if isJSFile path then
                            Some(path.Replace("src/client/", ""))
                        else
                            None)
                    |> String.concat " "

                let! prettierExit =
                    if String.IsNullOrWhiteSpace prettierArgument then
                        async.Return 0
                    else
                        Cli
                            .Wrap("yarn")
                            .WithWorkingDirectory(clientDir)
                            .WithArguments($"prettier --write {prettierArgument}")
                            .ExecuteBufferedAsync()
                            .Task.ContinueWith(fun (t: Task<BufferedCommandResult>) -> t.Result.ExitCode)
                        |> Async.AwaitTask

                // Exit code should in both cases by zero
                return fantomasExit + prettierExit
            }
            |> Async.RunSynchronously)
    }
    runIfOnlySpecified true
}

pipeline "Watch" {
    stage "yarn install" {
        workingDir clientDir
        run "yarn"
    }
    stage "dotnet install" {
        run "dotnet tool restore"
        run "dotnet restore"
    }
    stage "prepare environment variables" {
        run (fun _ ->
            async {
                let localhostBackend port subPath =
                    let gitpodEnv = Environment.GetEnvironmentVariable("GITPOD_WORKSPACE_URL")

                    if String.IsNullOrWhiteSpace(gitpodEnv) then
                        sprintf "http://localhost:%i/%s" port subPath
                    else
                        let gitpodEnv = gitpodEnv.Replace("https://", "")
                        sprintf "https://%i-%s/%s" port gitpodEnv subPath

                setEnv "NODE_ENV" "development"
                setEnv "VITE_AST_BACKEND" (localhostBackend astPort "ast-viewer")
                setEnv "VITE_OAK_BACKEND" (localhostBackend oakPort "oak-viewer")
                setEnv "VITE_FANTOMAS_V4" (localhostBackend fantomasV4Port "fantomas/v4")
                setEnv "VITE_FANTOMAS_V5" (localhostBackend fantomasV5Port "fantomas/v5")
                setEnv "VITE_FANTOMAS_V6" (localhostBackend fantomasV6Port "fantomas/v6")
                setEnv "VITE_FANTOMAS_MAIN" (localhostBackend fantomasMainPort "fantomas/main")
                setEnv "VITE_FANTOMAS_PREVIEW" (localhostBackend fantomasPreviewPort "fantomas/preview")
                return 0
            })
    }
    stage "launch services" {
        paralle
        run (runLambda "ASTViewer")
        run (runLambda "OakViewer")
        run (runLambda "FantomasOnlineV4")
        run (runLambda "FantomasOnlineV5")
        run (runLambda "FantomasOnlineV6")
        run (runLambda "FantomasOnlineMain")
        run (runLambda "FantomasOnlinePreview")
        stage "frontend" {
            workingDir clientDir
            run "dotnet tool restore"
            run "dotnet fable watch ./fsharp/FantomasTools.fsproj --outDir ./src/bin --run vite"
        }
    }
    runIfOnlySpecified true
}
