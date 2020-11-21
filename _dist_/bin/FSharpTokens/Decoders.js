import { array, fromString, int, string, object } from "../bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Model, Token, TokenInfo } from "./Model.js";

const decodeTokenInfo = (path_4) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4, objectArg_5, objectArg_6, objectArg_7;
    return new TokenInfo((objectArg = get$.Required, objectArg.Field("colorClass", string)), (objectArg_1 = get$.Required, objectArg_1.Field("charClass", string)), (objectArg_2 = get$.Required, objectArg_2.Field("fsharpTokenTriggerClass", string)), (objectArg_3 = get$.Required, objectArg_3.Field("tokenName", string)), (objectArg_4 = get$.Required, objectArg_4.Field("leftColumn", uncurry(2, int))), (objectArg_5 = get$.Required, objectArg_5.Field("rightColumn", uncurry(2, int))), (objectArg_6 = get$.Required, objectArg_6.Field("tag", uncurry(2, int))), (objectArg_7 = get$.Required, objectArg_7.Field("fullMatchedLength", uncurry(2, int))));
}, path_4, v));

const decodeToken = (path_1) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2;
    return new Token((objectArg = get$.Required, objectArg.Field("tokenInfo", uncurry(2, decodeTokenInfo))), (objectArg_1 = get$.Required, objectArg_1.Field("lineNumber", uncurry(2, int))), (objectArg_2 = get$.Required, objectArg_2.Field("content", string)));
}, path_1, v));

export function decodeTokens(json) {
    return fromString((path, value) => array(uncurry(2, decodeToken), path, value), json);
}

export function decodeUrlModel(initialModel) {
    return (path_1) => ((v) => object((get$) => {
        let objectArg;
        return new Model((objectArg = get$.Required, objectArg.Field("defines", string)), initialModel.Tokens, initialModel.ActiveLine, initialModel.ActiveTokenIndex, initialModel.IsLoading, initialModel.Version);
    }, path_1, v));
}

