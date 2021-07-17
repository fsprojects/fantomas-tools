import { printf, toConsole, interpolate, toText, isNullOrWhiteSpace, split } from "./.fable/fable-library.3.2.9/String.js";
import { toString } from "./.fable/fable-library.3.2.9/Types.js";
import { decompressFromEncodedURIComponent, compressToEncodedURIComponent } from "../../_snowpack/pkg/lz-string.js";
import { length } from "./.fable/fable-library.3.2.9/Seq.js";
import { map, choose, tryHead } from "./.fable/fable-library.3.2.9/Array.js";
import { fromString } from "./.fable/Thoth.Json.5.1.0/Decode.fs.js";

function setGetParam(encodedJson) {
    if (!(((arg00) => {
        history.pushState(arg00);
    }) == null)) {
        const hashPieces = split(window.location.hash, ["?"], null, 1);
        const hash = ((!(hashPieces == null)) ? (!isNullOrWhiteSpace(hashPieces[0])) : false) ? hashPieces[0] : "";
        const params = new URLSearchParams();
        params.set("data", encodedJson);
        const newUrl = toText(interpolate("%P()//%P()%P()%P()?%P()", [window.location.protocol, window.location.host, window.location.pathname, hash, toString(params)]));
        history.pushState({
            path: newUrl,
        }, "", newUrl);
    }
}

const encodeUrl = compressToEncodedURIComponent;

const decodeUrl = decompressFromEncodedURIComponent;

const URLSearchParamsExist = 'URLSearchParams' in window;

export function updateUrlBy(mapFn) {
    if (URLSearchParamsExist) {
        const hashPieces = window.location.hash.split("?");
        const params = new URLSearchParams(hashPieces[1]);
        const newUrl = toText(interpolate("%P()//%P()%P()%P()?%P()", [window.location.protocol, window.location.host, window.location.pathname, mapFn((window.location.hash == null) ? "" : window.location.hash).split("?")[0], toString(params)]));
        history.pushState({
            path: newUrl,
        }, "", newUrl);
    }
}

export function updateUrlWithData(json) {
    setGetParam(encodeUrl(json));
}

function $007CKeyValuesFromHash$007C_$007C(hash) {
    if (isNullOrWhiteSpace(hash)) {
        return void 0;
    }
    else {
        const search = hash.split("?");
        if (length(search) > 1) {
            return tryHead(choose((tupledArg) => {
                if (tupledArg[0] === "data") {
                    return tupledArg[1];
                }
                else {
                    return void 0;
                }
            }, map((kv) => [kv.split("=")[0], kv.split("=")[1]], search[1].split("\u0026"))));
        }
        else {
            return void 0;
        }
    }
}

export function restoreModelFromUrl(decoder, defaultValue) {
    const matchValue = window.location.hash;
    const activePatternResult12427 = $007CKeyValuesFromHash$007C_$007C(matchValue);
    if (activePatternResult12427 != null) {
        const v = activePatternResult12427;
        const modelResult = fromString(decoder, decodeUrl(decodeURIComponent(v)));
        if (modelResult.tag === 1) {
            toConsole(printf("%A"))(modelResult.fields[0]);
            return defaultValue;
        }
        else {
            return modelResult.fields[0];
        }
    }
    else {
        return defaultValue;
    }
}

