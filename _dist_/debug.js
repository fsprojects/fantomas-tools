import { decompressFromEncodedURIComponent } from "./js/urlUtils.js";

window.addEventListener("load", () => {
    document.querySelector("button").addEventListener("click", () => {
        const input = document.querySelector("textarea").value;
        const decodeUri = decodeURIComponent(input);
        const decompressed = decompressFromEncodedURIComponent(decodeUri);
        console.log(`decodeUri:\n${decodeUri}\n\ndecompressed:\n${decompressed}`);
    });
});