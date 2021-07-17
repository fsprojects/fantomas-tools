import { Record } from "../.fable/fable-library.3.2.9/Types.js";
import { record_type, list_type, string_type } from "../.fable/fable-library.3.2.9/Reflection.js";

export class GetTokensRequest extends Record {
    constructor(Defines, SourceCode) {
        super();
        this.Defines = Defines;
        this.SourceCode = SourceCode;
    }
}

export function GetTokensRequest$reflection() {
    return record_type("FSharpTokens.Shared.GetTokensRequest", [], GetTokensRequest, () => [["Defines", list_type(string_type)], ["SourceCode", string_type]]);
}

