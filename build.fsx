#r "nuget: Fun.Build, 0.1.9"
#r "nuget: CliWrap, 3.5.0"
#r "nuget: Fake.IO.FileSystem, 5.23.0"

open System
open System.IO
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
let pwd = __SOURCE_DIRECTORY__
let fantomasDepDir = pwd </> ".deps" </> "fantomas"
let dallasDepDir = pwd </> ".deps" </> "dallas"
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

let cmd file (arguments: string) =
    async {
        let! result = Cli.Wrap(file).WithArguments(arguments).ExecuteAsync().Task |> Async.AwaitTask
        return result.ExitCode
    }

let setEnv name value =
    Environment.SetEnvironmentVariable(name, value)

pipeline "Fantomas-Git" {
    stage "get fantomas source code" {
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
    }
    stage "build fantomas" {
        workingDir fantomasDepDir
        run "dotnet fsi build.fsx -p Init"
        run "dotnet build src/Fantomas.Core"
    }
    // stage "get project Dallas source code" {
    //     run (fun _ ->
    //         async {
    //             let branch = "v5.2"
    //
    //             if Directory.Exists(dallasDepDir) then
    //                 let! exitCode, _ = git "pull" dallasDepDir
    //                 return exitCode
    //             else
    //                 let! exitCode, _ =
    //                     git
    //                         $"clone -b {branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/dallas"
    //                         __SOURCE_DIRECTORY__
    //                 return exitCode
    //         })
    // }
    // stage "build fantomas" {
    //     workingDir dallasDepDir
    //     run "dotnet fsi build.fsx -p Init"
    //     run "dotnet build -c Release src/Fantomas.Core"
    // }
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
    stage "check format" { run "dotnet fantomas src infrastructure build.fsx -r --check" }
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

pipeline "FormatChanged" {
    workingDir __SOURCE_DIRECTORY__
    stage "Format" {
        run (fun _ ->
            async {
                let! exitCode, stdout = git "status --porcelain" pwd
                if exitCode <> 0 then
                    return exitCode
                else
                    let files =
                        stdout.Split('\n')
                        |> Array.choose (fun line ->
                            let line = line.Trim()
                            if
                                (line.StartsWith("AM") || line.StartsWith("M"))
                                && (line.EndsWith(".fs") || line.EndsWith(".fsx") || line.EndsWith(".fsi"))
                            then
                                Some(line.Replace("AM ", "").Replace("M ", ""))
                            else
                                None)
                        |> String.concat " "
                    return! cmd "dotnet" $"fantomas {files}"
            })
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
