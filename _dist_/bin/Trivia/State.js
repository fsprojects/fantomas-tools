import __SNOWPACK_ENV__ from '../../../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import { split, printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { encodeUrlModel, encodeParseRequest } from "./Encoders.js";
import { getText, postJson } from "../Http.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, ActiveTab as ActiveTab_1, Msg } from "./Model.js";
import { tryItem, ofArray, empty } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { ParseRequest } from "../shared/TriviaShared.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Cmd_none, Cmd_ofSub, Cmd_batch, Cmd_OfPromise_either } from "../bin/.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { toString } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { defaultArg, map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function fetchTrivia(payload, dispatch) {
    let url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_TRIVIA_BACKEND;
    const clo1 = toText(printf("%s/api/get-trivia"));
    url = clo1(arg10);
    const json = encodeParseRequest(payload);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        let matchValue, clo1_1;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (new Msg(7, (clo1_1 = toText(printf("failed to decode response: %A")), clo1_1(matchValue.fields[0])))) : (new Msg(2, matchValue.fields[0]))) : ((status === 400) ? (new Msg(7, body)) : ((status === 413) ? (new Msg(7, "the input was too large to process")) : (new Msg(7, body)))));
    }));
}

function fetchFSCVersion() {
    let _url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_TRIVIA_BACKEND;
    const clo1 = toText(printf("%s/api/version"));
    _url = clo1(arg10);
    return getText(_url);
}

const initialModel = new Model(new ActiveTab_1(0), empty(), empty(), empty(), void 0, true, 0, 0, "", "???", false);

function splitDefines(value) {
    const array = split(value, [" ", ";"], null, 1);
    return ofArray(array);
}

function modelToParseRequest(sourceCode, model) {
    return new ParseRequest(sourceCode, splitDefines(model.Defines), model.IsFsi ? "script.fsi" : "script.fsx");
}

export function init(isActive) {
    const model = isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel;
    const cmd = Cmd_OfPromise_either(fetchFSCVersion, void 0, (arg0) => (new Msg(5, arg0)), (ex) => (new Msg(7, ex.message)));
    return [model, cmd];
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const parseRequest = modelToParseRequest(code, model);
            const cmd = Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                fetchTrivia(parseRequest, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]));
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, true, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), cmd];
        }
        case 2: {
            const result = msg.fields[0];
            return [new Model(model.ActiveTab, result.Trivia, result.TriviaNodeCandidates, result.TriviaNodes, model.Error, false, 0, 0, model.Defines, model.Version, model.IsFsi), Cmd_none()];
        }
        case 7: {
            return [new Model(initialModel.ActiveTab, initialModel.Trivia, initialModel.TriviaNodeCandidates, initialModel.TriviaNodes, msg.fields[0], false, initialModel.ActiveByTriviaNodeIndex, initialModel.ActiveByTriviaIndex, initialModel.Defines, initialModel.Version, initialModel.IsFsi), Cmd_none()];
        }
        case 3: {
            const tab_1 = msg.fields[0];
            const index = msg.fields[1] | 0;
            let patternInput;
            switch (tab_1.tag) {
                case 0: {
                    let range;
                    const option = tryItem(index, model.TriviaNodes);
                    range = map((t) => t.Range, option);
                    patternInput = [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, index, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), range];
                    break;
                }
                case 1: {
                    let range_1;
                    const option_1 = tryItem(index, model.Trivia);
                    range_1 = map((tv) => tv.Range, option_1);
                    patternInput = [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, index, model.Defines, model.Version, model.IsFsi), range_1];
                    break;
                }
                default: {
                    patternInput = [model, void 0];
                }
            }
            let cmd_1;
            let option_3;
            option_3 = map((r) => {
                const highLightRange = new HighLightRange(r.StartLine, r.StartColumn, r.EndLine, r.EndColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(highLightRange, arg10$0040);
                });
            }, patternInput[1]);
            const value = Cmd_none();
            cmd_1 = defaultArg(option_3, value);
            return [patternInput[0], cmd_1];
        }
        case 4: {
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, msg.fields[0], model.Version, model.IsFsi), Cmd_none()];
        }
        case 5: {
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, false, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, msg.fields[0], model.IsFsi), Cmd_none()];
        }
        case 6: {
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, msg.fields[0]), Cmd_none()];
        }
        default: {
            return [new Model(msg.fields[0], model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), Cmd_none()];
        }
    }
}

