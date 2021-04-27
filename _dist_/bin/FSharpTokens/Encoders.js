import { object } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map, toArray } from "../.fable/fable-library.3.1.15/List.js";

export function encodeGetTokensRequest(value) {
    return object([["defines", toArray(map((value_1) => value_1, value.Defines))], ["sourceCode", value.SourceCode]]);
}

export function encodeUrlModel(code, model) {
    return object([["defines", model.Defines], ["code", code]]);
}

