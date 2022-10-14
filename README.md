# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Node.js 14.x](https://nodejs.org/en/download/) or higher
* [.NET 6.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Running locally

* Pull in the source dependencies:

> dotnet fsi build.fsx -p Fantomas-Git

* Run the Watch pipeline:

> dotnet fsi build.fsx -p Watch

* Open http://localhost:9060

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Run 
> dotnet fsi build.fsx -p Fantomas-Git

> dotnet fsi build.fsx -p Watch

* Open browser for port `9060`