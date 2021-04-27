import { Record, Union } from "./.fable/fable-library.3.1.15/Types.js";
import { Msg$reflection as Msg$reflection_4, Model$reflection as Model$reflection_4, FantomasMode$reflection } from "./FantomasOnline/Model.js";
import { record_type, bool_type, string_type, union_type } from "./.fable/fable-library.3.1.15/Reflection.js";
import { Msg$reflection as Msg$reflection_1, Model$reflection as Model$reflection_1 } from "./Trivia/Model.js";
import { Msg$reflection as Msg$reflection_2, Model$reflection as Model$reflection_2 } from "./FSharpTokens/Model.js";
import { Msg$reflection as Msg$reflection_3, Model$reflection as Model$reflection_3 } from "./ASTViewer/Model.js";

export class ActiveTab extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["HomeTab", "TokensTab", "ASTTab", "TriviaTab", "FantomasTab"];
    }
}

export function ActiveTab$reflection() {
    return union_type("FantomasTools.Client.Model.ActiveTab", [], ActiveTab, () => [[], [], [], [], [["Item", FantomasMode$reflection()]]]);
}

export class Model extends Record {
    constructor(ActiveTab, SourceCode, SettingsOpen, TriviaModel, FSharpTokensModel, ASTModel, FantomasModel) {
        super();
        this.ActiveTab = ActiveTab;
        this.SourceCode = SourceCode;
        this.SettingsOpen = SettingsOpen;
        this.TriviaModel = TriviaModel;
        this.FSharpTokensModel = FSharpTokensModel;
        this.ASTModel = ASTModel;
        this.FantomasModel = FantomasModel;
    }
}

export function Model$reflection() {
    return record_type("FantomasTools.Client.Model.Model", [], Model, () => [["ActiveTab", ActiveTab$reflection()], ["SourceCode", string_type], ["SettingsOpen", bool_type], ["TriviaModel", Model$reflection_1()], ["FSharpTokensModel", Model$reflection_2()], ["ASTModel", Model$reflection_3()], ["FantomasModel", Model$reflection_4()]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["SelectTab", "UpdateSourceCode", "TriviaMsg", "FSharpTokensMsg", "ASTMsg", "FantomasMsg", "ToggleSettings"];
    }
}

export function Msg$reflection() {
    return union_type("FantomasTools.Client.Model.Msg", [], Msg, () => [[["Item", ActiveTab$reflection()]], [["Item", string_type]], [["Item", Msg$reflection_1()]], [["Item", Msg$reflection_2()]], [["Item", Msg$reflection_3()]], [["Item", Msg$reflection_4()]], []]);
}

