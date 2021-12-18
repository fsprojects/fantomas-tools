# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Node.js 14.x](https://nodejs.org/en/download/) or higher
* [.NET 3.1.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1)
* [.NET 5.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)

## Running locally

* Restore the dotnet tools:

> dotnet tool restore

* Pull in source dependencies:

> dotnet fake run build.fsx -t "Fantomas-Git"

* Run the Watch target with FAKE:

> dotnet fake run build.fsx -t Watch

NOTE: you may see some error output during this process on first run, but those errors should eventually resolve and subsequent runs work without error.

* Open http://localhost:9060

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Run 
> dotnet fake run build.fsx -t "Fantomas-Git"
> dotnet fake run build.fsx -t Watch

* Open browser for port `9060`