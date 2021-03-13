import { array, fromString, int, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { uncurry } from "../.fable/fable-library.3.1.7/Util.js";
import { Model, Token, TokenInfo } from "./Model.js";

const decodeTokenInfo = (path_4) => ((v) => object((get$) => (new TokenInfo(get$.Required.Field("colorClass", (path, value) => string(path, value)), get$.Required.Field("charClass", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("fsharpTokenTriggerClass", (path_2, value_2) => string(path_2, value_2)), get$.Required.Field("tokenName", (path_3, value_3) => string(path_3, value_3)), get$.Required.Field("leftColumn", uncurry(2, int)), get$.Required.Field("rightColumn", uncurry(2, int)), get$.Required.Field("tag", uncurry(2, int)), get$.Required.Field("fullMatchedLength", uncurry(2, int)))), path_4, v));

const decodeToken = (path_1) => ((v) => object((get$) => (new Token(get$.Required.Field("tokenInfo", uncurry(2, decodeTokenInfo)), get$.Required.Field("lineNumber", uncurry(2, int)), get$.Required.Field("content", (path, value) => string(path, value)))), path_1, v));

export function decodeTokens(json) {
    return fromString((path, value) => array(uncurry(2, decodeToken), path, value), json);
}

export function decodeUrlModel(initialModel) {
    return (path_1) => ((v) => object((get$) => (new Model(get$.Required.Field("defines", (path, value) => string(path, value)), initialModel.Tokens, initialModel.ActiveLine, initialModel.ActiveTokenIndex, initialModel.IsLoading, initialModel.Version)), path_1, v));
}

