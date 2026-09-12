#!/usr/bin/env -S dotnet fsi

#r "nuget: Fun.Build, 1.2.0"
#r "nuget: Fake.IO.FileSystem, 6.1.4"
// Must stay on the version Fun.Build depends on: a newer Spectre.Console moves types Fun.Build
// looks up and every pipeline dies with a TypeLoadException before it starts.
#r "nuget: Spectre.Console, 0.46.0"

open System
open System.IO
open Fun.Build
open Fun.Build.Internal
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Spectre.Console

let astPort = 7412
let oakPort = 8904
let fantomasMainPort = 11084
let fantomasPreviewPort = 12007
let fantomasV5Port = 11009
let fantomasV6Port = 13042
let fantomasV7Port = 10707
/// Mirrors `server.port` and `preview.port` in src/client/vite.config.js.
let frontendPort = 9060
let pwd = __SOURCE_DIRECTORY__

/// The branch the preview backend is built from, when there is one.
///
/// Preview exists so the next major version can be worked on in its own branch while the stable
/// release keeps shipping. There is no such branch right now: everything is on main, and
/// `FantomasPreviewRepository` in Directory.Build.props points at the same checkout as main. A
/// second clone would then be a second copy of what main already built, and nothing would read it.
///
/// When the next major gets its own branch, name it here and point `FantomasPreviewRepository` at
/// `.deps/<branch>`. The two go together: this decides what is cloned and built, that decides what
/// the preview backend compiles against.
let previewBranch: string option = None

/// Every Fantomas checkout the tools build against, as the branch to clone and the folder under
/// `.deps` to keep it in.
let fantomasCheckouts: (string * string) list =
    [
        yield "main", "fantomas"

        match previewBranch with
        | Some branch -> yield branch, branch
        | None -> ()
    ]

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
        run (fun ctx ->
            async {
                let! results =
                    fantomasCheckouts
                    |> List.map (fun (branch, folder) ->
                        let checkoutDir = pwd </> ".deps" </> folder

                        async {
                            if Directory.Exists checkoutDir then
                                return! git ctx checkoutDir "pull"
                            else
                                return!
                                    git
                                        ctx
                                        __SOURCE_DIRECTORY__
                                        $"clone -b {branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/{folder}"
                        })
                    |> Async.Parallel

                return results |> Array.map mapResultToCode |> Array.fold max 0
            })
    }
    stage "build" {
        run (fun ctx ->
            async {
                let! results =
                    fantomasCheckouts
                    |> List.map (fun (_, folder) ->
                        let checkoutDir = pwd </> ".deps" </> folder

                        async {
                            match! ctx.RunCommand("dotnet fsi build.fsx -p Init", workingDir = checkoutDir) with
                            | Error error -> return Error error
                            | Ok() ->
                                return! ctx.RunCommand("dotnet build src/Fantomas.Core", workingDir = checkoutDir)
                        })
                    |> Async.Parallel

                return results |> Array.map mapResultToCode |> Array.fold max 0
            })
    }
    runIfOnlySpecified true
}

let publishLambda name =
    $"dotnet publish --tl -c Release {serverDir}/{name}/{name}.fsproj"

// Hot Reload has no F# support, so `dotnet watch` announces that every project does not support it
// and rebuilds anyway, once per change per project. Ask for the rebuild it was going to do and the
// output stays about the code.
let runLambda name =
    $"dotnet watch --no-hot-reload run --project {serverDir </> name </> name}.fsproj --tl"

let setViteToProduction () =
    setEnv "NODE_ENV" "production"

    let mainStageUrl =
        "https://arlp8cgo97.execute-api.eu-west-1.amazonaws.com/fantomas-main-stage-1c52a6a"

    setEnv "VITE_AST_BACKEND" $"{mainStageUrl}/ast-viewer"
    setEnv "VITE_OAK_BACKEND" $"{mainStageUrl}/oak-viewer"
    setEnv "VITE_FANTOMAS_V5" $"{mainStageUrl}/fantomas/v5"
    setEnv "VITE_FANTOMAS_V6" $"{mainStageUrl}/fantomas/v6"
    setEnv "VITE_FANTOMAS_V7" $"{mainStageUrl}/fantomas/v7"
    setEnv "VITE_FANTOMAS_MAIN" $"{mainStageUrl}/fantomas/main"
    setEnv "VITE_FANTOMAS_PREVIEW" $"{mainStageUrl}/fantomas/preview"

let bunInstall =
    stage "bun install" {
        workingDir clientDir
        run "bun i"
    }

let dotnetInstall =
    stage "dotnet install" {
        run "dotnet tool restore"
        run "dotnet restore --tl"
    }

pipeline "Build" {
    workingDir __SOURCE_DIRECTORY__
    bunInstall
    dotnetInstall
    stage "check format F#" { run "dotnet fantomas check src infrastructure build.fsx" }
    stage "check format JS" {
        workingDir clientDir
        run "bun run lint"
    }
    stage "clean" {
        run (fun _ ->
            async {
                Shell.rm_rf artifactDir
                Shell.rm_rf (clientDir + "/build")
                return 0
            })
    }
    stage "publish lambdas" {
        stage "parallel ones" {
            paralle
            run (publishLambda "FantomasOnlineV5")
            run (publishLambda "FantomasOnlineV6")
            run (publishLambda "FantomasOnlineV7")
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

/// Where a locally running service can be reached. Gitpod exposes ports on a generated host
/// instead of on localhost.
let localUrl (port: int) (subPath: string) : string =
    let gitpodEnv = Environment.GetEnvironmentVariable("GITPOD_WORKSPACE_URL")

    if String.IsNullOrWhiteSpace(gitpodEnv) then
        sprintf "http://localhost:%i/%s" port subPath
    else
        let gitpodEnv = gitpodEnv.Replace("https://", "")
        sprintf "https://%i-%s/%s" port gitpodEnv subPath

/// One backend the dev pipelines launch: the project to run, the port it listens on, the route
/// prefix it serves and the Vite variable the frontend reads its url from.
type Backend =
    {
        Project: string
        Port: int
        SubPath: string
        EnvironmentVariable: string
    }

    member this.Url = localUrl this.Port this.SubPath

let backends: Backend list =
    [
        {
            Project = "ASTViewer"
            Port = astPort
            SubPath = "ast-viewer"
            EnvironmentVariable = "VITE_AST_BACKEND"
        }
        {
            Project = "OakViewer"
            Port = oakPort
            SubPath = "oak-viewer"
            EnvironmentVariable = "VITE_OAK_BACKEND"
        }
        {
            Project = "FantomasOnlineV5"
            Port = fantomasV5Port
            SubPath = "fantomas/v5"
            EnvironmentVariable = "VITE_FANTOMAS_V5"
        }
        {
            Project = "FantomasOnlineV6"
            Port = fantomasV6Port
            SubPath = "fantomas/v6"
            EnvironmentVariable = "VITE_FANTOMAS_V6"
        }
        {
            Project = "FantomasOnlineV7"
            Port = fantomasV7Port
            SubPath = "fantomas/v7"
            EnvironmentVariable = "VITE_FANTOMAS_V7"
        }
        {
            Project = "FantomasOnlineMain"
            Port = fantomasMainPort
            SubPath = "fantomas/main"
            EnvironmentVariable = "VITE_FANTOMAS_MAIN"
        }
        {
            Project = "FantomasOnlinePreview"
            Port = fantomasPreviewPort
            SubPath = "fantomas/preview"
            EnvironmentVariable = "VITE_FANTOMAS_PREVIEW"
        }
    ]

let prepareEnvironmentVariables =
    stage "prepare environment variables" {
        run (fun _ ->
            async {
                setEnv "NODE_ENV" "development"

                for backend in backends do
                    setEnv backend.EnvironmentVariable backend.Url

                return 0
            })
    }

/// The services print nothing on startup, so this is the only place that says what is listening
/// where. It goes up before the parallel stage starts, while the console is still quiet.
let printOverview (title: string) =
    stage "overview" {
        run (fun _ ->
            async {
                let table = Table()
                table.Title <- TableTitle(title)
                table.Border <- TableBorder.Rounded
                table.AddColumn("Service") |> ignore
                table.AddColumn(TableColumn("Port").RightAligned()) |> ignore
                table.AddColumn("Url") |> ignore

                for backend in backends do
                    table.AddRow(backend.Project, string backend.Port, backend.Url) |> ignore

                table.AddRow("Frontend", string frontendPort, localUrl frontendPort "fantomas-tools/")
                |> ignore

                AnsiConsole.Write(table)
                return 0
            })
    }

pipeline "Watch" {
    bunInstall
    dotnetInstall
    prepareEnvironmentVariables
    printOverview "Fantomas Tools (watch)"
    stage "launch services" {
        paralle
        run (runLambda "ASTViewer")
        run (runLambda "OakViewer")
        run (runLambda "FantomasOnlineV5")
        run (runLambda "FantomasOnlineV6")
        run (runLambda "FantomasOnlineV7")
        run (runLambda "FantomasOnlineMain")
        run (runLambda "FantomasOnlinePreview")
        stage "frontend" {
            workingDir clientDir
            run "bunx --bun vite"
        }
    }
    runIfOnlySpecified true
}

let runPublishedLambda name =
    let binary =
        __SOURCE_DIRECTORY__
        </> "artifacts"
        </> "publish"
        </> name
        </> "debug"
        </> $"%s{name}.dll"

    stage $"Run %s{name}" {
        run $"dotnet publish --nologo -c Debug -tl {serverDir </> name </> name}.fsproj"
        run $"dotnet %s{binary}"
    }

pipeline "Start" {
    bunInstall
    dotnetInstall
    prepareEnvironmentVariables
    printOverview "Fantomas Tools (start)"
    stage "launch services" {
        paralle
        runPublishedLambda "ASTViewer"
        runPublishedLambda "OakViewer"
        runPublishedLambda "FantomasOnlineV5"
        runPublishedLambda "FantomasOnlineV6"
        runPublishedLambda "FantomasOnlineV7"
        runPublishedLambda "FantomasOnlineMain"
        runPublishedLambda "FantomasOnlinePreview"
        stage "frontend" {
            workingDir clientDir
            run "bun run build"
            run "bun run serve"
        }
    }
    runIfOnlySpecified true
}

tryPrintPipelineCommandHelp ()
