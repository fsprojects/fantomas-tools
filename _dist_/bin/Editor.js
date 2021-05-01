import { Record, Union } from "./.fable/fable-library.3.1.15/Types.js";
import { record_type, int32_type, bool_type, union_type, delegate_type, obj_type, class_type, lambda_type, unit_type, string_type } from "./.fable/fable-library.3.1.15/Reflection.js";
import { useReact_useRef_1505, useReact_useEffect_Z5234A374 } from "./.fable/Feliz.1.43.0/React.fs.js";
import { ofArray, append } from "./.fable/fable-library.3.1.15/List.js";
import * as react from "../../_snowpack/pkg/react.js";
import react_1 from "../../_snowpack/pkg/@monaco-editor/react.js";
import { keyValueList } from "./.fable/fable-library.3.1.15/MapUtil.js";

export class MonacoEditorProp extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Height", "DefaultLanguage", "DefaultValue", "OnChange", "OnMount", "Options"];
    }
}

export function MonacoEditorProp$reflection() {
    return union_type("FantomasTools.Client.Editor.MonacoEditorProp", [], MonacoEditorProp, () => [[["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", lambda_type(string_type, unit_type)]], [["Item", delegate_type(class_type("FantomasTools.Client.Editor.IMonacoEditor"), obj_type, unit_type)]], [["Item", obj_type]]]);
}

function useEventListener(target, type, listener) {
    useReact_useEffect_Z5234A374(() => {
        target.addEventListener(type, listener);
        return {
            Dispose() {
                target.removeEventListener(type, listener);
            },
        };
    }, [target, type, listener]);
}

export function Editor(editorInputProps) {
    const props = editorInputProps.props;
    const isReadOnly = editorInputProps.isReadOnly;
    const editorRef = useReact_useRef_1505(null);
    useEventListener(window, "select_range", (ev) => {
        const ev_1 = ev;
        if (ev_1 && ev_1.detail && editorRef.current) {
            const range = ev_1.detail;
            const editor = editorRef.current;
            editor.setSelection(range);
            editor.revealRangeInCenter(range, 0);
        }
    });
    const props_1 = append(ofArray([new MonacoEditorProp(0, "100%"), new MonacoEditorProp(1, "fsharp"), new MonacoEditorProp(5, {
        readOnly: isReadOnly,
        selectOnLineNumbers: true,
        lineNumbers: true,
        theme: "vs-light",
        renderWhitespace: "all",
        minimap: {
            enabled: false,
        },
    }), new MonacoEditorProp(4, (editor_1, _arg1) => {
        editorRef.current = editor_1;
    })]), props);
    return react.createElement(react_1, keyValueList(props_1, 1));
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
    void window.dispatchEvent(new CustomEvent("select_range", data));
}

