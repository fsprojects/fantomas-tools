#!/usr/bin/env -S dotnet fsi --

#r "nuget: Fun.Build, 1.2.0"
#r "nuget: Fake.IO.FileSystem, 6.1.4"
// Must stay on the version Fun.Build depends on: a newer Spectre.Console moves types Fun.Build
// looks up and every pipeline dies with a TypeLoadException before it starts.
#r "nuget: Spectre.Console, 0.46.0"
#r "nuget: Humanizer.Core, 3.0.10"

open System
open System.IO
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Json.Nodes
open System.Xml.Linq
open System.Xml.XPath
open Fun.Build
open Fun.Build.Internal
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Spectre.Console
open Humanizer

let astPort = 7412
let oakPort = 8904
let fantomasMainPort = 11084
let fantomasPreviewPort = 12007
let fantomasV6Port = 13042
let fantomasV7Port = 10707
let fantomasV8Port = 10808
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
                                        $"clone -b %s{branch} --single-branch https://github.com/fsprojects/fantomas.git .deps/%s{folder}"
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
    $"dotnet publish --tl -c Release %s{serverDir}/%s{name}/%s{name}.fsproj"

// The backends are only watched when asked for with `--server`: most work happens in the client,
// and seven projects rebuilding on every change is a lot of noise for code that did not change.
//
// Hot Reload has no F# support, so `dotnet watch` announces that every project does not support it
// and rebuilds anyway, once per change per project. Ask for the rebuild it was going to do and the
// output stays about the code.
let runLambda name (ctx: StageContext) =
    let project = serverDir </> name </> $"%s{name}.fsproj"

    if List.contains "--server" (ctx.GetAllCmdArgs()) then
        $"dotnet watch --no-hot-reload run --project %s{project} --tl"
    else
        $"dotnet run --project %s{project} --tl"

let setViteToProduction () =
    setEnv "NODE_ENV" "production"

    let mainStageUrl =
        "https://arlp8cgo97.execute-api.eu-west-1.amazonaws.com/fantomas-main-stage-1c52a6a"

    setEnv "VITE_AST_BACKEND" $"%s{mainStageUrl}/ast-viewer"
    setEnv "VITE_OAK_BACKEND" $"%s{mainStageUrl}/oak-viewer"
    setEnv "VITE_FANTOMAS_V6" $"%s{mainStageUrl}/fantomas/v6"
    setEnv "VITE_FANTOMAS_V7" $"%s{mainStageUrl}/fantomas/v7"
    setEnv "VITE_FANTOMAS_V8" $"%s{mainStageUrl}/fantomas/v8"
    setEnv "VITE_FANTOMAS_MAIN" $"%s{mainStageUrl}/fantomas/main"
    setEnv "VITE_FANTOMAS_PREVIEW" $"%s{mainStageUrl}/fantomas/preview"

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
    // The links people share carry the whole model through lz-string and the decoders, so a change
    // to either breaks every link ever posted. These tests run over links taken from Fantomas
    // issues, and they are what says a new version of those pieces is safe to take.
    stage "test client" {
        workingDir clientDir
        run "bun run test"
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
            run (publishLambda "FantomasOnlineV6")
            run (publishLambda "FantomasOnlineV7")
            run (publishLambda "FantomasOnlineV8")
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
        | Error _ -> return failwith "Could not run git status"
        | Ok stdout ->
            return
                stdout.Split('\n')
                |> Array.choose (fun line ->
                    let line = line.Trim()
                    if
                        line.StartsWith("AM", StringComparison.Ordinal)
                        || line.StartsWith("M", StringComparison.Ordinal)
                    then
                        Some(line.Replace("AM ", "").Replace("M ", ""))
                    else
                        None)
    }

let fsharpExtensions = set [| ".fs"; ".fsi"; ".fsx" |]
/// What oxfmt formats in this repository. The client stylesheet is in there too, so a changed
/// stylesheet is formatted by the same pass as a changed component.
let clientExtensions = set [| ".js"; ".jsx"; ".css" |]
let isFSharpFile path =
    FileInfo(path).Extension |> fsharpExtensions.Contains
let isClientFile path =
    FileInfo(path).Extension |> clientExtensions.Contains

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
                let oxfmtArgument =
                    files
                    |> Array.choose (fun path ->
                        if isClientFile path then
                            Some(path.Replace("src/client/", ""))
                        else
                            None)
                    |> String.concat " "

                let! oxfmtResult =
                    if String.IsNullOrWhiteSpace oxfmtArgument then
                        alwaysOk
                    else
                        ctx.RunCommand(
                            $"bunx oxfmt %s{oxfmtArgument}",
                            workingDir = (__SOURCE_DIRECTORY__ </> "src" </> "client")
                        )

                return (mapResultToCode fsharpResult + mapResultToCode oxfmtResult)
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
            Project = "FantomasOnlineV8"
            Port = fantomasV8Port
            SubPath = "fantomas/v8"
            EnvironmentVariable = "VITE_FANTOMAS_V8"
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

// The analyzers, over every project of the solution and over this script.
//
// This mirrors what the Fantomas repository does: the same two analyzer packages, referenced by
// every project so that MSBuild is the only place their versions live, and one process per target
// so that findings arrive while the rest of the run is still going.

/// The projects the analyzers run over. The Fantomas checkouts under `.deps` are in the solution so
/// an editor can navigate into them, but their source is not ours: a finding there is something to
/// report upstream rather than something to fix here.
let projectsToAnalyze: string list =
    XDocument.Load(pwd </> "fantomas-tools.slnx").XPathSelectElements("//Project")
    |> Seq.choose (fun (project: XElement) ->
        let path: string = project.Attribute(XName.Get "Path").Value.Replace('\\', '/')

        if path.StartsWith(".deps/", StringComparison.Ordinal) then
            None
        else
            Some path)
    |> Seq.toList

/// Where the analyzer packages are restored to. They are ordinary package references, so MSBuild
/// already knows the path of each and there is no second place to keep a version in sync. Any
/// project answers this, they all inherit the references from Directory.Build.props.
let analyzerPaths (ctx: StageContext) : Async<string list> =
    async {
        let! result =
            ctx.RunCommandCaptureAll(
                "dotnet msbuild src/server/ASTViewer/ASTViewer.fsproj "
                + "-getProperty:PkgIonide_Analyzers "
                + "-getProperty:PkgG-Research_FSharp_Analyzers",
                workingDir = pwd,
                disablePrintCommand = true,
                disablePrintOutput = true
            )

        if result.ExitCode <> 0 then
            failwith $"Could not resolve the analyzer packages. Run `dotnet restore` first.\n%s{result.StandardError}"

        use document = JsonDocument.Parse(result.StandardOutput)

        return
            [
                for property in document.RootElement.GetProperty("Properties").EnumerateObject() do
                    let path = property.Value.GetString()

                    if String.IsNullOrEmpty path then
                        failwith $"MSBuild has no value for %s{property.Name}. Run `dotnet restore` first."

                    path </> "analyzers" </> "dotnet" </> "fs"
            ]
    }

/// Where every target writes its own report, before they are merged into one.
let analysisReportsDir: string = pwd </> "analysisreports"

/// The merged analyzer report, holding the last run and nothing more. This is the file CI uploads
/// to code scanning, and the reason the per-target reports are merged at all.
let mergedAnalysisReport: string = pwd </> "analysis.sarif"

/// A child of a JSON node, or null when either the node or the child is missing. SARIF leaves most
/// of its properties optional, and a report without a single finding writes neither `results` nor
/// `rules`.
let private child (name: string) (node: JsonNode) : JsonNode =
    if isNull node then null else node[name]

let private clone (node: JsonNode) : JsonNode =
    if isNull node then null else node.DeepClone()

/// The prefix the tool puts in front of every path it reports.
///
/// It writes a location relative to the folder that holds the code root rather than to the code
/// root itself, so every uri starts with the name of this repository's own folder. Code scanning
/// resolves a relative uri against the repository root and would find nothing under that, so the
/// segment comes off on the way into the merged report.
let private reportedPathPrefix: string = $"%s{Path.GetFileName pwd}/"

let private repositoryRelative (uri: string) : string =
    if uri.StartsWith(reportedPathPrefix, StringComparison.Ordinal) then
        uri.Substring reportedPathPrefix.Length
    else
        uri

/// Rewrites the paths of one result in place, so the merged report points at files as the
/// repository holds them.
let private relativizeResult (result: JsonNode) : unit =
    match child "locations" result with
    | null -> ()
    | locations ->
        for location in locations.AsArray() do
            match location |> child "physicalLocation" |> child "artifactLocation" with
            | null -> ()
            | artifact ->
                match child "uri" artifact with
                | null -> ()
                | uri -> artifact["uri"] <- JsonValue.Create(repositoryRelative (uri.GetValue<string>()))

/// Folds the per-target reports into the one SARIF run that GitHub code scanning takes.
///
/// SARIF carries a run per tool invocation, but code scanning rejects a file holding several unless
/// each names its own category, and one project of this repository is not an analysis of its own.
/// The runs all come from the same tool, so their results concatenate into a single run. Every
/// invocation is kept, which is what records that a target was looked at even when it turned up
/// nothing.
let mergeSarifReports (reports: string list) (target: string) : unit =
    let documents =
        reports
        |> List.choose (fun (report: string) ->
            if File.Exists report then
                Some(JsonNode.Parse(File.ReadAllText report))
            else
                None)

    let runs =
        documents
        |> List.collect (fun (document: JsonNode) -> document["runs"].AsArray() |> List.ofSeq)

    match documents, runs with
    | firstDocument :: _, firstRun :: _ ->
        // `ruleIndex` addresses `tool.driver.rules` by position within its own run, so merging the
        // runs means pointing every result at where its own rule ended up.
        //
        // Identical entries collapse. The analyzers write one entry per finding rather than one per
        // rule, its `name` being that finding's message, so the same entry is written again for
        // every finding that reads the same: the same rule firing twice in one project, or in two.
        // GitHub refuses a document whose rules array holds a duplicate.
        let rules = ResizeArray<JsonNode>()
        let seen = Collections.Generic.Dictionary<string, int>()

        let indexOf (rule: JsonNode) : int =
            let key = rule.ToJsonString()

            match seen.TryGetValue key with
            | true, index -> index
            | false, _ ->
                let index = rules.Count
                rules.Add(clone rule)
                seen[key] <- index
                index

        let results = JsonArray()
        let invocations = JsonArray()

        for run in runs do
            // Every rule of the run is placed, whether a result points at it or not, so that the
            // merged report says the same about what the tool knows as the parts did.
            let placed: int array =
                match run |> child "tool" |> child "driver" |> child "rules" with
                | null -> [||]
                | rules -> rules.AsArray() |> Seq.map indexOf |> Array.ofSeq

            match child "results" run with
            | null -> ()
            | runResults ->
                for result in runResults.AsArray() do
                    let result = clone result

                    match child "ruleIndex" result with
                    | null -> ()
                    | index ->
                        let original = index.GetValue<int>()

                        if original >= 0 && original < placed.Length then
                            result["ruleIndex"] <- JsonValue.Create(placed[original])

                    relativizeResult result
                    results.Add result

            match child "invocations" run with
            | null -> ()
            | runInvocations ->
                for invocation in runInvocations.AsArray() do
                    invocations.Add(clone invocation)

        let tool = clone firstRun["tool"]
        tool["driver"]["rules"] <- JsonArray(rules.ToArray())

        let mergedRun = JsonObject()
        mergedRun["tool"] <- tool
        mergedRun["columnKind"] <- clone (child "columnKind" firstRun)
        mergedRun["results"] <- results
        mergedRun["invocations"] <- invocations

        let merged = JsonObject()
        merged["$schema"] <- clone (child "$schema" firstDocument)
        merged["version"] <- clone (child "version" firstDocument)
        merged["runs"] <- JsonArray(mergedRun)

        // Indented, because this file is read by people as often as by code scanning, and one line
        // of fifty thousand characters is not something you can read at all. The relaxed encoder is
        // for the same reason: the default escapes `<` and `>`, and every message about a
        // `[<Struct>]` attribute then arrives full of `\u003C`.
        let options =
            JsonSerializerOptions(WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping)
        File.WriteAllText(target, merged.ToJsonString(options) + "\n")
    | _ -> failwith "The analyzers wrote no report to merge."

/// What one analyzer process covers.
type AnalysisTarget =
    | Project of project: string
    /// The scripts that compile on their own. `build.fsx` is the only one, and it is as much source
    /// as anything under `src`.
    | Scripts of scripts: string list

let private targetName (target: AnalysisTarget) : string =
    match target with
    | Scripts _ -> "Scripts"
    | Project project -> Path.GetFileNameWithoutExtension project

/// One line of the tool's output that reports a finding, whatever its severity. Counting these is
/// what lets a target say how it went the moment it finishes; the tool itself only reports that
/// through the report file, and reads a clean run and a failed one the same way otherwise.
let private findingLine: Text.RegularExpressions.Regex =
    Text.RegularExpressions.Regex(@"\(\d+,\d+\): (Hint|Info|Warning|Error) [A-Z][A-Z0-9-]* :")

let private countFindings (output: string) : int =
    output.Split('\n')
    |> Array.filter (fun (line: string) -> findingLine.IsMatch line)
    |> Array.length

/// Runs the analyzers over the given targets, one process each, a few at a time.
///
/// Analyzing a project costs what type checking it costs, and the server projects type check
/// Fantomas.Core on the way, so a single process walking all of them says nothing for a minute.
/// Each target is instead analyzed on its own and its output held back until it finishes, so
/// findings arrive as they are found and no two targets interleave their lines.
///
/// Returns the highest exit code, so a target the analyzers could not process fails the stage
/// rather than passing for want of findings. Findings themselves do not fail it: every rule of
/// these two packages reports below error severity, and the run is there to be read.
let analyzeTargets (ctx: StageContext) (targets: AnalysisTarget list) : Async<int> =
    async {
        let! analyzers = analyzerPaths ctx

        // Whatever is analyzed here is what the report holds afterwards, so a run over a couple of
        // targets replaces the report of an earlier run over the solution.
        if Directory.Exists analysisReportsDir then
            Directory.Delete(analysisReportsDir, true)

        Directory.CreateDirectory analysisReportsDir |> ignore

        printfn
            $"""Analyzing %s{"target".ToQuantity targets.Length}: %s{targets |> List.map targetName |> String.concat ", "}"""

        let analyzeTarget (target: AnalysisTarget) : Async<string * int> =
            async {
                let name = targetName target
                let report = analysisReportsDir </> $"%s{name}.sarif"
                let started = DateTime.UtcNow

                let arguments: string list =
                    [
                        for analyzer in analyzers do
                            "--analyzers-path"
                            analyzer

                        "--code-root"
                        pwd

                        "--report"
                        report

                        match target with
                        | Scripts scripts ->
                            "--script"
                            yield! scripts
                        | Project project ->
                            // MSBuild generates an AssemblyInfo per project and it is part of what
                            // gets type checked. Nobody wrote it, so a finding in it is not a
                            // finding about this repository.
                            "--exclude-files"
                            "**/*.AssemblyInfo.fs"

                            "--project"
                            pwd </> project
                    ]

                // Fun.Build takes a command as a single string and splits it on whitespace, so a
                // path with a space in it has to say that it is one argument.
                let command =
                    arguments |> List.map (fun argument -> $"\"%s{argument}\"") |> String.concat " "

                let! result =
                    ctx.RunCommandCaptureAll(
                        $"dotnet fsharp-analyzers %s{command}",
                        workingDir = pwd,
                        disablePrintCommand = true,
                        disablePrintOutput = true
                    )

                let elapsed = DateTime.UtcNow - started

                let findings =
                    match countFindings result.StandardOutput with
                    | 0 -> "no findings"
                    | count -> "finding".ToQuantity count

                // A non-zero exit is worth saying out loud: the tool exits non-zero both for a
                // finding at error severity and for a run that never happened, and "no findings"
                // would read the same either way.
                let summary =
                    if result.ExitCode = 0 then
                        findings
                    else
                        $"%s{findings}, exit code %i{result.ExitCode}"

                printfn $"\n=== %s{name}: %s{summary} in %.1f{elapsed.TotalSeconds}s"
                printf $"%s{result.StandardOutput}"
                eprintf $"%s{result.StandardError}"

                return report, result.ExitCode
            }

        // Every process type checks a whole project, so a handful at a time keeps the machine busy
        // without the runs starving each other of memory.
        let! results =
            Async.Parallel(List.map analyzeTarget targets, max 2 (Environment.ProcessorCount / 2))

        mergeSarifReports (results |> Array.map fst |> List.ofArray) mergedAnalysisReport
        printfn $"\nWrote %s{Path.GetFileName mergedAnalysisReport}"

        return results |> Array.map snd |> Array.fold max 0
    }

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
                    table.AddRow(backend.Project, string<int> backend.Port, backend.Url) |> ignore

                table.AddRow("Frontend", string<int> frontendPort, localUrl frontendPort "fantomas-tools/")
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
        run (runLambda "FantomasOnlineV6")
        run (runLambda "FantomasOnlineV7")
        run (runLambda "FantomasOnlineV8")
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
        run $"dotnet publish --nologo -c Debug -tl %s{serverDir </> name </> name}.fsproj"
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
        runPublishedLambda "FantomasOnlineV6"
        runPublishedLambda "FantomasOnlineV7"
        runPublishedLambda "FantomasOnlineV8"
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

pipeline "Analyze" {
    workingDir __SOURCE_DIRECTORY__
    dotnetInstall
    stage "Analyze" {
        // fsharp-analyzers targets the previous runtime and loads MSBuild from the SDK that
        // global.json picks. The host never rolls a release app onto a prerelease runtime while
        // a release one is installed, so an RC SDK needs this or the tool starts on the old
        // runtime and fails to load the SDK's assemblies.
        envVars [| "DOTNET_ROLL_FORWARD_TO_PRERELEASE", "1" |]

        run (fun ctx ->
            [
                for project in projectsToAnalyze do
                    Project project

                Scripts [ pwd </> "build.fsx" ]
            ]
            |> analyzeTargets ctx)
    }
    runIfOnlySpecified true
}

tryPrintPipelineCommandHelp ()
