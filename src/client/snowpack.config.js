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
        "@snowpack/plugin-sass"
    ],
    install: [
        "reactstrap"
    ],
    installOptions: {
        /* ... */
    },
    devOptions: {
        port
    },
    buildOptions: {
        /* ... */
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