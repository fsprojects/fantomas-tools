/** @type {import("snowpack").SnowpackUserConfig } */
const port = parseInt(process.env.SNOWPACK_PUBLIC_FRONTEND_PORT || 9060);

module.exports = {
    mount: {
        public: "/",
        src: "/_dist_"
    },
    plugins: [
        "@snowpack/plugin-react-refresh",
        "@snowpack/plugin-dotenv",
        "@snowpack/plugin-sass",
        ["@snowpack/plugin-run-script", {
            "cmd": "echo 'Fable already built'",
            "watch": "dotnet fable watch ./fsharp/FantomasTools.fsproj --outDir ./src/bin"
        }]
    ],
    packageOptions: {
        knownEntrypoints: [
            "reactstrap"
        ]
    },
    devOptions: {
        port,
        output: "stream"
    },
    buildOptions: {
        /* ... */
        baseUrl: "/fantomas-tools",
        clean: true
    },
    alias: {
        /* ... */
    }
};

/*
        ["@snowpack/plugin-run-script", {
            "cmd": "dotnet fable ./src/FantomasTools/FantomasTools.fsproj --outDir ./src/bin",
            "watch": "dotnet fable watch ./src/FantomasTools/FantomasTools.fsproj --outDir ./src/bin"
        }]
 */