import { toFail, printf, toText, join } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { int16ToString, mapCurriedArgs, compare, int32ToString, uncurry, partialApply } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Result_Map, FSharpResult$2 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";
import { Util_CachedDecoders, Util_Cache$1__GetOrAdd_43981464, CaseStrategy, Util_Casing_convert, ErrorReason } from "./Types.fs.js";
import { tryParse as tryParse_2 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Guid.js";
import { toString as toString_12, FSharpRef } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { tryParse as tryParse_3 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Int32.js";
import { tryParse as tryParse_4, fromInt, fromNumber, toNumber, fromBits } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Long.js";
import { parse, fromInt32 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/BigInt.js";
import { tryParse as tryParse_5 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Decimal.js";
import Decimal from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Decimal.js";
import { toUniversalTime, tryParse as tryParse_6, minValue } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Date.js";
import { tryParse as tryParse_7, minValue as minValue_1 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/DateOffset.js";
import { tryParse as tryParse_8 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/TimeSpan.js";
import { map as map_4, defaultArg, some } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { ofArray, map as map_1, length, singleton, append as append_1, ofSeq, reverse, empty, fold, tryLast, cons } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { map as map_2, tryFind, foldBack2, foldBack, fill, fold as fold_1 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { contains, fold as fold_2, reverse as reverse_1, append } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { empty as empty_1, map as map_3, tryFind as tryFind_1, add, ofSeq as ofSeq_1, ofList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Map.js";
import { getGenerics, getGenericTypeDefinition, makeTuple, getTupleElements, isTuple, isGenericType, parseEnum, getEnumValues, getEnumUnderlyingType, isEnum, getElementType, isArray, isUnion, makeRecord, getRecordElements, isRecord, fullName, getUnionCaseFields, makeUnion as makeUnion_1, name as name_3, getUnionCases, class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { ofSeq as ofSeq_2 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Set.js";

export function Helpers_isUndefined(o) {
    return (typeof o) === "undefined";
}

function genericMsg(msg, value_1, newLine) {
    try {
        return ((("Expecting " + msg) + " but instead got:") + (newLine ? "\n" : " ")) + (JSON.stringify(value_1, null, 4) + '');
    }
    catch (matchValue) {
        return (("Expecting " + msg) + " but decoder failed. Couldn\u0027t report given value due to circular structure.") + (newLine ? "\n" : " ");
    }
}

function errorToString(path, error) {
    const reason_1 = (error.tag === 2) ? genericMsg(error.fields[0], error.fields[1], true) : ((error.tag === 1) ? ((genericMsg(error.fields[0], error.fields[1], false) + "\nReason: ") + error.fields[2]) : ((error.tag === 3) ? genericMsg(error.fields[0], error.fields[1], true) : ((error.tag === 4) ? (genericMsg(error.fields[0], error.fields[1], true) + (("\nNode `" + error.fields[2]) + "` is unkown.")) : ((error.tag === 5) ? ((("Expecting " + error.fields[0]) + ".\n") + (JSON.stringify(error.fields[1], null, 4) + '')) : ((error.tag === 7) ? ("The following errors were found:\n\n" + join("\n\n", error.fields[0])) : ((error.tag === 6) ? ("The following `failure` occurred with the decoder: " + error.fields[0]) : genericMsg(error.fields[0], error.fields[1], false)))))));
    if (error.tag === 7) {
        return reason_1;
    }
    else {
        return (("Error at: `" + path) + "`\n") + reason_1;
    }
}

export function fromValue(path, decoder, value_1) {
    let matchValue;
    const clo1 = partialApply(1, decoder, [path]);
    matchValue = clo1(value_1);
    if (matchValue.tag === 1) {
        const error = matchValue.fields[0];
        return new FSharpResult$2(1, errorToString(error[0], error[1]));
    }
    else {
        return new FSharpResult$2(0, matchValue.fields[0]);
    }
}

export function fromString(decoder, value_1) {
    try {
        const json = JSON.parse(value_1);
        return fromValue("$", decoder, json);
    }
    catch (matchValue) {
        if (matchValue instanceof SyntaxError) {
            return new FSharpResult$2(1, "Given an invalid JSON: " + matchValue.message);
        }
        else {
            throw matchValue;
        }
    }
}

export function unsafeFromString(decoder, value_1) {
    const matchValue = fromString(decoder, value_1);
    if (matchValue.tag === 1) {
        throw (new Error(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

export function decodeValue(path, decoder) {
    const decoder_1 = decoder;
    return (value_1) => fromValue(path, decoder_1, value_1);
}

export function decodeString(decoder) {
    const decoder_1 = decoder;
    return (value_1) => fromString(decoder_1, value_1);
}

export function string(path, value_1) {
    if ((typeof value_1) === "string") {
        return new FSharpResult$2(0, value_1);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a string", value_1)]);
    }
}

export function guid(path, value_1) {
    if ((typeof value_1) === "string") {
        let matchValue;
        let outArg = "00000000-0000-0000-0000-000000000000";
        matchValue = [tryParse_2(value_1, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a guid", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a guid", value_1)]);
    }
}

export function unit(path, value_1) {
    if (value_1 == null) {
        return new FSharpResult$2(0, void 0);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "null", value_1)]);
    }
}

export const sbyte = (path) => ((value_2) => {
    const name_1 = "a sbyte";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((-128 <= value_4) ? (value_4 <= 127) : false) {
                return new FSharpResult$2(0, ((value_4 + 0x80 & 0xFF) - 0x80));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, false, 8, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const byte = (path) => ((value_2) => {
    const name_1 = "a byte";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((0 <= value_4) ? (value_4 <= 255) : false) {
                return new FSharpResult$2(0, (value_4 & 0xFF));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, true, 8, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const int16 = (path) => ((value_2) => {
    const name_1 = "an int16";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((-32768 <= value_4) ? (value_4 <= 32767) : false) {
                return new FSharpResult$2(0, ((value_4 + 0x8000 & 0xFFFF) - 0x8000));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, false, 16, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const uint16 = (path) => ((value_2) => {
    const name_1 = "an uint16";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((0 <= value_4) ? (value_4 <= 65535) : false) {
                return new FSharpResult$2(0, (value_4 & 0xFFFF));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, true, 16, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const int = (path) => ((value_2) => {
    const name_1 = "an int";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((-2147483648 <= value_4) ? (value_4 <= 2147483647) : false) {
                return new FSharpResult$2(0, (~(~value_4)));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, false, 32, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const uint32 = (path) => ((value_2) => {
    const name_1 = "an uint32";
    const path_1 = path;
    const value_3 = value_2;
    if ((typeof value_3) === "number") {
        const value_4 = value_3;
        if (isFinite(value_4) && Math.floor(value_4) === value_4) {
            if ((0 <= value_4) ? (value_4 <= 4294967295) : false) {
                return new FSharpResult$2(0, (value_4 >>> 0));
            }
            else {
                const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                return new FSharpResult$2(1, arg0);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
        }
    }
    else if ((typeof value_3) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_3(value_3, 511, true, 32, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
    }
});

export const int64 = (() => {
    const min = fromBits(0, 2147483648, false);
    const max = fromBits(4294967295, 2147483647, false);
    return (path) => ((value_2) => {
        const name_1 = "an int64";
        const path_1 = path;
        const value_3 = value_2;
        if ((typeof value_3) === "number") {
            const value_4 = value_3;
            if (isFinite(value_4) && Math.floor(value_4) === value_4) {
                if ((toNumber(min) <= value_4) ? (value_4 <= toNumber(max)) : false) {
                    return new FSharpResult$2(0, (fromNumber(value_4, false)));
                }
                else {
                    const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                    return new FSharpResult$2(1, arg0);
                }
            }
            else {
                return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
            }
        }
        else if ((typeof value_3) === "string") {
            let matchValue;
            let outArg = fromInt(0);
            matchValue = [tryParse_4(value_3, 511, false, 64, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            if (matchValue[0]) {
                return new FSharpResult$2(0, matchValue[1]);
            }
            else {
                return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    });
})();

export const uint64 = (() => {
    const min = fromBits(0, 0, true);
    const max = fromBits(4294967295, 4294967295, true);
    return (path) => ((value_2) => {
        const name_1 = "an uint64";
        const path_1 = path;
        const value_3 = value_2;
        if ((typeof value_3) === "number") {
            const value_4 = value_3;
            if (isFinite(value_4) && Math.floor(value_4) === value_4) {
                if ((toNumber(min) <= value_4) ? (value_4 <= toNumber(max)) : false) {
                    return new FSharpResult$2(0, (fromNumber(value_4, true)));
                }
                else {
                    const arg0 = [path_1, new ErrorReason(1, name_1, value_4, "Value was either too large or too small for " + name_1)];
                    return new FSharpResult$2(1, arg0);
                }
            }
            else {
                return new FSharpResult$2(1, [path_1, new ErrorReason(1, name_1, value_4, "Value is not an integral value")]);
            }
        }
        else if ((typeof value_3) === "string") {
            let matchValue;
            let outArg = fromInt(0);
            matchValue = [tryParse_4(value_3, 511, true, 64, new FSharpRef(() => outArg, (v) => {
                outArg = v;
            })), outArg];
            if (matchValue[0]) {
                return new FSharpResult$2(0, matchValue[1]);
            }
            else {
                return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
            }
        }
        else {
            return new FSharpResult$2(1, [path_1, new ErrorReason(0, name_1, value_3)]);
        }
    });
})();

export function bigint(path, value_1) {
    if ((typeof value_1) === "number") {
        let arg0;
        arg0 = fromInt32(value_1);
        return new FSharpResult$2(0, arg0);
    }
    else if ((typeof value_1) === "string") {
        try {
            const arg0_1 = parse(value_1);
            return new FSharpResult$2(0, arg0_1);
        }
        catch (matchValue) {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a bigint", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a bigint", value_1)]);
    }
}

export function bool(path, value_1) {
    if ((typeof value_1) === "boolean") {
        return new FSharpResult$2(0, value_1);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a boolean", value_1)]);
    }
}

export function float(path, value_1) {
    if ((typeof value_1) === "number") {
        return new FSharpResult$2(0, value_1);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a float", value_1)]);
    }
}

export function float32(path, value_1) {
    if ((typeof value_1) === "number") {
        return new FSharpResult$2(0, value_1);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a float32", value_1)]);
    }
}

export function decimal(path, value_1) {
    if ((typeof value_1) === "number") {
        let arg0;
        arg0 = (new Decimal(value_1));
        return new FSharpResult$2(0, arg0);
    }
    else if ((typeof value_1) === "string") {
        let matchValue;
        let outArg = new Decimal(0);
        matchValue = [tryParse_5(value_1, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a decimal", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a decimal", value_1)]);
    }
}

export function datetime(path, value_1) {
    if ((typeof value_1) === "string") {
        let matchValue;
        let outArg = minValue();
        matchValue = [tryParse_6(value_1, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            const arg0 = toUniversalTime(matchValue[1]);
            return new FSharpResult$2(0, arg0);
        }
        else {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a datetime", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a datetime", value_1)]);
    }
}

export function datetimeOffset(path, value_1) {
    if ((typeof value_1) === "string") {
        let matchValue;
        let outArg = minValue_1();
        matchValue = [tryParse_7(value_1, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a datetimeoffset", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a datetime", value_1)]);
    }
}

export function timespan(path, value_1) {
    if ((typeof value_1) === "string") {
        let matchValue;
        let outArg = 0;
        matchValue = [tryParse_8(value_1, new FSharpRef(() => outArg, (v) => {
            outArg = v;
        })), outArg];
        if (matchValue[0]) {
            return new FSharpResult$2(0, matchValue[1]);
        }
        else {
            return new FSharpResult$2(1, [path, new ErrorReason(0, "a timespan", value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a timespan", value_1)]);
    }
}

function decodeMaybeNull(path, decoder, value_1) {
    let matchValue;
    const clo1 = partialApply(1, decoder, [path]);
    matchValue = clo1(value_1);
    if (matchValue.tag === 1) {
        if (value_1 == null) {
            return new FSharpResult$2(0, void 0);
        }
        else if (matchValue.tag === 1) {
            return new FSharpResult$2(1, matchValue.fields[0]);
        }
        else {
            throw (new Error("The match cases were incomplete"));
        }
    }
    else {
        return new FSharpResult$2(0, some(matchValue.fields[0]));
    }
}

export function optional(fieldName, decoder, path, value_1) {
    if (value_1 === null ? false : (Object.getPrototypeOf(value_1 || false) === Object.prototype)) {
        const fieldValue = value_1[fieldName];
        if (Helpers_isUndefined(fieldValue)) {
            return new FSharpResult$2(0, void 0);
        }
        else {
            return decodeMaybeNull((path + ".") + fieldName, decoder, fieldValue);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(2, "an object", value_1)]);
    }
}

function badPathError(fieldNames, currentPath, value_1) {
    let option_1;
    const currentPath_1 = defaultArg(currentPath, (join(".", cons("$", fieldNames))));
    const msg = ("an object with path `" + join(".", fieldNames)) + "`";
    return new FSharpResult$2(1, [currentPath_1, new ErrorReason(4, msg, value_1, (option_1 = tryLast(fieldNames), (defaultArg(option_1, ""))))]);
}

export function optionalAt(fieldNames, decoder, firstPath, firstValue) {
    let _arg1;
    _arg1 = fold(uncurry(2, (tupledArg) => {
        const curPath = tupledArg[0];
        const curValue = tupledArg[1];
        const res = tupledArg[2];
        return (field_1) => {
            if (res == null) {
                if (curValue == null) {
                    return [curPath, curValue, new FSharpResult$2(0, void 0)];
                }
                else if (curValue === null ? false : (Object.getPrototypeOf(curValue || false) === Object.prototype)) {
                    const curValue_1 = curValue[field_1];
                    return [(curPath + ".") + field_1, curValue_1, void 0];
                }
                else {
                    return [curPath, curValue, new FSharpResult$2(1, [curPath, new ErrorReason(2, "an object", curValue)])];
                }
            }
            else {
                return [curPath, curValue, res];
            }
        };
    }), [firstPath, firstValue, void 0], fieldNames);
    if (_arg1[2] == null) {
        const lastValue = _arg1[1];
        if (Helpers_isUndefined(lastValue)) {
            return new FSharpResult$2(0, void 0);
        }
        else {
            return decodeMaybeNull(_arg1[0], decoder, lastValue);
        }
    }
    else {
        const res_2 = _arg1[2];
        return res_2;
    }
}

export function field(fieldName, decoder, path, value_1) {
    if (value_1 === null ? false : (Object.getPrototypeOf(value_1 || false) === Object.prototype)) {
        const fieldValue = value_1[fieldName];
        if (Helpers_isUndefined(fieldValue)) {
            return new FSharpResult$2(1, [path, new ErrorReason(3, ("an object with a field named `" + fieldName) + "`", value_1)]);
        }
        else {
            return decoder((path + ".") + fieldName, fieldValue);
        }
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(2, "an object", value_1)]);
    }
}

export function at(fieldNames, decoder, firstPath, firstValue) {
    let _arg1;
    _arg1 = fold(uncurry(2, (tupledArg) => {
        const curPath = tupledArg[0];
        const curValue = tupledArg[1];
        const res = tupledArg[2];
        return (field_1) => {
            if (res == null) {
                if (curValue == null) {
                    const res_1 = badPathError(fieldNames, curPath, firstValue);
                    return [curPath, curValue, res_1];
                }
                else if (curValue === null ? false : (Object.getPrototypeOf(curValue || false) === Object.prototype)) {
                    const curValue_1 = curValue[field_1];
                    if (Helpers_isUndefined(curValue_1)) {
                        const res_2 = badPathError(fieldNames, void 0, firstValue);
                        return [curPath, curValue_1, res_2];
                    }
                    else {
                        return [(curPath + ".") + field_1, curValue_1, void 0];
                    }
                }
                else {
                    return [curPath, curValue, new FSharpResult$2(1, [curPath, new ErrorReason(2, "an object", curValue)])];
                }
            }
            else {
                return [curPath, curValue, res];
            }
        };
    }), [firstPath, firstValue, void 0], fieldNames);
    if (_arg1[2] == null) {
        return decoder(_arg1[0], _arg1[1]);
    }
    else {
        const res_4 = _arg1[2];
        return res_4;
    }
}

export function index(requestedIndex, decoder, path, value_1) {
    let copyOfStruct;
    const currentPath = ((path + ".[") + int32ToString(requestedIndex)) + "]";
    if (Array.isArray(value_1)) {
        const vArray = value_1;
        if (requestedIndex < vArray.length) {
            return decoder(currentPath, vArray[requestedIndex]);
        }
        else {
            const msg = ((("a longer array. Need index `" + int32ToString(requestedIndex)) + "` but there are only `") + (copyOfStruct = (vArray.length | 0), int32ToString(copyOfStruct))) + "` entries";
            return new FSharpResult$2(1, [currentPath, new ErrorReason(5, msg, value_1)]);
        }
    }
    else {
        return new FSharpResult$2(1, [currentPath, new ErrorReason(0, "an array", value_1)]);
    }
}

export function option(decoder, path, value_1) {
    if (value_1 == null) {
        return new FSharpResult$2(0, void 0);
    }
    else {
        const result = decoder(path, value_1);
        return Result_Map(some, result);
    }
}

export function list(decoder, path, value_1) {
    if (Array.isArray(value_1)) {
        let i = -1;
        let result;
        result = fold_1((acc, value_2) => {
            i = (i + 1);
            if (acc.tag === 0) {
                const matchValue = decoder(((path + ".[") + int32ToString(i)) + "]", value_2);
                if (matchValue.tag === 0) {
                    return new FSharpResult$2(0, cons(matchValue.fields[0], acc.fields[0]));
                }
                else {
                    return new FSharpResult$2(1, matchValue.fields[0]);
                }
            }
            else {
                return acc;
            }
        }, new FSharpResult$2(0, empty()), value_1);
        return Result_Map(reverse, result);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a list", value_1)]);
    }
}

export function seq(decoder, path, value_1) {
    if (Array.isArray(value_1)) {
        let i = -1;
        let result;
        result = fold_1((acc, value_2) => {
            i = (i + 1);
            if (acc.tag === 0) {
                const matchValue = decoder(((path + ".[") + int32ToString(i)) + "]", value_2);
                if (matchValue.tag === 0) {
                    return new FSharpResult$2(0, append([matchValue.fields[0]], acc.fields[0]));
                }
                else {
                    return new FSharpResult$2(1, matchValue.fields[0]);
                }
            }
            else {
                return acc;
            }
        }, new FSharpResult$2(0, []), value_1);
        return Result_Map(reverse_1, result);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "a seq", value_1)]);
    }
}

export function array(decoder, path, value_1) {
    if (Array.isArray(value_1)) {
        let i = -1;
        const tokens = value_1;
        const arr = fill(new Array(tokens.length), 0, tokens.length, null);
        return fold_1((acc, value_2) => {
            i = (i + 1);
            if (acc.tag === 0) {
                const acc_1 = acc.fields[0];
                const matchValue = decoder(((path + ".[") + int32ToString(i)) + "]", value_2);
                if (matchValue.tag === 0) {
                    acc_1[i] = matchValue.fields[0];
                    return new FSharpResult$2(0, acc_1);
                }
                else {
                    return new FSharpResult$2(1, matchValue.fields[0]);
                }
            }
            else {
                return acc;
            }
        }, new FSharpResult$2(0, arr), tokens);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "an array", value_1)]);
    }
}

export function keys(path, value_1) {
    if (value_1 === null ? false : (Object.getPrototypeOf(value_1 || false) === Object.prototype)) {
        let arg0;
        const source = Object.keys(value_1);
        arg0 = ofSeq(source);
        return new FSharpResult$2(0, arg0);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "an object", value_1)]);
    }
}

export function keyValuePairs(decoder, path, value_1) {
    const matchValue = keys(path, value_1);
    if (matchValue.tag === 1) {
        return new FSharpResult$2(1, matchValue.fields[0]);
    }
    else {
        let result;
        result = fold((acc, prop) => {
            if (acc.tag === 0) {
                const matchValue_1 = decoder(path, value_1[prop]);
                if (matchValue_1.tag === 0) {
                    return new FSharpResult$2(0, cons([prop, matchValue_1.fields[0]], acc.fields[0]));
                }
                else {
                    return new FSharpResult$2(1, matchValue_1.fields[0]);
                }
            }
            else {
                return acc;
            }
        }, new FSharpResult$2(0, empty()), matchValue.fields[0]);
        return Result_Map(reverse, result);
    }
}

export function oneOf(decoders, path, value_1) {
    const runner = (decoders_1_mut, errors_mut) => {
        runner:
        while (true) {
            const decoders_1 = decoders_1_mut, errors = errors_mut;
            if (decoders_1.tail == null) {
                return new FSharpResult$2(1, [path, new ErrorReason(7, errors)]);
            }
            else {
                const matchValue = fromValue(path, uncurry(2, decoders_1.head), value_1);
                if (matchValue.tag === 1) {
                    decoders_1_mut = decoders_1.tail;
                    errors_mut = append_1(errors, singleton(matchValue.fields[0]));
                    continue runner;
                }
                else {
                    return new FSharpResult$2(0, matchValue.fields[0]);
                }
            }
            break;
        }
    };
    return runner(decoders, empty());
}

export function nil(output, path, value_1) {
    if (value_1 == null) {
        return new FSharpResult$2(0, output);
    }
    else {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "null", value_1)]);
    }
}

export function value(_arg1, v) {
    return new FSharpResult$2(0, v);
}

export function succeed(output, _arg2, _arg1) {
    return new FSharpResult$2(0, output);
}

export function fail(msg, path, _arg1) {
    return new FSharpResult$2(1, [path, new ErrorReason(6, msg)]);
}

export function andThen(cb, decoder, path, value_1) {
    const matchValue = decoder(path, value_1);
    if (matchValue.tag === 0) {
        return cb(matchValue.fields[0], path, value_1);
    }
    else {
        return new FSharpResult$2(1, matchValue.fields[0]);
    }
}

export function all(decoders, path, value_1) {
    const runner = (decoders_1_mut, values_mut) => {
        runner:
        while (true) {
            const decoders_1 = decoders_1_mut, values = values_mut;
            if (decoders_1.tail == null) {
                return new FSharpResult$2(0, values);
            }
            else {
                const matchValue = decoders_1.head(path)(value_1);
                if (matchValue.tag === 1) {
                    return new FSharpResult$2(1, matchValue.fields[0]);
                }
                else {
                    decoders_1_mut = decoders_1.tail;
                    values_mut = append_1(values, singleton(matchValue.fields[0]));
                    continue runner;
                }
            }
            break;
        }
    };
    return runner(decoders, empty());
}

export function map(ctor, d1, path, value_1) {
    const matchValue = d1(path, value_1);
    if (matchValue.tag === 1) {
        return new FSharpResult$2(1, matchValue.fields[0]);
    }
    else {
        return new FSharpResult$2(0, ctor(matchValue.fields[0]));
    }
}

export function map2(ctor, d1, d2, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0]));
        }
    }
}

export function map3(ctor, d1, d2, d3, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0]));
            }
        }
    }
}

export function map4(ctor, d1, d2, d3, d4, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1), d4(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                const copyOfStruct_3 = matchValue[3];
                if (copyOfStruct_3.tag === 1) {
                    return new FSharpResult$2(1, copyOfStruct_3.fields[0]);
                }
                else {
                    return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0]));
                }
            }
        }
    }
}

export function map5(ctor, d1, d2, d3, d4, d5, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1), d4(path, value_1), d5(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                const copyOfStruct_3 = matchValue[3];
                if (copyOfStruct_3.tag === 1) {
                    return new FSharpResult$2(1, copyOfStruct_3.fields[0]);
                }
                else {
                    const copyOfStruct_4 = matchValue[4];
                    if (copyOfStruct_4.tag === 1) {
                        return new FSharpResult$2(1, copyOfStruct_4.fields[0]);
                    }
                    else {
                        return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0]));
                    }
                }
            }
        }
    }
}

export function map6(ctor, d1, d2, d3, d4, d5, d6, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1), d4(path, value_1), d5(path, value_1), d6(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                const copyOfStruct_3 = matchValue[3];
                if (copyOfStruct_3.tag === 1) {
                    return new FSharpResult$2(1, copyOfStruct_3.fields[0]);
                }
                else {
                    const copyOfStruct_4 = matchValue[4];
                    if (copyOfStruct_4.tag === 1) {
                        return new FSharpResult$2(1, copyOfStruct_4.fields[0]);
                    }
                    else {
                        const copyOfStruct_5 = matchValue[5];
                        if (copyOfStruct_5.tag === 1) {
                            return new FSharpResult$2(1, copyOfStruct_5.fields[0]);
                        }
                        else {
                            return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0]));
                        }
                    }
                }
            }
        }
    }
}

export function map7(ctor, d1, d2, d3, d4, d5, d6, d7, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1), d4(path, value_1), d5(path, value_1), d6(path, value_1), d7(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                const copyOfStruct_3 = matchValue[3];
                if (copyOfStruct_3.tag === 1) {
                    return new FSharpResult$2(1, copyOfStruct_3.fields[0]);
                }
                else {
                    const copyOfStruct_4 = matchValue[4];
                    if (copyOfStruct_4.tag === 1) {
                        return new FSharpResult$2(1, copyOfStruct_4.fields[0]);
                    }
                    else {
                        const copyOfStruct_5 = matchValue[5];
                        if (copyOfStruct_5.tag === 1) {
                            return new FSharpResult$2(1, copyOfStruct_5.fields[0]);
                        }
                        else {
                            const copyOfStruct_6 = matchValue[6];
                            if (copyOfStruct_6.tag === 1) {
                                return new FSharpResult$2(1, copyOfStruct_6.fields[0]);
                            }
                            else {
                                return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0], copyOfStruct_6.fields[0]));
                            }
                        }
                    }
                }
            }
        }
    }
}

export function map8(ctor, d1, d2, d3, d4, d5, d6, d7, d8, path, value_1) {
    const matchValue = [d1(path, value_1), d2(path, value_1), d3(path, value_1), d4(path, value_1), d5(path, value_1), d6(path, value_1), d7(path, value_1), d8(path, value_1)];
    const copyOfStruct = matchValue[0];
    if (copyOfStruct.tag === 1) {
        return new FSharpResult$2(1, copyOfStruct.fields[0]);
    }
    else {
        const copyOfStruct_1 = matchValue[1];
        if (copyOfStruct_1.tag === 1) {
            return new FSharpResult$2(1, copyOfStruct_1.fields[0]);
        }
        else {
            const copyOfStruct_2 = matchValue[2];
            if (copyOfStruct_2.tag === 1) {
                return new FSharpResult$2(1, copyOfStruct_2.fields[0]);
            }
            else {
                const copyOfStruct_3 = matchValue[3];
                if (copyOfStruct_3.tag === 1) {
                    return new FSharpResult$2(1, copyOfStruct_3.fields[0]);
                }
                else {
                    const copyOfStruct_4 = matchValue[4];
                    if (copyOfStruct_4.tag === 1) {
                        return new FSharpResult$2(1, copyOfStruct_4.fields[0]);
                    }
                    else {
                        const copyOfStruct_5 = matchValue[5];
                        if (copyOfStruct_5.tag === 1) {
                            return new FSharpResult$2(1, copyOfStruct_5.fields[0]);
                        }
                        else {
                            const copyOfStruct_6 = matchValue[6];
                            if (copyOfStruct_6.tag === 1) {
                                return new FSharpResult$2(1, copyOfStruct_6.fields[0]);
                            }
                            else {
                                const copyOfStruct_7 = matchValue[7];
                                if (copyOfStruct_7.tag === 1) {
                                    return new FSharpResult$2(1, copyOfStruct_7.fields[0]);
                                }
                                else {
                                    return new FSharpResult$2(0, ctor(copyOfStruct.fields[0], copyOfStruct_1.fields[0], copyOfStruct_2.fields[0], copyOfStruct_3.fields[0], copyOfStruct_4.fields[0], copyOfStruct_5.fields[0], copyOfStruct_6.fields[0], copyOfStruct_7.fields[0]));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

export function dict(decoder) {
    let d1;
    const decoder_1 = decoder;
    d1 = ((path) => ((value_1) => keyValuePairs(decoder_1, path, value_1)));
    return (path_1) => ((value_2) => map(ofList, uncurry(2, d1), path_1, value_2));
}

function unwrapWith(errors, path, decoder, value_1) {
    let matchValue;
    const clo1 = partialApply(1, decoder, [path]);
    matchValue = clo1(value_1);
    if (matchValue.tag === 1) {
        void (errors.push(matchValue.fields[0]));
        return null;
    }
    else {
        return matchValue.fields[0];
    }
}

export class Getters$1 {
    constructor(path, v) {
        this.errors = [];
        const _this = this;
        this.required = {
            Field(fieldName, decoder) {
                return unwrapWith(_this.errors, path, (path_1, value_1) => field(fieldName, decoder, path_1, value_1), v);
            },
            At(fieldNames, decoder_2) {
                return unwrapWith(_this.errors, path, (firstPath, firstValue) => at(fieldNames, decoder_2, firstPath, firstValue), v);
            },
            Raw(decoder_4) {
                return unwrapWith(_this.errors, path, decoder_4, v);
            },
        };
        const _this_1 = this;
        this.optional = {
            Field(fieldName_1, decoder_5) {
                return unwrapWith(_this_1.errors, path, (path_2, value_2) => optional(fieldName_1, decoder_5, path_2, value_2), v);
            },
            At(fieldNames_1, decoder_7) {
                return unwrapWith(_this_1.errors, path, (firstPath_1, firstValue_1) => optionalAt(fieldNames_1, decoder_7, firstPath_1, firstValue_1), v);
            },
            Raw(decoder_9) {
                let matchValue;
                const clo1 = partialApply(1, decoder_9, [path]);
                matchValue = clo1(v);
                if (matchValue.tag === 1) {
                    const reason = matchValue.fields[0][1];
                    const error = matchValue.fields[0];
                    let pattern_matching_result, v_2;
                    switch (reason.tag) {
                        case 1: {
                            pattern_matching_result = 0;
                            v_2 = reason.fields[1];
                            break;
                        }
                        case 2: {
                            pattern_matching_result = 0;
                            v_2 = reason.fields[1];
                            break;
                        }
                        case 3:
                        case 4: {
                            pattern_matching_result = 1;
                            break;
                        }
                        case 5:
                        case 6:
                        case 7: {
                            pattern_matching_result = 2;
                            break;
                        }
                        default: {
                            pattern_matching_result = 0;
                            v_2 = reason.fields[1];
                        }
                    }
                    switch (pattern_matching_result) {
                        case 0: {
                            if (v_2 == null) {
                                return void 0;
                            }
                            else {
                                void (_this_1.errors.push(error));
                                return null;
                            }
                        }
                        case 1: {
                            return void 0;
                        }
                        case 2: {
                            void (_this_1.errors.push(error));
                            return null;
                        }
                    }
                }
                else {
                    return some(matchValue.fields[0]);
                }
            },
        };
    }
    get Required() {
        const __ = this;
        return __.required;
    }
    get Optional() {
        const __ = this;
        return __.optional;
    }
}

export function Getters$1$reflection(gen0) {
    return class_type("Thoth.Json.Decode.Getters`1", [gen0], Getters$1);
}

export function Getters$1_$ctor_4A51B60E(path, v) {
    return new Getters$1(path, v);
}

export function Getters$1__get_Errors(__) {
    return ofSeq(__.errors);
}

export function object(builder, path, v) {
    const getters = Getters$1_$ctor_4A51B60E(path, v);
    let result;
    result = builder(getters);
    const matchValue = Getters$1__get_Errors(getters);
    if (matchValue.tail != null) {
        const errors = matchValue;
        if (length(errors) > 1) {
            const errors_1 = map_1((tupledArg) => errorToString(tupledArg[0], tupledArg[1]), errors);
            return new FSharpResult$2(1, [path, new ErrorReason(7, errors_1)]);
        }
        else {
            return new FSharpResult$2(1, matchValue.head);
        }
    }
    else {
        return new FSharpResult$2(0, result);
    }
}

export function tuple2(decoder1, decoder2) {
    let decoder_3;
    const decoder = decoder1;
    decoder_3 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_3) => ((value_4) => andThen(uncurry(3, (v1) => {
        let decoder_2;
        const decoder_1 = decoder2;
        decoder_2 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_2) => ((value_3) => andThen((v2, arg10$0040, arg20$0040) => succeed([v1, v2], arg10$0040, arg20$0040), uncurry(2, decoder_2), path_2, value_3));
    }), uncurry(2, decoder_3), path_3, value_4));
}

export function tuple3(decoder1, decoder2, decoder3) {
    let decoder_5;
    const decoder = decoder1;
    decoder_5 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_5) => ((value_6) => andThen(uncurry(3, (v1) => {
        let decoder_4;
        const decoder_1 = decoder2;
        decoder_4 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_4) => ((value_5) => andThen(uncurry(3, (v2) => {
            let decoder_3;
            const decoder_2 = decoder3;
            decoder_3 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_3) => ((value_4) => andThen((v3, arg10$0040, arg20$0040) => succeed([v1, v2, v3], arg10$0040, arg20$0040), uncurry(2, decoder_3), path_3, value_4));
        }), uncurry(2, decoder_4), path_4, value_5));
    }), uncurry(2, decoder_5), path_5, value_6));
}

export function tuple4(decoder1, decoder2, decoder3, decoder4) {
    let decoder_7;
    const decoder = decoder1;
    decoder_7 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_7) => ((value_8) => andThen(uncurry(3, (v1) => {
        let decoder_6;
        const decoder_1 = decoder2;
        decoder_6 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_6) => ((value_7) => andThen(uncurry(3, (v2) => {
            let decoder_5;
            const decoder_2 = decoder3;
            decoder_5 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_5) => ((value_6) => andThen(uncurry(3, (v3) => {
                let decoder_4;
                const decoder_3 = decoder4;
                decoder_4 = ((path_3) => ((value_4) => index(3, decoder_3, path_3, value_4)));
                return (path_4) => ((value_5) => andThen((v4, arg10$0040, arg20$0040) => succeed([v1, v2, v3, v4], arg10$0040, arg20$0040), uncurry(2, decoder_4), path_4, value_5));
            }), uncurry(2, decoder_5), path_5, value_6));
        }), uncurry(2, decoder_6), path_6, value_7));
    }), uncurry(2, decoder_7), path_7, value_8));
}

export function tuple5(decoder1, decoder2, decoder3, decoder4, decoder5) {
    let decoder_9;
    const decoder = decoder1;
    decoder_9 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_9) => ((value_10) => andThen(uncurry(3, (v1) => {
        let decoder_8;
        const decoder_1 = decoder2;
        decoder_8 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_8) => ((value_9) => andThen(uncurry(3, (v2) => {
            let decoder_7;
            const decoder_2 = decoder3;
            decoder_7 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_7) => ((value_8) => andThen(uncurry(3, (v3) => {
                let decoder_6;
                const decoder_3 = decoder4;
                decoder_6 = ((path_3) => ((value_4) => index(3, decoder_3, path_3, value_4)));
                return (path_6) => ((value_7) => andThen(uncurry(3, (v4) => {
                    let decoder_5;
                    const decoder_4 = decoder5;
                    decoder_5 = ((path_4) => ((value_5) => index(4, decoder_4, path_4, value_5)));
                    return (path_5) => ((value_6) => andThen((v5, arg10$0040, arg20$0040) => succeed([v1, v2, v3, v4, v5], arg10$0040, arg20$0040), uncurry(2, decoder_5), path_5, value_6));
                }), uncurry(2, decoder_6), path_6, value_7));
            }), uncurry(2, decoder_7), path_7, value_8));
        }), uncurry(2, decoder_8), path_8, value_9));
    }), uncurry(2, decoder_9), path_9, value_10));
}

export function tuple6(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6) {
    let decoder_11;
    const decoder = decoder1;
    decoder_11 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_11) => ((value_12) => andThen(uncurry(3, (v1) => {
        let decoder_10;
        const decoder_1 = decoder2;
        decoder_10 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_10) => ((value_11) => andThen(uncurry(3, (v2) => {
            let decoder_9;
            const decoder_2 = decoder3;
            decoder_9 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_9) => ((value_10) => andThen(uncurry(3, (v3) => {
                let decoder_8;
                const decoder_3 = decoder4;
                decoder_8 = ((path_3) => ((value_4) => index(3, decoder_3, path_3, value_4)));
                return (path_8) => ((value_9) => andThen(uncurry(3, (v4) => {
                    let decoder_7;
                    const decoder_4 = decoder5;
                    decoder_7 = ((path_4) => ((value_5) => index(4, decoder_4, path_4, value_5)));
                    return (path_7) => ((value_8) => andThen(uncurry(3, (v5) => {
                        let decoder_6;
                        const decoder_5 = decoder6;
                        decoder_6 = ((path_5) => ((value_6) => index(5, decoder_5, path_5, value_6)));
                        return (path_6) => ((value_7) => andThen((v6, arg10$0040, arg20$0040) => succeed([v1, v2, v3, v4, v5, v6], arg10$0040, arg20$0040), uncurry(2, decoder_6), path_6, value_7));
                    }), uncurry(2, decoder_7), path_7, value_8));
                }), uncurry(2, decoder_8), path_8, value_9));
            }), uncurry(2, decoder_9), path_9, value_10));
        }), uncurry(2, decoder_10), path_10, value_11));
    }), uncurry(2, decoder_11), path_11, value_12));
}

export function tuple7(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6, decoder7) {
    let decoder_13;
    const decoder = decoder1;
    decoder_13 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_13) => ((value_14) => andThen(uncurry(3, (v1) => {
        let decoder_12;
        const decoder_1 = decoder2;
        decoder_12 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_12) => ((value_13) => andThen(uncurry(3, (v2) => {
            let decoder_11;
            const decoder_2 = decoder3;
            decoder_11 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_11) => ((value_12) => andThen(uncurry(3, (v3) => {
                let decoder_10;
                const decoder_3 = decoder4;
                decoder_10 = ((path_3) => ((value_4) => index(3, decoder_3, path_3, value_4)));
                return (path_10) => ((value_11) => andThen(uncurry(3, (v4) => {
                    let decoder_9;
                    const decoder_4 = decoder5;
                    decoder_9 = ((path_4) => ((value_5) => index(4, decoder_4, path_4, value_5)));
                    return (path_9) => ((value_10) => andThen(uncurry(3, (v5) => {
                        let decoder_8;
                        const decoder_5 = decoder6;
                        decoder_8 = ((path_5) => ((value_6) => index(5, decoder_5, path_5, value_6)));
                        return (path_8) => ((value_9) => andThen(uncurry(3, (v6) => {
                            let decoder_7;
                            const decoder_6 = decoder7;
                            decoder_7 = ((path_6) => ((value_7) => index(6, decoder_6, path_6, value_7)));
                            return (path_7) => ((value_8) => andThen((v7, arg10$0040, arg20$0040) => succeed([v1, v2, v3, v4, v5, v6, v7], arg10$0040, arg20$0040), uncurry(2, decoder_7), path_7, value_8));
                        }), uncurry(2, decoder_8), path_8, value_9));
                    }), uncurry(2, decoder_9), path_9, value_10));
                }), uncurry(2, decoder_10), path_10, value_11));
            }), uncurry(2, decoder_11), path_11, value_12));
        }), uncurry(2, decoder_12), path_12, value_13));
    }), uncurry(2, decoder_13), path_13, value_14));
}

export function tuple8(decoder1, decoder2, decoder3, decoder4, decoder5, decoder6, decoder7, decoder8) {
    let decoder_15;
    const decoder = decoder1;
    decoder_15 = ((path) => ((value_1) => index(0, decoder, path, value_1)));
    return (path_15) => ((value_16) => andThen(uncurry(3, (v1) => {
        let decoder_14;
        const decoder_1 = decoder2;
        decoder_14 = ((path_1) => ((value_2) => index(1, decoder_1, path_1, value_2)));
        return (path_14) => ((value_15) => andThen(uncurry(3, (v2) => {
            let decoder_13;
            const decoder_2 = decoder3;
            decoder_13 = ((path_2) => ((value_3) => index(2, decoder_2, path_2, value_3)));
            return (path_13) => ((value_14) => andThen(uncurry(3, (v3) => {
                let decoder_12;
                const decoder_3 = decoder4;
                decoder_12 = ((path_3) => ((value_4) => index(3, decoder_3, path_3, value_4)));
                return (path_12) => ((value_13) => andThen(uncurry(3, (v4) => {
                    let decoder_11;
                    const decoder_4 = decoder5;
                    decoder_11 = ((path_4) => ((value_5) => index(4, decoder_4, path_4, value_5)));
                    return (path_11) => ((value_12) => andThen(uncurry(3, (v5) => {
                        let decoder_10;
                        const decoder_5 = decoder6;
                        decoder_10 = ((path_5) => ((value_6) => index(5, decoder_5, path_5, value_6)));
                        return (path_10) => ((value_11) => andThen(uncurry(3, (v6) => {
                            let decoder_9;
                            const decoder_6 = decoder7;
                            decoder_9 = ((path_6) => ((value_7) => index(6, decoder_6, path_6, value_7)));
                            return (path_9) => ((value_10) => andThen(uncurry(3, (v7) => {
                                let decoder_8;
                                const decoder_7 = decoder8;
                                decoder_8 = ((path_7) => ((value_8) => index(7, decoder_7, path_7, value_8)));
                                return (path_8) => ((value_9) => andThen((v8, arg10$0040, arg20$0040) => succeed([v1, v2, v3, v4, v5, v6, v7, v8], arg10$0040, arg20$0040), uncurry(2, decoder_8), path_8, value_9));
                            }), uncurry(2, decoder_9), path_9, value_10));
                        }), uncurry(2, decoder_10), path_10, value_11));
                    }), uncurry(2, decoder_11), path_11, value_12));
                }), uncurry(2, decoder_12), path_12, value_13));
            }), uncurry(2, decoder_13), path_13, value_14));
        }), uncurry(2, decoder_14), path_14, value_15));
    }), uncurry(2, decoder_15), path_15, value_16));
}

function toMap(xs) {
    return ofSeq_1(xs);
}

function toSet(xs) {
    return ofSeq_2(xs, {
        Compare: compare,
    });
}

function autoObject(decoderInfos, path, value_1) {
    if (!(value_1 === null ? false : (Object.getPrototypeOf(value_1 || false) === Object.prototype))) {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "an object", value_1)]);
    }
    else {
        return foldBack(uncurry(2, (tupledArg) => {
            const name = tupledArg[0];
            return (acc) => {
                if (acc.tag === 0) {
                    const result_1 = tupledArg[1]((path + ".") + name)(value_1[name]);
                    return Result_Map((v) => cons(v, acc.fields[0]), result_1);
                }
                else {
                    return acc;
                }
            };
        }), decoderInfos, new FSharpResult$2(0, empty()));
    }
}

function autoObject2(keyDecoder, valueDecoder, path, value_1) {
    if (!(value_1 === null ? false : (Object.getPrototypeOf(value_1 || false) === Object.prototype))) {
        return new FSharpResult$2(1, [path, new ErrorReason(0, "an object", value_1)]);
    }
    else {
        const source = Object.keys(value_1);
        return fold_2((acc, name) => {
            if (acc.tag === 0) {
                let matchValue;
                const clo1 = partialApply(1, keyDecoder, [path]);
                matchValue = clo1(name);
                if (matchValue.tag === 0) {
                    const _arg1 = valueDecoder((path + ".") + name, value_1[name]);
                    if (_arg1.tag === 0) {
                        return new FSharpResult$2(0, cons([matchValue.fields[0], _arg1.fields[0]], acc.fields[0]));
                    }
                    else {
                        return new FSharpResult$2(1, _arg1.fields[0]);
                    }
                }
                else {
                    return new FSharpResult$2(1, matchValue.fields[0]);
                }
            }
            else {
                return acc;
            }
        }, new FSharpResult$2(0, empty()), source);
    }
}

function mixedArray(msg, decoders, path, values) {
    let arg0, arg10, arg30, clo1, clo2, clo3;
    if (decoders.length !== values.length) {
        const arg0_1 = [path, (arg0 = (arg10 = (decoders.length | 0), arg30 = (values.length | 0), (clo1 = toText(printf("Expected %i %s but got %i")), clo2 = clo1(arg10), clo3 = clo2(msg), clo3(arg30))), (new ErrorReason(6, arg0)))];
        return new FSharpResult$2(1, arg0_1);
    }
    else {
        return foldBack2(uncurry(3, mapCurriedArgs((value_1) => ((decoder) => ((acc) => {
            if (acc.tag === 0) {
                const result_1 = decoder(path, value_1);
                return Result_Map((v) => cons(v, acc.fields[0]), result_1);
            }
            else {
                return acc;
            }
        })), [0, [0, 2], 0])), values, decoders, new FSharpResult$2(0, empty()));
    }
}

function makeUnion(extra, caseStrategy, t, name, path, values) {
    let uci;
    const array_1 = getUnionCases(t, true);
    uci = tryFind((x) => (name_3(x) === name), array_1);
    if (uci != null) {
        const uci_1 = uci;
        if (values.length === 0) {
            const arg0_1 = makeUnion_1(uci_1, [], true);
            return new FSharpResult$2(0, arg0_1);
        }
        else {
            let decoders;
            const array_2 = getUnionCaseFields(uci_1);
            decoders = map_2((fi) => autoDecoder(extra, caseStrategy, false, fi[1]), array_2);
            const result = mixedArray("union fields", decoders, path, values);
            return Result_Map((values_1) => makeUnion_1(uci_1, Array.from(values_1), true), result);
        }
    }
    else {
        const arg0 = [path, new ErrorReason(6, (("Cannot find case " + name) + " in ") + fullName(t))];
        return new FSharpResult$2(1, arg0);
    }
}

function autoDecodeRecordsAndUnions(extra, caseStrategy, isOptional, t) {
    const decoderRef = new FSharpRef(null);
    let extra_1;
    const key = fullName(t);
    extra_1 = add(key, decoderRef, extra);
    let decoder;
    if (isRecord(t, true)) {
        let decoders;
        const array_1 = getRecordElements(t, true);
        decoders = map_2((fi) => {
            const name = Util_Casing_convert(caseStrategy, name_3(fi));
            return [name, autoDecoder(extra_1, caseStrategy, false, fi[1])];
        }, array_1);
        decoder = ((path) => ((value_1) => {
            const result = autoObject(decoders, path, value_1);
            return Result_Map((xs) => makeRecord(t, Array.from(xs), true), result);
        }));
    }
    else if (isUnion(t, true)) {
        decoder = ((path_1) => ((value_2) => {
            if ((typeof value_2) === "string") {
                return makeUnion(extra_1, caseStrategy, t, value_2, path_1, []);
            }
            else if (Array.isArray(value_2)) {
                const values = value_2;
                let name_2;
                const o_4 = values[0];
                name_2 = o_4;
                return makeUnion(extra_1, caseStrategy, t, name_2, path_1, values.slice(1, values.length));
            }
            else {
                return new FSharpResult$2(1, [path_1, new ErrorReason(0, "a string or array", value_2)]);
            }
        }));
    }
    else if (isOptional) {
        decoder = ((path_2) => ((value_3) => (new FSharpResult$2(1, [path_2, new ErrorReason(2, "an extra coder for " + fullName(t), value_3)]))));
    }
    else {
        let message;
        const arg10 = fullName(t);
        const clo1 = toText(printf("Cannot generate auto decoder for %s. Please pass an extra decoder."));
        message = clo1(arg10);
        throw (new Error(message));
    }
    decoderRef.contents = decoder;
    return decoder;
}

function autoDecoder(extra, caseStrategy, isOptional, t) {
    let clo1, decoder_15;
    const fullname = fullName(t);
    const matchValue = tryFind_1(fullname, extra);
    if (matchValue == null) {
        if (isArray(t)) {
            let decoder;
            const t_1 = getElementType(t);
            decoder = autoDecoder(extra, caseStrategy, false, t_1);
            return (path_1) => ((value_2) => array(uncurry(2, decoder), path_1, value_2));
        }
        else if (isEnum(t)) {
            const enumType = fullName(getEnumUnderlyingType(t));
            if (enumType === "System.SByte") {
                return (path_2) => ((value_4) => {
                    const t_2 = t;
                    const path_3 = path_2;
                    const value_5 = value_4;
                    const matchValue_1 = sbyte(path_3)(value_5);
                    if (matchValue_1.tag === 1) {
                        return new FSharpResult$2(1, matchValue_1.fields[0]);
                    }
                    else {
                        const enumValue = matchValue_1.fields[0] | 0;
                        let _arg1;
                        let source_1;
                        const source = getEnumValues(t_2);
                        source_1 = source;
                        _arg1 = contains(enumValue, source_1);
                        if (_arg1) {
                            const arg0 = parseEnum(t_2, (enumValue.toString()));
                            return new FSharpResult$2(0, arg0);
                        }
                        else {
                            const arg0_1 = [path_3, new ErrorReason(1, fullName(t_2), value_5, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_1);
                        }
                    }
                });
            }
            else if (enumType === "System.Byte") {
                return (path_4) => ((value_7) => {
                    const t_3 = t;
                    const path_5 = path_4;
                    const value_8 = value_7;
                    const matchValue_2 = byte(path_5)(value_8);
                    if (matchValue_2.tag === 1) {
                        return new FSharpResult$2(1, matchValue_2.fields[0]);
                    }
                    else {
                        const enumValue_1 = matchValue_2.fields[0];
                        let _arg1_1;
                        let source_3;
                        const source_2 = getEnumValues(t_3);
                        source_3 = source_2;
                        _arg1_1 = contains(enumValue_1, source_3);
                        if (_arg1_1) {
                            const arg0_2 = parseEnum(t_3, (enumValue_1.toString()));
                            return new FSharpResult$2(0, arg0_2);
                        }
                        else {
                            const arg0_3 = [path_5, new ErrorReason(1, fullName(t_3), value_8, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_3);
                        }
                    }
                });
            }
            else if (enumType === "System.Int16") {
                return (path_6) => ((value_10) => {
                    const t_4 = t;
                    const path_7 = path_6;
                    const value_11 = value_10;
                    const matchValue_3 = int16(path_7)(value_11);
                    if (matchValue_3.tag === 1) {
                        return new FSharpResult$2(1, matchValue_3.fields[0]);
                    }
                    else {
                        const enumValue_2 = matchValue_3.fields[0] | 0;
                        let _arg1_2;
                        let source_5;
                        const source_4 = getEnumValues(t_4);
                        source_5 = source_4;
                        _arg1_2 = contains(enumValue_2, source_5);
                        if (_arg1_2) {
                            const arg0_4 = parseEnum(t_4, (int16ToString(enumValue_2)));
                            return new FSharpResult$2(0, arg0_4);
                        }
                        else {
                            const arg0_5 = [path_7, new ErrorReason(1, fullName(t_4), value_11, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_5);
                        }
                    }
                });
            }
            else if (enumType === "System.UInt16") {
                return (path_8) => ((value_13) => {
                    const t_5 = t;
                    const path_9 = path_8;
                    const value_14 = value_13;
                    const matchValue_4 = uint16(path_9)(value_14);
                    if (matchValue_4.tag === 1) {
                        return new FSharpResult$2(1, matchValue_4.fields[0]);
                    }
                    else {
                        const enumValue_3 = matchValue_4.fields[0];
                        let _arg1_3;
                        let source_7;
                        const source_6 = getEnumValues(t_5);
                        source_7 = source_6;
                        _arg1_3 = contains(enumValue_3, source_7);
                        if (_arg1_3) {
                            const arg0_6 = parseEnum(t_5, (enumValue_3.toString()));
                            return new FSharpResult$2(0, arg0_6);
                        }
                        else {
                            const arg0_7 = [path_9, new ErrorReason(1, fullName(t_5), value_14, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_7);
                        }
                    }
                });
            }
            else if (enumType === "System.Int32") {
                return (path_10) => ((value_16) => {
                    const t_6 = t;
                    const path_11 = path_10;
                    const value_17 = value_16;
                    const matchValue_5 = int(path_11)(value_17);
                    if (matchValue_5.tag === 1) {
                        return new FSharpResult$2(1, matchValue_5.fields[0]);
                    }
                    else {
                        const enumValue_4 = matchValue_5.fields[0] | 0;
                        let _arg1_4;
                        let source_9;
                        const source_8 = getEnumValues(t_6);
                        source_9 = source_8;
                        _arg1_4 = contains(enumValue_4, source_9);
                        if (_arg1_4) {
                            const arg0_8 = parseEnum(t_6, (int32ToString(enumValue_4)));
                            return new FSharpResult$2(0, arg0_8);
                        }
                        else {
                            const arg0_9 = [path_11, new ErrorReason(1, fullName(t_6), value_17, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_9);
                        }
                    }
                });
            }
            else if (enumType === "System.UInt32") {
                return (path_12) => ((value_19) => {
                    const t_7 = t;
                    const path_13 = path_12;
                    const value_20 = value_19;
                    const matchValue_6 = uint32(path_13)(value_20);
                    if (matchValue_6.tag === 1) {
                        return new FSharpResult$2(1, matchValue_6.fields[0]);
                    }
                    else {
                        const enumValue_5 = matchValue_6.fields[0];
                        let _arg1_5;
                        let source_11;
                        const source_10 = getEnumValues(t_7);
                        source_11 = source_10;
                        _arg1_5 = contains(enumValue_5, source_11);
                        if (_arg1_5) {
                            const arg0_10 = parseEnum(t_7, (enumValue_5.toString()));
                            return new FSharpResult$2(0, arg0_10);
                        }
                        else {
                            const arg0_11 = [path_13, new ErrorReason(1, fullName(t_7), value_20, "Unkown value provided for the enum")];
                            return new FSharpResult$2(1, arg0_11);
                        }
                    }
                });
            }
            else {
                return (clo1 = toFail(printf("Cannot generate auto decoder for %s.\nThoth.Json.Net only support the folluwing enum types:\n- sbyte\n- byte\n- int16\n- uint16\n- int\n- uint32\nIf you can\u0027t use one of these types, please pass an extra decoder.\n                    ")), (arg10) => {
                    const clo2 = clo1(arg10);
                    return (arg20) => {
                        const clo3 = clo2(arg20);
                        return clo3;
                    };
                })(fullName(t));
            }
        }
        else if (isGenericType(t)) {
            if (isTuple(t)) {
                let decoders;
                const array_1 = getTupleElements(t);
                decoders = map_2((t_8) => autoDecoder(extra, caseStrategy, false, t_8), array_1);
                return (path_14) => ((value_21) => {
                    if (Array.isArray(value_21)) {
                        const result = mixedArray("tuple elements", decoders, path_14, value_21);
                        return Result_Map((xs) => makeTuple(Array.from(xs), t), result);
                    }
                    else {
                        return new FSharpResult$2(1, [path_14, new ErrorReason(0, "an array", value_21)]);
                    }
                });
            }
            else {
                const fullname_1 = fullName(getGenericTypeDefinition(t));
                if (fullname_1 === "Microsoft.FSharp.Core.FSharpOption`1[System.Object]") {
                    let d_14;
                    let decoder_13;
                    const t_9 = getGenerics(t)[0];
                    decoder_13 = autoDecoder(extra, caseStrategy, true, t_9);
                    d_14 = ((path_15) => ((value_22) => option(uncurry(2, decoder_13), path_15, value_22)));
                    return d_14;
                }
                else if (fullname_1 === "Microsoft.FSharp.Collections.FSharpList`1[System.Object]") {
                    let d_16;
                    let decoder_14;
                    const t_10 = getGenerics(t)[0];
                    decoder_14 = autoDecoder(extra, caseStrategy, false, t_10);
                    d_16 = ((path_16) => ((value_23) => list(uncurry(2, decoder_14), path_16, value_23)));
                    return d_16;
                }
                else if (fullname_1 === "Microsoft.FSharp.Collections.FSharpMap`2[System.Object,System.Object]") {
                    let keyDecoder;
                    const t_11 = getGenerics(t)[0];
                    keyDecoder = autoDecoder(extra, caseStrategy, false, t_11);
                    let valueDecoder;
                    const t_12 = getGenerics(t)[1];
                    valueDecoder = autoDecoder(extra, caseStrategy, false, t_12);
                    let d1;
                    const decoders_1 = ofArray([(path_17) => ((value_24) => autoObject2(uncurry(2, keyDecoder), uncurry(2, valueDecoder), path_17, value_24)), (decoder_15 = tuple2(uncurry(2, keyDecoder), uncurry(2, valueDecoder)), (path_18) => ((value_25) => list(uncurry(2, decoder_15), path_18, value_25)))]);
                    d1 = ((path_19) => ((value_26) => oneOf(decoders_1, path_19, value_26)));
                    return (path_20) => ((value_28) => map((ar) => {
                        const value_27 = toMap(ar);
                        return value_27;
                    }, uncurry(2, d1), path_20, value_28));
                }
                else if (fullname_1 === "Microsoft.FSharp.Collections.FSharpSet`1[System.Object]") {
                    let decoder_16;
                    const t_13 = getGenerics(t)[0];
                    decoder_16 = autoDecoder(extra, caseStrategy, false, t_13);
                    return (path_21) => ((value_29) => {
                        const matchValue_7 = array(uncurry(2, decoder_16), path_21, value_29);
                        if (matchValue_7.tag === 0) {
                            let arg0_13;
                            const value_30 = toSet(matchValue_7.fields[0]);
                            arg0_13 = value_30;
                            return new FSharpResult$2(0, arg0_13);
                        }
                        else {
                            return new FSharpResult$2(1, matchValue_7.fields[0]);
                        }
                    });
                }
                else {
                    return autoDecodeRecordsAndUnions(extra, caseStrategy, isOptional, t);
                }
            }
        }
        else if (fullname === "System.Boolean") {
            return (path_22) => ((value_31) => bool(path_22, value_31));
        }
        else if (fullname === "Microsoft.FSharp.Core.Unit") {
            return (path_23) => ((value_32) => unit(path_23, value_32));
        }
        else if (fullname === "System.String") {
            return (path_24) => ((value_33) => string(path_24, value_33));
        }
        else if (fullname === "System.SByte") {
            return sbyte;
        }
        else if (fullname === "System.Byte") {
            return byte;
        }
        else if (fullname === "System.Int16") {
            return int16;
        }
        else if (fullname === "System.UInt16") {
            return uint16;
        }
        else if (fullname === "System.Int32") {
            return int;
        }
        else if (fullname === "System.UInt32") {
            return uint32;
        }
        else if (fullname === "System.Double") {
            return (path_25) => ((value_34) => float(path_25, value_34));
        }
        else if (fullname === "System.Single") {
            return (path_26) => ((value_35) => float32(path_26, value_35));
        }
        else if (fullname === "System.DateTime") {
            return (path_27) => ((value_36) => datetime(path_27, value_36));
        }
        else if (fullname === "System.DateTimeOffset") {
            return (path_28) => ((value_37) => datetimeOffset(path_28, value_37));
        }
        else if (fullname === "System.TimeSpan") {
            return (path_29) => ((value_38) => timespan(path_29, value_38));
        }
        else if (fullname === "System.Guid") {
            return (path_30) => ((value_39) => guid(path_30, value_39));
        }
        else if (fullname === "System.Object") {
            return (_arg1_6) => ((v) => (new FSharpResult$2(0, v)));
        }
        else {
            return autoDecodeRecordsAndUnions(extra, caseStrategy, isOptional, t);
        }
    }
    else {
        const decoderRef = matchValue;
        return (path) => ((value_1) => decoderRef.contents(path)(value_1));
    }
}

function makeExtra(extra) {
    if (extra != null) {
        const e = extra;
        return map_3((_arg2, tupledArg) => (new FSharpRef(tupledArg[1])), e.Coders);
    }
    else {
        return empty_1();
    }
}

export class Auto {
    constructor() {
    }
}

export function Auto$reflection() {
    return class_type("Thoth.Json.Decode.Auto", void 0, Auto);
}

export function Auto_generateDecoderCached_7848D058(caseStrategy, extra, resolver) {
    const t = resolver.ResolveType();
    const caseStrategy_1 = defaultArg(caseStrategy, new CaseStrategy(0));
    let key;
    let y_1;
    const y = fullName(t);
    const x = toString_12(caseStrategy_1);
    y_1 = (x + y);
    let x_1;
    let option_2;
    option_2 = map_4((e) => e.Hash, extra);
    x_1 = defaultArg(option_2, "");
    key = (x_1 + y_1);
    const d = Util_Cache$1__GetOrAdd_43981464(Util_CachedDecoders, key, () => autoDecoder(makeExtra(extra), caseStrategy_1, false, t));
    return d;
}

export function Auto_generateDecoder_7848D058(caseStrategy, extra, resolver) {
    const caseStrategy_1 = defaultArg(caseStrategy, new CaseStrategy(0));
    let d;
    const t = resolver.ResolveType();
    const extra_1 = makeExtra(extra);
    d = autoDecoder(extra_1, caseStrategy_1, false, t);
    return d;
}

export function Auto_fromString_Z5CB6BD(json, caseStrategy, extra, resolver) {
    const decoder = Auto_generateDecoder_7848D058(caseStrategy, extra, resolver);
    return fromString(uncurry(2, decoder), json);
}

export function Auto_unsafeFromString_Z5CB6BD(json, caseStrategy, extra, resolver) {
    const decoder = Auto_generateDecoder_7848D058(caseStrategy, extra, resolver);
    const matchValue = fromString(uncurry(2, decoder), json);
    if (matchValue.tag === 1) {
        throw (new Error(matchValue.fields[0]));
    }
    else {
        return matchValue.fields[0];
    }
}

