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

Making changes should reflect in the tool.

Or try the Run pipeline:

```shell
dotnet fsi build -- -p Run
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

## Other pipelines

To see any other avaiable build script pipelines:

```shell
dotnet fsi build.fsx -- --help
```