import { Record } from "../.fable/fable-library.3.1.1/Types.js";
import { bool_type, array_type, obj_type, option_type, string_type, record_type, int32_type } from "../.fable/fable-library.3.1.1/Reflection.js";

export class Range$ extends Record {
    constructor(StartLine, StartCol, EndLine, EndCol) {
        super();
        this.StartLine = (StartLine | 0);
        this.StartCol = (StartCol | 0);
        this.EndLine = (EndLine | 0);
        this.EndCol = (EndCol | 0);
    }
}

export function Range$$reflection() {
    return record_type("ASTViewer.Shared.Range", [], Range$, () => [["StartLine", int32_type], ["StartCol", int32_type], ["EndLine", int32_type], ["EndCol", int32_type]]);
}

export class Id extends Record {
    constructor(Ident, Range$) {
        super();
        this.Ident = Ident;
        this.Range = Range$;
    }
}

export function Id$reflection() {
    return record_type("ASTViewer.Shared.Id", [], Id, () => [["Ident", string_type], ["Range", option_type(Range$$reflection())]]);
}

export class Node$ extends Record {
    constructor(Type, Range$, Properties, Childs) {
        super();
        this.Type = Type;
        this.Range = Range$;
        this.Properties = Properties;
        this.Childs = Childs;
    }
}

export function Node$$reflection() {
    return record_type("ASTViewer.Shared.Node", [], Node$, () => [["Type", string_type], ["Range", option_type(Range$$reflection())], ["Properties", obj_type], ["Childs", array_type(Node$$reflection())]]);
}

export class Dto extends Record {
    constructor(Node$, String$) {
        super();
        this.Node = Node$;
        this.String = String$;
    }
}

export function Dto$reflection() {
    return record_type("ASTViewer.Shared.Dto", [], Dto, () => [["Node", Node$$reflection()], ["String", string_type]]);
}

export class Input extends Record {
    constructor(SourceCode, Defines, IsFsi) {
        super();
        this.SourceCode = SourceCode;
        this.Defines = Defines;
        this.IsFsi = IsFsi;
    }
}

export function Input$reflection() {
    return record_type("ASTViewer.Shared.Input", [], Input, () => [["SourceCode", string_type], ["Defines", array_type(string_type)], ["IsFsi", bool_type]]);
}

