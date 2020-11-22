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
            "cmd": "dotnet fable ./src/FantomasTools/FantomasTools.fsproj --outDir ./src/bin",
            "watch": "dotnet fable watch ./src/FantomasTools/FantomasTools.fsproj --outDir ./src/bin"
        }]
    ],
    install: [
        "reactstrap"
    ],
    installOptions: {
        /* ... */
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
    proxy: {
        /* ... */
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