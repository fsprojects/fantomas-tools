import { Record, Union } from "../.fable/fable-library.3.1.7/Types.js";
import { record_type, class_type, bool_type, tuple_type, list_type, string_type, union_type } from "../.fable/fable-library.3.1.7/Reflection.js";
import { sortByOption, FormatResponse$reflection, FantomasOption$reflection } from "../shared/FantomasOnlineShared.js";
import { zip, filter, map, sortBy } from "../.fable/fable-library.3.1.7/List.js";
import { equals, comparePrimitives } from "../.fable/fable-library.3.1.7/Util.js";
import { toList } from "../.fable/fable-library.3.1.7/Map.js";

export class FantomasMode extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["V2", "V3", "V4", "Preview"];
    }
}

export function FantomasMode$reflection() {
    return union_type("FantomasTools.Client.FantomasOnline.Model.FantomasMode", [], FantomasMode, () => [[], [], [], []]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["VersionReceived", "OptionsReceived", "FormatException", "Format", "FormattedReceived", "UpdateOption", "ChangeMode", "SetFsiFile", "CopySettings"];
    }
}

export function Msg$reflection() {
    return union_type("FantomasTools.Client.FantomasOnline.Model.Msg", [], Msg, () => [[["Item", string_type]], [["Item", list_type(FantomasOption$reflection())]], [["Item", string_type]], [], [["Item", FormatResponse$reflection()]], [["Item", tuple_type(string_type, FantomasOption$reflection())]], [["Item", FantomasMode$reflection()]], [["Item", bool_type]], []]);
}

export class EditorState extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["LoadingOptions", "OptionsLoaded", "LoadingFormatRequest", "FormatResult", "FormatError"];
    }
}

export function EditorState$reflection() {
    return union_type("FantomasTools.Client.FantomasOnline.Model.EditorState", [], EditorState, () => [[], [], [], [["Item", FormatResponse$reflection()]], [["Item", string_type]]]);
}

export class Model extends Record {
    constructor(IsFsi, Version, DefaultOptions, UserOptions, Mode, State) {
        super();
        this.IsFsi = IsFsi;
        this.Version = Version;
        this.DefaultOptions = DefaultOptions;
        this.UserOptions = UserOptions;
        this.Mode = Mode;
        this.State = State;
    }
}

export function Model$reflection() {
    return record_type("FantomasTools.Client.FantomasOnline.Model.Model", [], Model, () => [["IsFsi", bool_type], ["Version", string_type], ["DefaultOptions", list_type(FantomasOption$reflection())], ["UserOptions", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, FantomasOption$reflection()])], ["Mode", FantomasMode$reflection()], ["State", EditorState$reflection()]]);
}

export function Model__get_SettingsChangedByTheUser(this$) {
    const defaultValues = sortBy((_arg1) => sortByOption(_arg1), this$.DefaultOptions, {
        Compare: (x, y) => comparePrimitives(x, y),
    });
    const userValues = sortBy((_arg1_1) => sortByOption(_arg1_1), map((tuple) => tuple[1], toList(this$.UserOptions)), {
        Compare: (x_1, y_1) => comparePrimitives(x_1, y_1),
    });
    return map((tuple_1) => tuple_1[1], filter((tupledArg) => {
        const dv = tupledArg[0];
        const uv = tupledArg[1];
        return !equals(dv, uv);
    }, zip(defaultValues, userValues)));
}

