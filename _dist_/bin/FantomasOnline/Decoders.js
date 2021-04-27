import { map, list, array as array_2, fromString, bool, int, tuple3, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { comparePrimitives, uncurry } from "../.fable/fable-library.3.1.15/Util.js";
import { FormatResponse, ASTError, ASTErrorSeverity, Range$, sortByOption, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { Result_Map } from "../.fable/fable-library.3.1.15/Choice.js";
import { ofArray } from "../.fable/fable-library.3.1.15/List.js";
import { sortBy } from "../.fable/fable-library.3.1.15/Array.js";

const optionDecoder = (path_8) => ((v) => object((get$) => {
    const t = get$.Required.Field("$type", (path, value) => string(path, value));
    if (t === "int") {
        const tupledArg = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), (path_1, value_1) => string(path_1, value_1), uncurry(2, int))));
        return new FantomasOption(0, tupledArg[0], tupledArg[1], tupledArg[2]);
    }
    else if (t === "bool") {
        const tupledArg_1 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), (path_2, value_2) => string(path_2, value_2), (path_3, value_3) => bool(path_3, value_3))));
        return new FantomasOption(1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2]);
    }
    else if (t === "multilineFormatterType") {
        const tupledArg_2 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), (path_4, value_4) => string(path_4, value_4), (path_5, value_5) => string(path_5, value_5))));
        return new FantomasOption(2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2]);
    }
    else {
        const tupledArg_3 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), (path_6, value_6) => string(path_6, value_6), (path_7, value_7) => string(path_7, value_7))));
        return new FantomasOption(3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2]);
    }
}, path_8, v));

export function decodeOptions(json) {
    return Result_Map((arg) => ofArray(sortBy((_arg1) => sortByOption(_arg1), arg, {
        Compare: (x, y) => comparePrimitives(x, y),
    })), fromString((path, value) => array_2(uncurry(2, optionDecoder), path, value), json));
}

export const decodeOptionsFromUrl = (path_2) => ((v) => object((get$) => [get$.Required.Field("settings", (path, value) => list(uncurry(2, optionDecoder), path, value)), get$.Required.Field("isFsi", (path_1, value_1) => bool(path_1, value_1))], path_2, v));

const decodeRange = (path) => ((v) => object((get$) => (new Range$(get$.Required.Field("startLine", uncurry(2, int)), get$.Required.Field("startCol", uncurry(2, int)), get$.Required.Field("endLine", uncurry(2, int)), get$.Required.Field("endCol", uncurry(2, int)))), path, v));

const decoderASTErrorSeverity = (path_1) => ((value_1) => map((s) => {
    switch (s) {
        case "error": {
            return new ASTErrorSeverity(0);
        }
        case "warning": {
            return new ASTErrorSeverity(1);
        }
        case "info": {
            return new ASTErrorSeverity(2);
        }
        default: {
            return new ASTErrorSeverity(3);
        }
    }
}, (path, value) => string(path, value), path_1, value_1));

const decodeASTError = (path_2) => ((v) => object((get$) => (new ASTError(get$.Required.Field("subCategory", (path, value) => string(path, value)), get$.Required.Field("range", uncurry(2, decodeRange)), get$.Required.Field("severity", uncurry(2, decoderASTErrorSeverity)), get$.Required.Field("errorNumber", uncurry(2, int)), get$.Required.Field("message", (path_1, value_1) => string(path_1, value_1)))), path_2, v));

export const decodeFormatResponse = (path_4) => ((v) => object((get$) => (new FormatResponse(get$.Required.Field("firstFormat", (path, value) => string(path, value)), get$.Required.Field("firstValidation", (path_1, value_1) => list(uncurry(2, decodeASTError), path_1, value_1)), get$.Optional.Field("secondFormat", (path_2, value_2) => string(path_2, value_2)), get$.Required.Field("secondValidation", (path_3, value_3) => list(uncurry(2, decodeASTError), path_3, value_3)))), path_4, v));

