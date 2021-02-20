import { updateUrlBy as updateUrlBy_1, decompressFromEncodedURIComponent, compressToEncodedURIComponent, setGetParam as setGetParam_1 } from "../js/urlUtils.js";
import { printf, toConsole, isNullOrWhiteSpace } from "./.fable/fable-library.3.1.1/String.js";
import { length } from "./.fable/fable-library.3.1.1/Seq.js";
import { map, choose, tryHead } from "./.fable/fable-library.3.1.1/Array.js";
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
    const activePatternResult12202 = $007CKeyValuesFromHash$007C_$007C(matchValue);
    if (activePatternResult12202 != null) {
        const v = activePatternResult12202;
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

