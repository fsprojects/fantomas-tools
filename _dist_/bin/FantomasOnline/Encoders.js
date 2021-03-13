import { toString, list as list_2, object, tuple3 } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { sortBy, map } from "../.fable/fable-library.3.1.7/List.js";
import { getOptionKey, sortByOption } from "../shared/FantomasOnlineShared.js";
import { toList } from "../.fable/fable-library.3.1.7/Map.js";
import { comparePrimitives } from "../.fable/fable-library.3.1.7/Util.js";

function encodeOption(fantomasOption) {
    let patternInput;
    switch (fantomasOption.tag) {
        case 1: {
            const v_1 = fantomasOption.fields[2];
            const o_1 = fantomasOption.fields[0] | 0;
            const k_1 = fantomasOption.fields[1];
            patternInput = ["bool", tuple3((value_6) => value_6, (value_8) => value_8, (value_10) => value_10, o_1, k_1, v_1)];
            break;
        }
        case 2: {
            const v_2 = fantomasOption.fields[2];
            const o_2 = fantomasOption.fields[0] | 0;
            const k_2 = fantomasOption.fields[1];
            patternInput = ["multilineFormatterType", tuple3((value_12) => value_12, (value_14) => value_14, (value_16) => value_16, o_2, k_2, v_2)];
            break;
        }
        case 3: {
            const v_3 = fantomasOption.fields[2];
            const o_3 = fantomasOption.fields[0] | 0;
            const k_3 = fantomasOption.fields[1];
            patternInput = ["endOfLineStyle", tuple3((value_18) => value_18, (value_20) => value_20, (value_22) => value_22, o_3, k_3, v_3)];
            break;
        }
        default: {
            const v = fantomasOption.fields[2] | 0;
            const o = fantomasOption.fields[0] | 0;
            const k = fantomasOption.fields[1];
            patternInput = ["int", tuple3((value) => value, (value_2) => value_2, (value_4) => value_4, o, k, v)];
        }
    }
    const value_24 = patternInput[1];
    const key = patternInput[0];
    return object([["$type", key], ["$value", value_24]]);
}

function encodeUserSettings(model) {
    return list_2(map((arg_1) => encodeOption(arg_1[1]), sortBy((arg) => sortByOption(arg[1]), toList(model.UserOptions), {
        Compare: (x, y) => comparePrimitives(x, y),
    })));
}

export function encodeRequest(code, model) {
    return toString(2, object([["sourceCode", code], ["options", encodeUserSettings(model)], ["isFsi", model.IsFsi]]));
}

export function encodeUrlModel(code, model) {
    return object([["code", code], ["settings", encodeUserSettings(model)], ["isFsi", model.IsFsi]]);
}

export function encodeUserSettingToConfiguration(options) {
    const encodeValue = (option) => {
        let pattern_matching_result, v_2;
        switch (option.tag) {
            case 1: {
                pattern_matching_result = 1;
                break;
            }
            case 2: {
                pattern_matching_result = 2;
                v_2 = option.fields[2];
                break;
            }
            case 3: {
                pattern_matching_result = 2;
                v_2 = option.fields[2];
                break;
            }
            default: pattern_matching_result = 0}
        switch (pattern_matching_result) {
            case 0: {
                const v = option.fields[2] | 0;
                return v;
            }
            case 1: {
                const v_1 = option.fields[2];
                return v_1;
            }
            case 2: {
                return v_2;
            }
        }
    };
    return toString(4, object(map((option_1) => [getOptionKey(option_1), encodeValue(option_1)], options)));
}

