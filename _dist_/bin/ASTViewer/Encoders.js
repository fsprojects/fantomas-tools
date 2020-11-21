import { toString, object } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";

export function encodeUrlModel(code, model) {
    return object([["defines", model.Defines], ["isFsi", model.IsFsi], ["code", code]]);
}

export function encodeInput(input) {
    let values;
    const value_4 = object([["sourceCode", input.SourceCode], ["defines", (values = map((value_1) => value_1, input.Defines), (values))], ["isFsi", input.IsFsi]]);
    return toString(2, value_4);
}

