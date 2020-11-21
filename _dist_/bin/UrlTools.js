import { updateUrlBy as updateUrlBy_1, decompressFromEncodedURIComponent, compressToEncodedURIComponent, setGetParam as setGetParam_1 } from "../js/urlUtils.js";
import { printf, toConsole, isNullOrWhiteSpace } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { length } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { tryHead, choose, map } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { fromString } from "./bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";

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
            let array_2;
            let array_1;
            const array = search[1].split("\u0026");
            array_1 = map((kv) => [kv.split("=")[0], kv.split("=")[1]], array);
            array_2 = choose((tupledArg) => {
                if (tupledArg[0] === "data") {
                    return tupledArg[1];
                }
                else {
                    return void 0;
                }
            }, array_1);
            return tryHead(array_2);
        }
        else {
            return void 0;
        }
    }
}

export function restoreModelFromUrl(decoder, defaultValue) {
    const matchValue = window.location.hash;
    const activePatternResult11362 = $007CKeyValuesFromHash$007C_$007C(matchValue);
    if (activePatternResult11362 != null) {
        const v = activePatternResult11362;
        const clo1 = toConsole(printf("v: %s"));
        clo1(v);
        let json;
        const _x = decodeURIComponent(v);
        json = decodeUrl(_x);
        const clo1_1 = toConsole(printf("json: %s"));
        clo1_1(json);
        const modelResult = fromString(decoder, json);
        if (modelResult.tag === 1) {
            const clo1_2 = toConsole(printf("%A"));
            clo1_2(modelResult.fields[0]);
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

