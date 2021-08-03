# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Node.js 12.x](https://nodejs.org/en/download/) or higher
* [Azure Functions Core Tools v3](https://www.npmjs.com/package/azure-functions-core-tools)

## Running locally

* Perform a one-time set up of source dependencies:

> dotnet fake run build.fsx -t "Fantomas-Git"

* Run the Watch target with FAKE:

> dotnet fake run build.fsx -t Watch

NOTE: you may see some error output during this process on first run, but those errors should eventually resolve and subsequent runs work without error.

* Open http://localhost:9060

NOTE: you will need to disable CORS for the front-end to interact with the locally-hosted backend. For example, if using Firefox, you can do this using the CORS Everywhere extension.

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Run `dotnet fake run build.fsx -t Gitpod`

* Open browser for port `9060`