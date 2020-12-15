import { array, fromString, int, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { uncurry } from "../.fable/fable-library.3.0.1/Util.js";
import { Model, Token, TokenInfo } from "./Model.js";

const decodeTokenInfo = (path_4) => ((v) => object((get$) => (new TokenInfo(get$.Required.Field("colorClass", string), get$.Required.Field("charClass", string), get$.Required.Field("fsharpTokenTriggerClass", string), get$.Required.Field("tokenName", string), get$.Required.Field("leftColumn", uncurry(2, int)), get$.Required.Field("rightColumn", uncurry(2, int)), get$.Required.Field("tag", uncurry(2, int)), get$.Required.Field("fullMatchedLength", uncurry(2, int)))), path_4, v));

const decodeToken = (path_1) => ((v) => object((get$) => (new Token(get$.Required.Field("tokenInfo", uncurry(2, decodeTokenInfo)), get$.Required.Field("lineNumber", uncurry(2, int)), get$.Required.Field("content", string))), path_1, v));

export function decodeTokens(json) {
    return fromString((path, value) => array(uncurry(2, decodeToken), path, value), json);
}

export function decodeUrlModel(initialModel) {
    return (path_1) => ((v) => object((get$) => (new Model(get$.Required.Field("defines", string), initialModel.Tokens, initialModel.ActiveLine, initialModel.ActiveTokenIndex, initialModel.IsLoading, initialModel.Version)), path_1, v));
}

