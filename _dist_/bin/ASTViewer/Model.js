import { Record, Union } from "../.fable/fable-library.3.1.7/Types.js";
import { record_type, option_type, union_type, bool_type, string_type } from "../.fable/fable-library.3.1.7/Reflection.js";
import { Response$reflection } from "../shared/ASTViewerShared.js";
import { HighLightRange$reflection } from "../Editor.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.7/Choice.js";

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["VersionFound", "SetSourceText", "DoParse", "DoTypeCheck", "ASTParsed", "Error", "DefinesUpdated", "SetFsiFile", "HighLight"];
    }
}

export function Msg$reflection() {
    return union_type("FantomasTools.Client.ASTViewer.Model.Msg", [], Msg, () => [[["Item", string_type]], [["Item", string_type]], [], [], [["Item", Response$reflection()]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", HighLightRange$reflection()]]]);
}

export class EditorState extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Loading", "Loaded"];
    }
}

export function EditorState$reflection() {
    return union_type("FantomasTools.Client.ASTViewer.Model.EditorState", [], EditorState, () => [[], []]);
}

export class Model extends Record {
    constructor(Source, Defines, IsFsi, Parsed, IsLoading, Version, FSharpEditorState) {
        super();
        this.Source = Source;
        this.Defines = Defines;
        this.IsFsi = IsFsi;
        this.Parsed = Parsed;
        this.IsLoading = IsLoading;
        this.Version = Version;
        this.FSharpEditorState = FSharpEditorState;
    }
}

export function Model$reflection() {
    return record_type("FantomasTools.Client.ASTViewer.Model.Model", [], Model, () => [["Source", string_type], ["Defines", string_type], ["IsFsi", bool_type], ["Parsed", option_type(union_type("Microsoft.FSharp.Core.FSharpResult`2", [Response$reflection(), string_type], FSharpResult$2, () => [[["ResultValue", Response$reflection()]], [["ErrorValue", string_type]]]))], ["IsLoading", bool_type], ["Version", string_type], ["FSharpEditorState", EditorState$reflection()]]);
}

