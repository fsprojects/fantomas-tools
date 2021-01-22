import { Record, Union } from "../.fable/fable-library.3.1.1/Types.js";
import { record_type, int32_type, bool_type, option_type, string_type, list_type, union_type } from "../.fable/fable-library.3.1.1/Reflection.js";
import { ParseResult$reflection, TriviaNode$reflection, TriviaNodeCandidate$reflection, Trivia$reflection } from "../shared/TriviaShared.js";

export class ActiveTab extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["ByTriviaNodes", "ByTrivia", "ByTriviaNodeCandidates"];
    }
}

export function ActiveTab$reflection() {
    return union_type("FantomasTools.Client.Trivia.Model.ActiveTab", [], ActiveTab, () => [[], [], []]);
}

export class Model extends Record {
    constructor(ActiveTab, Trivia, TriviaNodeCandidates, TriviaNodes, Error$, IsLoading, ActiveByTriviaNodeIndex, ActiveByTriviaIndex, Defines, Version, IsFsi) {
        super();
        this.ActiveTab = ActiveTab;
        this.Trivia = Trivia;
        this.TriviaNodeCandidates = TriviaNodeCandidates;
        this.TriviaNodes = TriviaNodes;
        this.Error = Error$;
        this.IsLoading = IsLoading;
        this.ActiveByTriviaNodeIndex = (ActiveByTriviaNodeIndex | 0);
        this.ActiveByTriviaIndex = (ActiveByTriviaIndex | 0);
        this.Defines = Defines;
        this.Version = Version;
        this.IsFsi = IsFsi;
    }
}

export function Model$reflection() {
    return record_type("FantomasTools.Client.Trivia.Model.Model", [], Model, () => [["ActiveTab", ActiveTab$reflection()], ["Trivia", list_type(Trivia$reflection())], ["TriviaNodeCandidates", list_type(TriviaNodeCandidate$reflection())], ["TriviaNodes", list_type(TriviaNode$reflection())], ["Error", option_type(string_type)], ["IsLoading", bool_type], ["ActiveByTriviaNodeIndex", int32_type], ["ActiveByTriviaIndex", int32_type], ["Defines", string_type], ["Version", string_type], ["IsFsi", bool_type]]);
}

export class UrlModel extends Record {
    constructor(IsFsi, Defines) {
        super();
        this.IsFsi = IsFsi;
        this.Defines = Defines;
    }
}

export function UrlModel$reflection() {
    return record_type("FantomasTools.Client.Trivia.Model.UrlModel", [], UrlModel, () => [["IsFsi", bool_type], ["Defines", string_type]]);
}

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["SelectTab", "GetTrivia", "TriviaReceived", "ActiveItemChange", "DefinesUpdated", "FSCVersionReceived", "SetFsiFile", "Error"];
    }
}

export function Msg$reflection() {
    return union_type("FantomasTools.Client.Trivia.Model.Msg", [], Msg, () => [[["Item", ActiveTab$reflection()]], [], [["Item", ParseResult$reflection()]], [["Item1", ActiveTab$reflection()], ["Item2", int32_type]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", string_type]]]);
}

