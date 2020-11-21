import { toString, list, object } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";

export function encodeParseRequest(pr) {
    let values;
    const value_4 = object([["sourceCode", pr.SourceCode], ["defines", (values = map((value_1) => value_1, pr.Defines), (list(values)))], ["fileName", pr.FileName]]);
    return toString(4, value_4);
}

export function encodeUrlModel(code, model) {
    return object([["code", code], ["defines", model.Defines], ["isFsi", model.IsFsi]]);
}

