import { record_type, int32_type, union_type, obj_type, bool_type, lambda_type, unit_type, string_type, class_type } from "./.fable/fable-library.3.0.1/Reflection.js";
import { Record, Union } from "./.fable/fable-library.3.0.1/Types.js";

export class Editor {
    constructor() {
    }
}

export function Editor$reflection() {
    return class_type("FantomasTools.Client.Editor.Editor", void 0, Editor);
}

export class EditorProp extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["OnChange", "Value", "Language", "IsReadOnly", "GetEditor"];
    }
}

export function EditorProp$reflection() {
    return union_type("FantomasTools.Client.Editor.EditorProp", [], EditorProp, () => [[["Item", lambda_type(string_type, unit_type)]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", lambda_type(obj_type, unit_type)]]]);
}

export class HighLightRange extends Record {
    constructor(StartLine, StartColumn, EndLine, EndColumn) {
        super();
        this.StartLine = (StartLine | 0);
        this.StartColumn = (StartColumn | 0);
        this.EndLine = (EndLine | 0);
        this.EndColumn = (EndColumn | 0);
    }
}

export function HighLightRange$reflection() {
    return record_type("FantomasTools.Client.Editor.HighLightRange", [], HighLightRange, () => [["StartLine", int32_type], ["StartColumn", int32_type], ["EndLine", int32_type], ["EndColumn", int32_type]]);
}

export function selectRange(range, _arg1) {
    const data = {
        detail: {
            endColumn: range.EndColumn + 1,
            endLineNumber: range.EndLine,
            startColumn: range.StartColumn + 1,
            startLineNumber: range.StartLine,
        },
    };
    const value = window.dispatchEvent(new CustomEvent("select_range", data));
    void value;
}

