import { toString, object } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map } from "../.fable/fable-library.3.1.7/Array.js";

export function encodeUrlModel(code, model) {
    return object([["defines", model.Defines], ["isFsi", model.IsFsi], ["code", code]]);
}

export function encodeInput(input) {
    return toString(2, object([["sourceCode", input.SourceCode], ["defines", map((value_1) => value_1, input.Defines)], ["isFsi", input.IsFsi]]));
}

