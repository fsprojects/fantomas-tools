import { Record, Union } from "../.fable/fable-library.3.1.15/Types.js";
import { bool_type, option_type, array_type, record_type, union_type, class_type, int32_type, string_type } from "../.fable/fable-library.3.1.15/Reflection.js";

export class Msg extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["GetTokens", "TokenReceived", "LineSelected", "TokenSelected", "PlayScroll", "DefinesUpdated", "VersionFound", "NetworkException"];
    }
}

export function Msg$reflection() {
    return union_type("FantomasTools.Client.FSharpTokens.Model.Msg", [], Msg, () => [[], [["Item", string_type]], [["Item", int32_type]], [["Item", int32_type]], [["Item", int32_type]], [["Item", string_type]], [["Item", string_type]], [["Item", class_type("System.Exception")]]]);
}

export class TokenInfo extends Record {
    constructor(ColorClass, CharClass, FSharpTokenTriggerClass, TokenName, LeftColumn, RightColumn, Tag, FullMatchedLength) {
        super();
        this.ColorClass = ColorClass;
        this.CharClass = CharClass;
        this.FSharpTokenTriggerClass = FSharpTokenTriggerClass;
        this.TokenName = TokenName;
        this.LeftColumn = (LeftColumn | 0);
        this.RightColumn = (RightColumn | 0);
        this.Tag = (Tag | 0);
        this.FullMatchedLength = (FullMatchedLength | 0);
    }
}

export function TokenInfo$reflection() {
    return record_type("FantomasTools.Client.FSharpTokens.Model.TokenInfo", [], TokenInfo, () => [["ColorClass", string_type], ["CharClass", string_type], ["FSharpTokenTriggerClass", string_type], ["TokenName", string_type], ["LeftColumn", int32_type], ["RightColumn", int32_type], ["Tag", int32_type], ["FullMatchedLength", int32_type]]);
}

export class Token extends Record {
    constructor(TokenInfo, LineNumber, Content) {
        super();
        this.TokenInfo = TokenInfo;
        this.LineNumber = (LineNumber | 0);
        this.Content = Content;
    }
}

export function Token$reflection() {
    return record_type("FantomasTools.Client.FSharpTokens.Model.Token", [], Token, () => [["TokenInfo", TokenInfo$reflection()], ["LineNumber", int32_type], ["Content", string_type]]);
}

export class Model extends Record {
    constructor(Defines, Tokens, ActiveLine, ActiveTokenIndex, IsLoading, Version) {
        super();
        this.Defines = Defines;
        this.Tokens = Tokens;
        this.ActiveLine = ActiveLine;
        this.ActiveTokenIndex = ActiveTokenIndex;
        this.IsLoading = IsLoading;
        this.Version = Version;
    }
}

export function Model$reflection() {
    return record_type("FantomasTools.Client.FSharpTokens.Model.Model", [], Model, () => [["Defines", string_type], ["Tokens", array_type(Token$reflection())], ["ActiveLine", option_type(int32_type)], ["ActiveTokenIndex", option_type(int32_type)], ["IsLoading", bool_type], ["Version", string_type]]);
}

