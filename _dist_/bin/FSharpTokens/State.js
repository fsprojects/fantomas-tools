import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { toConsole, split, printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { encodeUrlModel, encodeGetTokensRequest } from "./Encoders.js";
import { Types_RequestProperties, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { ofArray } from "../.fable/fable-library.3.1.7/List.js";
import { getText } from "../Http.js";
import { Msg, Model } from "./Model.js";
import { string, list, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { GetTokensRequest } from "../shared/FSharpTokensShared.js";
import { scrollTo as scrollTo_1 } from "../../js/scrollTo.js";
import { restoreModelFromUrl, updateUrlWithData } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.1.7/Util.js";
import { decodeTokens, decodeUrlModel } from "./Decoders.js";
import { Cmd_OfPromise_perform, Cmd_batch, Cmd_ofSub, Cmd_OfFunc_result, Cmd_none, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { some, map, defaultArg } from "../.fable/fable-library.3.1.7/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function getTokens(request) {
    let url;
    const arg10 = __SNOWPACK_ENV__.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND;
    url = toText(printf("%s/%s"))(arg10)("api/get-tokens");
    const json = toString(4, encodeGetTokensRequest(request));
    const pr = fetch$(url, ofArray([new Types_RequestProperties(2, json), new Types_RequestProperties(0, "POST")]));
    return pr.then(((res) => res.text()));
}

function getVersion() {
    let arg10;
    return getText((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND), toText(printf("%s/%s"))(arg10)("api/version")));
}

const initialModel = new Model("", [], void 0, void 0, false, "??");

const decodeGetTokensRequest = (path_3) => ((v) => object((get$) => (new GetTokensRequest(get$.Required.Field("defines", (path_1, value_1) => list((path, value) => string(path, value), path_1, value_1)), get$.Required.Field("sourceCode", (path_2, value_2) => string(path_2, value_2)))), path_3, v));

function splitDefines(value) {
    return ofArray(split(value, [" ", ";"], null, 1));
}

const scrollTo = scrollTo_1;

function modelToParseRequest(sourceCode, model) {
    const defines = splitDefines(model.Defines);
    return new GetTokensRequest(defines, sourceCode);
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function init(isActive) {
    const model = isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel;
    const cmd = Cmd_OfPromise_either(getVersion, void 0, (arg0) => (new Msg(6, arg0)), (arg0_1) => (new Msg(7, arg0_1)));
    return [model, cmd];
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const tokensText = msg.fields[0];
            const matchValue = decodeTokens(tokensText);
            if (matchValue.tag === 1) {
                const error = matchValue.fields[0];
                toConsole(printf("%A"))(error);
                return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), Cmd_none()];
            }
            else {
                const tokens = matchValue.fields[0];
                const cmd_1 = (tokens.length === 1) ? Cmd_OfFunc_result(new Msg(2, 1)) : Cmd_none();
                return [new Model(model.Defines, tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), cmd_1];
            }
        }
        case 2: {
            const lineNumber = msg.fields[0] | 0;
            return [new Model(model.Defines, model.Tokens, lineNumber, model.ActiveTokenIndex, model.IsLoading, model.Version), Cmd_none()];
        }
        case 3: {
            const tokenIndex = msg.fields[0] | 0;
            const highlightCmd = defaultArg(map((activeLine) => {
                const token = model.Tokens.filter((t) => (t.LineNumber === activeLine))[tokenIndex].TokenInfo;
                const range = new HighLightRange(activeLine, token.LeftColumn, activeLine, token.RightColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(range, arg10$0040);
                });
            }, model.ActiveLine), Cmd_none());
            const scrollCmd = Cmd_OfFunc_result(new Msg(4, tokenIndex));
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, tokenIndex, model.IsLoading, model.Version), Cmd_batch(ofArray([highlightCmd, scrollCmd]))];
        }
        case 4: {
            const index = msg.fields[0] | 0;
            return [model, Cmd_ofSub((_arg1) => {
                scrollTo(index);
            })];
        }
        case 5: {
            const defines = msg.fields[0];
            return [new Model(defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, model.IsLoading, model.Version), Cmd_none()];
        }
        case 6: {
            const v = msg.fields[0];
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, model.IsLoading, v), Cmd_none()];
        }
        case 7: {
            const e = msg.fields[0];
            console.error(some(e));
            return [model, Cmd_none()];
        }
        default: {
            let cmd;
            const requestCmd = Cmd_OfPromise_perform((request) => getTokens(request), modelToParseRequest(code, model), (arg0) => (new Msg(1, arg0)));
            const updateUrlCmd = Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            });
            cmd = Cmd_batch(ofArray([requestCmd, updateUrlCmd]));
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, true, model.Version), cmd];
        }
    }
}

