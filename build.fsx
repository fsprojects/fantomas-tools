#r "nuget: Fun.Build, 1.0.4"
#r "nuget: Fake.IO.FileSystem, 5.23.0"

open System
open System.IO
open Fun.Build
open Fun.Build.Internal
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

let astPort = 7412
let oakPort = 8904
let fantomasMainPort = 11084
let fantomasPreviewPort = 12007
let fantomasV4Port = 10707
let fantomasV5Port = 11009
let fantomasV6Port = 13042
let pwd = __SOURCE_DIRECTORY__
let fantomasDepDir = pwd </> ".deps" </> "fantomas"
let previewBranch = "main"
let previewDepDir = pwd </> ".deps" </> previewBranch
let clientDir = pwd </> "src" </> "client"
let serverDir = __SOURCE_DIRECTORY__ </> "src" </> "server"
let artifactDir = __SOURCE_DIRECTORY__ </> "artifacts"

let alwaysOk = async { return Ok() }

let mapResultToCode =
    function
    | Ok _ -> 0
    | Error _ -> 1

let git (ctx: StageContext) (workingDir: string) (arguments: string) : Async<Result<unit, string>> =
    ctx.RunCommand($"git %s{arguments}", workingDir = workingDir)

let setEnv name value =
    Environment.SetEnvironmentVariable(name, value)

pipeline "Fantomas-Git" {
    stage "git" {
        paralle
        run (fun ctx ->
            async {
                let branch = "main"

                if Directory.Exists(fantomasDepDir) then
                    let! result = git ctx fantomasDepDir "pull"
                    return mapResultToCode result
                else
                    let! result =
                        git
                            ctx
                            __SOURCE_DIRECTORY__
                            $"clone -b {branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/fantomas"

                    return mapResultToCode result
            })
        run (fun ctx ->
            async {
                if Directory.Exists(previewDepDir) then
                    let! result = git ctx previewDepDir "pull"
                    return mapResultToCode result
                else
                    let! result =
                        git
                            ctx
                            __SOURCE_DIRECTORY__
                            $"clone -b {previewBranch} --single-branch https://github.com/fsprojects/fantomas.git .deps/{previewBranch}"

                    return mapResultToCode result
            })
    }
    stage "build" {
        paralle
        stage "build fantomas main" {
            workingDir fantomasDepDir
            run "dotnet fsi build.fsx -p Init"
            run "dotnet build src/Fantomas.Core"
        }
        stage "build fantomas preview" {
            workingDir previewDepDir
            run "dotnet fsi build.fsx -p Init"
            run "dotnet build src/Fantomas.Core"
        }
    }
    runIfOnlySpecified true
}

let publishLambda name =
    $"dotnet publish --tl -c Release -o {artifactDir </> name} {serverDir}/{name}/{name}.fsproj"

let runLambda name =
    $"dotnet watch run --project {serverDir </> name </> name}.fsproj --tl"

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
    stage "bun install" {
        workingDir clientDir
        run "bun i"
    }
    stage "dotnet install" {
        run "dotnet tool restore"
        run "dotnet restore --tl"
    }
    stage "check format F#" { run "dotnet fantomas src infrastructure build.fsx --check" }
    stage "check format JS" {
        workingDir clientDir
        run "bun run lint"
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
        run (fun _ ->
            async {
                setViteToProduction ()
                return 0
            })
        run "bun run build"
        run (fun _ ->
            async {
                File.Create(clientDir </> "build" </> ".nojekyll").Close()
                Shell.cp_r (clientDir </> "build") (artifactDir </> "client")
                return 0
            })
    }
    runIfOnlySpecified false
}

let changedFiles (ctx: StageContext) : Async<string array> =
    async {
        let! result = ctx.RunCommandCaptureOutput "git status --porcelain"
        match result with
        | Error _ -> return failwithf "Could not run git status"
        | Ok stdout ->
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
    stage "Format code" {
        run (fun ctx ->
            async {
                let! files = changedFiles ctx
                let fantomasArgument = files |> Array.filter isFSharpFile |> String.concat " "
                printfn "%s" fantomasArgument

                let! fsharpResult =
                    if String.IsNullOrWhiteSpace fantomasArgument then
                        alwaysOk
                    else
                        ctx.RunCommand $"dotnet fantomas %s{fantomasArgument}"

                let! files = changedFiles ctx
                let prettierArgument =
                    files
                    |> Array.choose (fun path ->
                        if isJSFile path then
                            Some(path.Replace("src/client/", ""))
                        else
                            None)
                    |> String.concat " "

                let! prettierResult =
                    if String.IsNullOrWhiteSpace prettierArgument then
                        alwaysOk
                    else
                        ctx.RunCommand(
                            $"bun x prettier --write {prettierArgument}",
                            workingDir = (__SOURCE_DIRECTORY__ </> "src" </> "client")
                        )

                return (mapResultToCode fsharpResult + mapResultToCode prettierResult)
            })
    }
    runIfOnlySpecified true
}

pipeline "Watch" {
    stage "bun install" {
        workingDir clientDir
        run "bun i"
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
            run "bunx --bun vite"
        }
    }
    runIfOnlySpecified true
}

tryPrintPipelineCommandHelp ()
