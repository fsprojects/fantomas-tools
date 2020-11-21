import { list, array as array_2, fromString, bool, int, tuple3, string, object } from "../bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { comparePrimitives, uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { sortByOption, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { Result_Map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";
import { sortBy } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { ofArray } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";

const optionDecoder = (path_8) => ((v) => object((get$) => {
    let t;
    const objectArg = get$.Required;
    t = objectArg.Field("$type", string);
    if (t === "int") {
        let tupledArg;
        const arg10_1 = tuple3(uncurry(2, int), string, uncurry(2, int));
        const objectArg_1 = get$.Required;
        tupledArg = objectArg_1.Field("$value", uncurry(2, arg10_1));
        return new FantomasOption(0, tupledArg[0], tupledArg[1], tupledArg[2]);
    }
    else if (t === "bool") {
        let tupledArg_1;
        const arg10_2 = tuple3(uncurry(2, int), string, bool);
        const objectArg_2 = get$.Required;
        tupledArg_1 = objectArg_2.Field("$value", uncurry(2, arg10_2));
        return new FantomasOption(1, tupledArg_1[0], tupledArg_1[1], tupledArg_1[2]);
    }
    else if (t === "multilineFormatterType") {
        let tupledArg_2;
        const arg10_3 = tuple3(uncurry(2, int), string, string);
        const objectArg_3 = get$.Required;
        tupledArg_2 = objectArg_3.Field("$value", uncurry(2, arg10_3));
        return new FantomasOption(2, tupledArg_2[0], tupledArg_2[1], tupledArg_2[2]);
    }
    else {
        let tupledArg_3;
        const arg10_4 = tuple3(uncurry(2, int), string, string);
        const objectArg_4 = get$.Required;
        tupledArg_3 = objectArg_4.Field("$value", uncurry(2, arg10_4));
        return new FantomasOption(3, tupledArg_3[0], tupledArg_3[1], tupledArg_3[2]);
    }
}, path_8, v));

export function decodeOptions(json) {
    const result = fromString((path, value) => array_2(uncurry(2, optionDecoder), path, value), json);
    return Result_Map((arg) => {
        let array_1;
        array_1 = sortBy(sortByOption, arg, {
            Compare: comparePrimitives,
        });
        return ofArray(array_1);
    }, result);
}

export const decodeOptionsFromUrl = (path_2) => ((v) => object((get$) => {
    let settings;
    const objectArg = get$.Required;
    settings = objectArg.Field("settings", (path, value) => list(uncurry(2, optionDecoder), path, value));
    let isFSI;
    const objectArg_1 = get$.Required;
    isFSI = objectArg_1.Field("isFsi", bool);
    return [settings, isFSI];
}, path_2, v));

