import { list, object, toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { map } from "../.fable/fable-library.3.1.1/List.js";

export function encodeParseRequest(pr) {
    return toString(4, object([["sourceCode", pr.SourceCode], ["defines", list(map((value_1) => value_1, pr.Defines))], ["fileName", pr.FileName]]));
}

export function encodeUrlModel(code, model) {
    return object([["code", code], ["defines", model.Defines], ["isFsi", model.IsFsi]]);
}

