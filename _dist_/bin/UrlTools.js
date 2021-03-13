import { updateUrlBy as updateUrlBy_1, decompressFromEncodedURIComponent, compressToEncodedURIComponent, setGetParam as setGetParam_1 } from "../js/urlUtils.js";
import { printf, toConsole, isNullOrWhiteSpace } from "./.fable/fable-library.3.1.7/String.js";
import { length } from "./.fable/fable-library.3.1.7/Seq.js";
import { map, choose, tryHead } from "./.fable/fable-library.3.1.7/Array.js";
import { fromString } from "./.fable/Thoth.Json.5.0.0/Decode.fs.js";

const setGetParam = setGetParam_1;

const encodeUrl = compressToEncodedURIComponent;

const decodeUrl = decompressFromEncodedURIComponent;

export const updateUrlBy = updateUrlBy_1;

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
                const k = tupledArg[0];
                const v = tupledArg[1];
                if (k === "data") {
                    return v;
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
    const activePatternResult12192 = $007CKeyValuesFromHash$007C_$007C(matchValue);
    if (activePatternResult12192 != null) {
        const v = activePatternResult12192;
        const json = decodeUrl(decodeURIComponent(v));
        const modelResult = fromString(decoder, json);
        if (modelResult.tag === 1) {
            const err = modelResult.fields[0];
            toConsole(printf("%A"))(err);
            return defaultValue;
        }
        else {
            const m = modelResult.fields[0];
            return m;
        }
    }
    else {
        return defaultValue;
    }
}

