# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Node.js 14.x](https://nodejs.org/en/download/) or higher
* [.NET 6.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## Running locally

* Restore the dotnet tools:

> dotnet tool restore

* Pull in source dependencies and packages:

> dotnet fake build -t Install

* Run the Watch target with FAKE:

> dotnet fake build -t Watch

NOTE: you may see some error output during this process on first run, but those errors should eventually resolve and subsequent runs work without error.

* Open http://localhost:9060

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Run 
> dotnet fake build -t Install

> dotnet fake build -t Watch

* Open browser for port `9060`