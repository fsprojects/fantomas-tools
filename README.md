# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Bun](https://bun.sh/)
* [.NET 8.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Running locally

* Pull in the source dependencies:

```shell
dotnet fsi build.fsx -- -p Fantomas-Git
```

* Run the Watch pipeline:

```shell
dotnet fsi build.fsx -- -p Watch
```

Making changes to the client should reflect in the tool. The backends are started once; add `--server` to have them rebuild on changes too:

```shell
dotnet fsi build.fsx -- -p Watch --server
```

Or try the Run pipeline:

```shell
dotnet fsi build -- -p Start
```

This will run a published version of the tools.

* Open http://localhost:9060

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Run

```shell
dotnet fsi build.fsx -- -p Fantomas-Git
```

```shell
dotnet fsi build.fsx -- -p Watch
```

* Open browser for port `9060`

## Analyzers

The projects and `build.fsx` are checked with the [Ionide](https://github.com/ionide/ionide-analyzers) and
[G-Research](https://github.com/G-Research/fsharp-analyzers) analyzers, the pair the Fantomas repository uses:

```shell
dotnet fsi build.fsx -- -p Analyze
```

Every finding is printed, and none of them fails the run: the two packages report below error severity, so the
pipeline only fails when a target could not be analyzed at all. Each target also writes a SARIF report to
`analysisreports/`, and those are merged into `analysis.sarif` in the repository root. CI runs the pipeline on
every pull request and uploads that merged report to GitHub code scanning.

## Other pipelines

To see any other avaiable build script pipelines:

```shell
dotnet fsi build.fsx -- --help
```