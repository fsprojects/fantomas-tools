import { Record, Union } from "../.fable/fable-library.3.1.1/Types.js";
import { option_type, list_type, bool_type, record_type, int32_type, union_type, string_type } from "../.fable/fable-library.3.1.1/Reflection.js";

export class TriviaNodeType extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["MainNode", "Token"];
    }
}

export function TriviaNodeType$reflection() {
    return union_type("TriviaViewer.Shared.TriviaNodeType", [], TriviaNodeType, () => [[["Item", string_type]], [["Item", string_type]]]);
}

export class Range$ extends Record {
    constructor(StartLine, StartColumn, EndLine, EndColumn) {
        super();
        this.StartLine = (StartLine | 0);
        this.StartColumn = (StartColumn | 0);
        this.EndLine = (EndLine | 0);
        this.EndColumn = (EndColumn | 0);
    }
}

export function Range$$reflection() {
    return record_type("TriviaViewer.Shared.Range", [], Range$, () => [["StartLine", int32_type], ["StartColumn", int32_type], ["EndLine", int32_type], ["EndColumn", int32_type]]);
}

export class Comment$ extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["LineCommentAfterSourceCode", "LineCommentOnSingleLine", "BlockComment"];
    }
}

export function Comment$$reflection() {
    return union_type("TriviaViewer.Shared.Comment", [], Comment$, () => [[["comment", string_type]], [["comment", string_type]], [["Item1", string_type], ["newlineBefore", bool_type], ["newlineAfter", bool_type]]]);
}

export class TriviaContent extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Keyword", "Number", "StringContent", "IdentOperatorAsWord", "IdentBetweenTicks", "Comment", "Newline", "Directive", "NewlineAfter", "CharContent", "EmbeddedIL"];
    }
}

export function TriviaContent$reflection() {
    return union_type("TriviaViewer.Shared.TriviaContent", [], TriviaContent, () => [[["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", Comment$$reflection()]], [], [["directive", string_type]], [], [["Item", string_type]], [["Item", string_type]]]);
}

export class TriviaNode extends Record {
    constructor(Type, ContentBefore, ContentItself, ContentAfter, Range$) {
        super();
        this.Type = Type;
        this.ContentBefore = ContentBefore;
        this.ContentItself = ContentItself;
        this.ContentAfter = ContentAfter;
        this.Range = Range$;
    }
}

export function TriviaNode$reflection() {
    return record_type("TriviaViewer.Shared.TriviaNode", [], TriviaNode, () => [["Type", TriviaNodeType$reflection()], ["ContentBefore", list_type(TriviaContent$reflection())], ["ContentItself", option_type(TriviaContent$reflection())], ["ContentAfter", list_type(TriviaContent$reflection())], ["Range", Range$$reflection()]]);
}

export class TriviaNodeCandidate extends Record {
    constructor(Type, Name, Range$) {
        super();
        this.Type = Type;
        this.Name = Name;
        this.Range = Range$;
    }
}

export function TriviaNodeCandidate$reflection() {
    return record_type("TriviaViewer.Shared.TriviaNodeCandidate", [], TriviaNodeCandidate, () => [["Type", string_type], ["Name", string_type], ["Range", Range$$reflection()]]);
}

export class Trivia extends Record {
    constructor(Item, Range$) {
        super();
        this.Item = Item;
        this.Range = Range$;
    }
}

export function Trivia$reflection() {
    return record_type("TriviaViewer.Shared.Trivia", [], Trivia, () => [["Item", TriviaContent$reflection()], ["Range", Range$$reflection()]]);
}

export class ParseResult extends Record {
    constructor(Trivia, TriviaNodeCandidates, TriviaNodes) {
        super();
        this.Trivia = Trivia;
        this.TriviaNodeCandidates = TriviaNodeCandidates;
        this.TriviaNodes = TriviaNodes;
    }
}

export function ParseResult$reflection() {
    return record_type("TriviaViewer.Shared.ParseResult", [], ParseResult, () => [["Trivia", list_type(Trivia$reflection())], ["TriviaNodeCandidates", list_type(TriviaNodeCandidate$reflection())], ["TriviaNodes", list_type(TriviaNode$reflection())]]);
}

export class ParseRequest extends Record {
    constructor(SourceCode, Defines, FileName) {
        super();
        this.SourceCode = SourceCode;
        this.Defines = Defines;
        this.FileName = FileName;
    }
}

export function ParseRequest$reflection() {
    return record_type("TriviaViewer.Shared.ParseRequest", [], ParseRequest, () => [["SourceCode", string_type], ["Defines", list_type(string_type)], ["FileName", string_type]]);
}

