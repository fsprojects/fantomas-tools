import * as react from "../../_snowpack/pkg/react.js";

export function versionBar(version) {
    return react.createElement("div", {
        className: "version-bar",
    }, version);
}

