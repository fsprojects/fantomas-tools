import * as react from "../../web_modules/react.js";

export function versionBar(version) {
    return react.createElement("div", {
        className: "version-bar",
    }, version);
}

