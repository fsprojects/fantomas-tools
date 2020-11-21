import { object } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";

export function encodeGetTokensRequest(value) {
    let values, list_1;
    return object([["defines", (values = (list_1 = (map((value_1) => value_1, value.Defines)), (Array.from(list_1))), (values))], ["sourceCode", value.SourceCode]]);
}

export function encodeUrlModel(code, model) {
    return object([["defines", model.Defines], ["code", code]]);
}

