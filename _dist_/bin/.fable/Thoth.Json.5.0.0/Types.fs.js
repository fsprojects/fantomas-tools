import { FSharpRef, Record, Union } from "../fable-library.3.1.7/Types.js";
import { record_type, class_type, tuple_type, lambda_type, union_type, list_type, obj_type, string_type } from "../fable-library.3.1.7/Reflection.js";
import { FSharpResult$2 } from "../fable-library.3.1.7/Choice.js";
import { addToDict, tryGetValue } from "../fable-library.3.1.7/MapUtil.js";
import { replace } from "../fable-library.3.1.7/RegExp.js";

export class ErrorReason extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["BadPrimitive", "BadPrimitiveExtra", "BadType", "BadField", "BadPath", "TooSmallArray", "FailMessage", "BadOneOf"];
    }
}

export function ErrorReason$reflection() {
    return union_type("Thoth.Json.ErrorReason", [], ErrorReason, () => [[["Item1", string_type], ["Item2", obj_type]], [["Item1", string_type], ["Item2", obj_type], ["Item3", string_type]], [["Item1", string_type], ["Item2", obj_type]], [["Item1", string_type], ["Item2", obj_type]], [["Item1", string_type], ["Item2", obj_type], ["Item3", string_type]], [["Item1", string_type], ["Item2", obj_type]], [["Item", string_type]], [["Item", list_type(string_type)]]]);
}

export class CaseStrategy extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["PascalCase", "CamelCase", "SnakeCase"];
    }
}

export function CaseStrategy$reflection() {
    return union_type("Thoth.Json.CaseStrategy", [], CaseStrategy, () => [[], [], []]);
}

export class ExtraCoders extends Record {
    constructor(Hash, Coders) {
        super();
        this.Hash = Hash;
        this.Coders = Coders;
    }
}

export function ExtraCoders$reflection() {
    return record_type("Thoth.Json.ExtraCoders", [], ExtraCoders, () => [["Hash", string_type], ["Coders", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, tuple_type(lambda_type(obj_type, obj_type), lambda_type(string_type, lambda_type(obj_type, union_type("Microsoft.FSharp.Core.FSharpResult`2", [obj_type, tuple_type(string_type, ErrorReason$reflection())], FSharpResult$2, () => [[["ResultValue", obj_type]], [["ErrorValue", tuple_type(string_type, ErrorReason$reflection())]]]))))])]]);
}

export class Util_Cache$1 {
    constructor() {
        this.cache = (new Map([]));
    }
}

export function Util_Cache$1$reflection(gen0) {
    return class_type("Thoth.Json.Util.Cache`1", [gen0], Util_Cache$1);
}

export function Util_Cache$1_$ctor() {
    return new Util_Cache$1();
}

export function Util_Cache$1__GetOrAdd_43981464(__, key, factory) {
    let matchValue;
    let outArg = null;
    matchValue = [tryGetValue(__.cache, key, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        const x = matchValue[1];
        return x;
    }
    else {
        const x_1 = factory();
        addToDict(__.cache, key, x_1);
        return x_1;
    }
}

export const Util_CachedEncoders = Util_Cache$1_$ctor();

export const Util_CachedDecoders = Util_Cache$1_$ctor();

export function Util_Casing_lowerFirst(str) {
    return str.slice(void 0, 0 + 1).toLowerCase() + str.slice(1, str.length);
}

export function Util_Casing_convert(caseStrategy, fieldName) {
    switch (caseStrategy.tag) {
        case 2: {
            return replace(Util_Casing_lowerFirst(fieldName), "[A-Z]", "_$0").toLowerCase();
        }
        case 0: {
            return fieldName;
        }
        default: {
            return Util_Casing_lowerFirst(fieldName);
        }
    }
}

