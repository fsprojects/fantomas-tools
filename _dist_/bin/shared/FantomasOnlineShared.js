import { Record, toString, Union } from "../.fable/fable-library.3.0.1/Types.js";
import { record_type, list_type, union_type, bool_type, string_type, int32_type } from "../.fable/fable-library.3.0.1/Reflection.js";
import { int32ToString } from "../.fable/fable-library.3.0.1/Util.js";

export class FantomasOption extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["IntOption", "BoolOption", "MultilineFormatterTypeOption", "EndOfLineStyleOption"];
    }
}

export function FantomasOption$reflection() {
    return union_type("FantomasOnline.Shared.FantomasOption", [], FantomasOption, () => [[["order", int32_type], ["name", string_type], ["value", int32_type]], [["order", int32_type], ["name", string_type], ["value", bool_type]], [["order", int32_type], ["name", string_type], ["value", string_type]], [["order", int32_type], ["name", string_type], ["value", string_type]]]);
}

export function sortByOption(_arg1) {
    let pattern_matching_result, o;
    switch (_arg1.tag) {
        case 1: {
            pattern_matching_result = 0;
            o = _arg1.fields[0];
            break;
        }
        case 2: {
            pattern_matching_result = 0;
            o = _arg1.fields[0];
            break;
        }
        case 3: {
            pattern_matching_result = 0;
            o = _arg1.fields[0];
            break;
        }
        default: {
            pattern_matching_result = 0;
            o = _arg1.fields[0];
        }
    }
    switch (pattern_matching_result) {
        case 0: {
            return o | 0;
        }
    }
}

export function getOptionKey(_arg1) {
    let pattern_matching_result, k;
    switch (_arg1.tag) {
        case 1: {
            pattern_matching_result = 0;
            k = _arg1.fields[1];
            break;
        }
        case 2: {
            pattern_matching_result = 0;
            k = _arg1.fields[1];
            break;
        }
        case 3: {
            pattern_matching_result = 0;
            k = _arg1.fields[1];
            break;
        }
        default: {
            pattern_matching_result = 0;
            k = _arg1.fields[1];
        }
    }
    switch (pattern_matching_result) {
        case 0: {
            return k;
        }
    }
}

export function optionValue(_arg1) {
    let pattern_matching_result, v;
    switch (_arg1.tag) {
        case 1: {
            pattern_matching_result = 1;
            break;
        }
        case 2: {
            pattern_matching_result = 2;
            v = _arg1.fields[2];
            break;
        }
        case 3: {
            pattern_matching_result = 2;
            v = _arg1.fields[2];
            break;
        }
        default: pattern_matching_result = 0}
    switch (pattern_matching_result) {
        case 0: {
            return int32ToString(_arg1.fields[2]);
        }
        case 1: {
            return toString(_arg1.fields[2]);
        }
        case 2: {
            return v;
        }
    }
}

export class FormatRequest extends Record {
    constructor(SourceCode, Options, IsFsi) {
        super();
        this.SourceCode = SourceCode;
        this.Options = Options;
        this.IsFsi = IsFsi;
    }
}

export function FormatRequest$reflection() {
    return record_type("FantomasOnline.Shared.FormatRequest", [], FormatRequest, () => [["SourceCode", string_type], ["Options", list_type(FantomasOption$reflection())], ["IsFsi", bool_type]]);
}

