import __SNOWPACK_ENV__ from '../../../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import { Types_RequestProperties, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { toConsole, split, printf, toText } from "../.fable/fable-library.3.0.1/String.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { encodeUrlModel, encodeGetTokensRequest } from "./Encoders.js";
import { ofArray } from "../.fable/fable-library.3.0.1/List.js";
import { getText } from "../Http.js";
import { Msg, Model } from "./Model.js";
import { string, list, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { GetTokensRequest } from "../shared/FSharpTokensShared.js";
import { scrollTo as scrollTo_1 } from "../../js/scrollTo.js";
import { restoreModelFromUrl, updateUrlWithData } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.0.1/Util.js";
import { decodeTokens, decodeUrlModel } from "./Decoders.js";
import { Cmd_OfPromise_perform, Cmd_ofSub, Cmd_batch, Cmd_OfFunc_result, Cmd_none, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { some, map, defaultArg } from "../.fable/fable-library.3.0.1/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function getTokens(request) {
    let arg10;
    const pr = fetch$((arg10 = (import.meta.env.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND), toText(printf("%s/%s"))(arg10)("api/get-tokens")), ofArray([new Types_RequestProperties(2, toString(4, encodeGetTokensRequest(request))), new Types_RequestProperties(0, "POST")]));
    return pr.then(((res) => res.text()));
}

function getVersion() {
    let arg10;
    return getText((arg10 = (import.meta.env.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND), toText(printf("%s/%s"))(arg10)("api/version")));
}

const initialModel = new Model("", [], void 0, void 0, false, "??");

const decodeGetTokensRequest = (path_3) => ((v) => object((get$) => (new GetTokensRequest(get$.Required.Field("defines", (path_1, value_1) => list(string, path_1, value_1)), get$.Required.Field("sourceCode", string))), path_3, v));

function splitDefines(value) {
    return ofArray(split(value, [" ", ";"], null, 1));
}

const scrollTo = scrollTo_1;

function modelToParseRequest(sourceCode, model) {
    return new GetTokensRequest(splitDefines(model.Defines), sourceCode);
}

function updateUrl(code, model, _arg1) {
    updateUrlWithData(toString(2, encodeUrlModel(code, model)));
}

export function init(isActive) {
    return [isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel, Cmd_OfPromise_either(getVersion, void 0, (arg0) => (new Msg(6, arg0)), (arg0_1) => (new Msg(7, arg0_1)))];
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const matchValue = decodeTokens(msg.fields[0]);
            if (matchValue.tag === 1) {
                toConsole(printf("%A"))(matchValue.fields[0]);
                return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), Cmd_none()];
            }
            else {
                const tokens = matchValue.fields[0];
                return [new Model(model.Defines, tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), (tokens.length === 1) ? Cmd_OfFunc_result(new Msg(2, 1)) : Cmd_none()];
            }
        }
        case 2: {
            return [new Model(model.Defines, model.Tokens, msg.fields[0], model.ActiveTokenIndex, model.IsLoading, model.Version), Cmd_none()];
        }
        case 3: {
            const tokenIndex = msg.fields[0] | 0;
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, tokenIndex, model.IsLoading, model.Version), Cmd_batch(ofArray([defaultArg(map((activeLine) => {
                const token = model.Tokens.filter((t) => (t.LineNumber === activeLine))[tokenIndex].TokenInfo;
                const range = new HighLightRange(activeLine, token.LeftColumn, activeLine, token.RightColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(range, arg10$0040);
                });
            }, model.ActiveLine), Cmd_none()), Cmd_OfFunc_result(new Msg(4, tokenIndex))]))];
        }
        case 4: {
            return [model, Cmd_ofSub((_arg1) => {
                scrollTo(msg.fields[0]);
            })];
        }
        case 5: {
            return [new Model(msg.fields[0], model.Tokens, model.ActiveLine, model.ActiveTokenIndex, model.IsLoading, model.Version), Cmd_none()];
        }
        case 6: {
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, model.IsLoading, msg.fields[0]), Cmd_none()];
        }
        case 7: {
            console.error(some(msg.fields[0]));
            return [model, Cmd_none()];
        }
        default: {
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, true, model.Version), Cmd_batch(ofArray([Cmd_OfPromise_perform(getTokens, modelToParseRequest(code, model), (arg0) => (new Msg(1, arg0))), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]))];
        }
    }
}

