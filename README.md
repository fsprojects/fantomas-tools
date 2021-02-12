# Fantomas tools

Collection of tools used when developing for Fantomas

## Prerequisites

To run this tool locally you need:

* [Node.js 12.x](https://nodejs.org/en/download/) or higher
* [Azure Functions Core Tools v3](https://www.npmjs.com/package/azure-functions-core-tools)

## Running locally

* Restore the dotnet tools:

> dotnet tool restore

* Restore the npm packages:

> yarn

* Run the Watch target with FAKE:

> dotnet fake run build.fsx -t Watch

Open http://localhost:9060

## Running in Gitpod

* Open the repository via https://gitpod.io/#https://github.com/fsprojects/fantomas-tools

* Each gitpod workspace has a unique url, this should be added as an environment variables:<br />
    Example workspace url `https://azure-dragonfly-j2ukxl6p.ws-eu03.gitpod.io/` => `export GITPOD=azure-dragonfly-j2ukxl6p.ws-eu03`

* Run `dotnet fake run build.fsx -t Gitpod`

* Open browser for port `9060`