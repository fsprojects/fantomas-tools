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

## AST viewer syntax reader

The AST viewer reads the untyped syntax tree without F# reflection, so it can run under Native AOT.
`src/server/ASTViewer/SyntaxReader.fs` has one function per syntax type that turns a value into a `SyntaxNode`.
Do not edit that file: it is generated from `SyntaxTree.fsi` and `SyntaxTrivia.fsi` of the FCS commit the
preview Fantomas checkout (`FantomasPreviewRepository`) builds Fantomas.FCS from.

Run the generator again each time that FCS commit changes:

```shell
dotnet fsi build.fsx -- -p Fantomas-Git
dotnet fsi src/server/ASTViewer/GenerateSyntaxReader.fsx
```

The first command clones or pulls the Fantomas checkouts and builds them, which puts the FCS sources under
`.deps`. The
generator then writes and formats `SyntaxReader.fs`. If the syntax files use a type they do not define, the
generator stops and names it. Add a function with that name, starting with a lowercase letter, to the `Leaf`
module in `src/server/ASTViewer/SyntaxNode.fs`, and run the generator again.

## Other pipelines

To see any other avaiable build script pipelines:

```shell
dotnet fsi build.fsx -- --help
```