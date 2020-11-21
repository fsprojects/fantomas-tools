import __SNOWPACK_ENV__ from '../../../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import { toConsole, split, printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { toString } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { encodeUrlModel, encodeGetTokensRequest } from "./Encoders.js";
import { Types_RequestProperties, fetch$ } from "../bin/.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { ofArray } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { getText } from "../Http.js";
import { Msg, Model } from "./Model.js";
import { string, list, object } from "../bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { GetTokensRequest } from "../shared/FSharpTokensShared.js";
import { scrollTo as scrollTo_1 } from "../../js/scrollTo.js";
import { restoreModelFromUrl, updateUrlWithData } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { decodeTokens, decodeUrlModel } from "./Decoders.js";
import { Cmd_OfPromise_perform, Cmd_batch, Cmd_ofSub, Cmd_OfFunc_result, Cmd_none, Cmd_OfPromise_either } from "../bin/.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { some, defaultArg, map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function getTokens(request) {
    let url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND;
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    url = clo2("api/get-tokens");
    const json = toString(4, encodeGetTokensRequest(request));
    const pr = fetch$(url, ofArray([new Types_RequestProperties(2, json), new Types_RequestProperties(0, "POST")]));
    return pr.then(((res) => res.text()));
}

function getVersion() {
    let _url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_FSHARP_TOKENS_BACKEND;
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    _url = clo2("api/version");
    return getText(_url);
}

const initialModel = new Model("", [], void 0, void 0, false, "??");

const decodeGetTokensRequest = (path_3) => ((v) => object((get$) => {
    let objectArg, objectArg_1;
    return new GetTokensRequest((objectArg = get$.Required, objectArg.Field("defines", (path_1, value_1) => list(string, path_1, value_1))), (objectArg_1 = get$.Required, objectArg_1.Field("sourceCode", string)));
}, path_3, v));

function splitDefines(value) {
    const array = split(value, [" ", ";"], null, 1);
    return ofArray(array);
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
            const matchValue = decodeTokens(msg.fields[0]);
            if (matchValue.tag === 1) {
                const clo1 = toConsole(printf("%A"));
                clo1(matchValue.fields[0]);
                return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), Cmd_none()];
            }
            else {
                const tokens = matchValue.fields[0];
                const cmd_1 = (tokens.length === 1) ? Cmd_OfFunc_result(new Msg(2, 1)) : Cmd_none();
                return [new Model(model.Defines, tokens, model.ActiveLine, model.ActiveTokenIndex, false, model.Version), cmd_1];
            }
        }
        case 2: {
            return [new Model(model.Defines, model.Tokens, msg.fields[0], model.ActiveTokenIndex, model.IsLoading, model.Version), Cmd_none()];
        }
        case 3: {
            const tokenIndex = msg.fields[0] | 0;
            let highlightCmd;
            let option_1;
            option_1 = map((activeLine) => {
                let token;
                let t_1;
                let array_1;
                array_1 = model.Tokens.filter((t) => (t.LineNumber === activeLine));
                t_1 = array_1[tokenIndex];
                token = t_1.TokenInfo;
                const range = new HighLightRange(activeLine, token.LeftColumn, activeLine, token.RightColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(range, arg10$0040);
                });
            }, model.ActiveLine);
            const value = Cmd_none();
            highlightCmd = defaultArg(option_1, value);
            const scrollCmd = Cmd_OfFunc_result(new Msg(4, tokenIndex));
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, tokenIndex, model.IsLoading, model.Version), Cmd_batch(ofArray([highlightCmd, scrollCmd]))];
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
            let cmd;
            const requestCmd = Cmd_OfPromise_perform(getTokens, modelToParseRequest(code, model), (arg0) => (new Msg(1, arg0)));
            const updateUrlCmd = Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            });
            cmd = Cmd_batch(ofArray([requestCmd, updateUrlCmd]));
            return [new Model(model.Defines, model.Tokens, model.ActiveLine, model.ActiveTokenIndex, true, model.Version), cmd];
        }
    }
}

