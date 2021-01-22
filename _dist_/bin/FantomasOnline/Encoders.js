import { toString, list as list_2, object, tuple3 } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { sortBy, map } from "../.fable/fable-library.3.1.1/List.js";
import { getOptionKey, sortByOption } from "../shared/FantomasOnlineShared.js";
import { toList } from "../.fable/fable-library.3.1.1/Map.js";
import { comparePrimitives } from "../.fable/fable-library.3.1.1/Util.js";

function encodeOption(fantomasOption) {
    const patternInput = (fantomasOption.tag === 1) ? ["bool", tuple3((value_6) => value_6, (value_8) => value_8, (value_10) => value_10, fantomasOption.fields[0], fantomasOption.fields[1], fantomasOption.fields[2])] : ((fantomasOption.tag === 2) ? ["multilineFormatterType", tuple3((value_12) => value_12, (value_14) => value_14, (value_16) => value_16, fantomasOption.fields[0], fantomasOption.fields[1], fantomasOption.fields[2])] : ((fantomasOption.tag === 3) ? ["endOfLineStyle", tuple3((value_18) => value_18, (value_20) => value_20, (value_22) => value_22, fantomasOption.fields[0], fantomasOption.fields[1], fantomasOption.fields[2])] : ["int", tuple3((value) => value, (value_2) => value_2, (value_4) => value_4, fantomasOption.fields[0], fantomasOption.fields[1], fantomasOption.fields[2])]));
    return object([["$type", patternInput[0]], ["$value", patternInput[1]]]);
}

function encodeUserSettings(model) {
    return list_2(map((arg_1) => encodeOption(arg_1[1]), sortBy((arg) => sortByOption(arg[1]), toList(model.UserOptions), {
        Compare: comparePrimitives,
    })));
}

export function encodeRequest(code, model) {
    return toString(2, object([["sourceCode", code], ["options", encodeUserSettings(model)], ["isFsi", model.IsFsi]]));
}

export function encodeUrlModel(code, model) {
    return object([["code", code], ["settings", encodeUserSettings(model)], ["isFsi", model.IsFsi]]);
}

export function encodeUserSettingToConfiguration(options) {
    return toString(4, object(map((option_1) => {
        let option;
        return [getOptionKey(option_1), (option = option_1, (option.tag === 1) ? option.fields[2] : ((option.tag === 2) ? option.fields[2] : ((option.tag === 3) ? option.fields[2] : option.fields[2])))];
    }, options)));
}

