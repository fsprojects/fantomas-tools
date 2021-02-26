import { map, list, array as array_2, fromString, bool, int, tuple3, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { comparePrimitives, uncurry } from "../.fable/fable-library.3.1.1/Util.js";
import { FormatResponse, ASTError, ASTErrorSeverity, Range$, sortByOption, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { Result_Map } from "../.fable/fable-library.3.1.1/Choice.js";
import { ofArray } from "../.fable/fable-library.3.1.1/List.js";
import { sortBy } from "../.fable/fable-library.3.1.1/Array.js";

const optionDecoder = (path_8) => ((v) => object((get$) => {
    const t = get$.Required.Field("$type", string);
    if (t === "int") {
        const tupledArg = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), string, uncurry(2, int))));
        return new FantomasOption(0, tupledArg[0], tupledArg[1], tupledArg[2]);
    }
    else if (t === "bool") {
        const tupledArg_1 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), string, bool)));
        return new FantomasOption(1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2]);
    }
    else if (t === "multilineFormatterType") {
        const tupledArg_2 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), string, string)));
        return new FantomasOption(2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2]);
    }
    else {
        const tupledArg_3 = get$.Required.Field("$value", uncurry(2, tuple3(uncurry(2, int), string, string)));
        return new FantomasOption(3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2]);
    }
}, path_8, v));

export function decodeOptions(json) {
    return Result_Map((arg) => ofArray(sortBy(sortByOption, arg, {
        Compare: comparePrimitives,
    })), fromString((path, value) => array_2(uncurry(2, optionDecoder), path, value), json));
}

export const decodeOptionsFromUrl = (path_2) => ((v) => object((get$) => [get$.Required.Field("settings", (path, value) => list(uncurry(2, optionDecoder), path, value)), get$.Required.Field("isFsi", bool)], path_2, v));

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
}, string, path_1, value_1));

const decodeASTError = (path_2) => ((v) => object((get$) => (new ASTError(get$.Required.Field("subCategory", string), get$.Required.Field("range", uncurry(2, decodeRange)), get$.Required.Field("severity", uncurry(2, decoderASTErrorSeverity)), get$.Required.Field("errorNumber", uncurry(2, int)), get$.Required.Field("message", string))), path_2, v));

export const decodeFormatResponse = (path_4) => ((v) => object((get$) => (new FormatResponse(get$.Required.Field("firstFormat", string), get$.Required.Field("firstValidation", (path_1, value_1) => list(uncurry(2, decodeASTError), path_1, value_1)), get$.Optional.Field("secondFormat", string), get$.Required.Field("secondValidation", (path_3, value_3) => list(uncurry(2, decodeASTError), path_3, value_3)))), path_4, v));

