import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { split, printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { encodeUrlModel, encodeParseRequest } from "./Encoders.js";
import { getText, postJson } from "../Http.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, ActiveTab as ActiveTab_1, Msg } from "./Model.js";
import { tryItem, ofArray, empty } from "../.fable/fable-library.3.1.7/List.js";
import { ParseRequest } from "../shared/TriviaShared.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.1.7/Util.js";
import { Cmd_none, Cmd_ofSub, Cmd_batch, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { defaultArg, map } from "../.fable/fable-library.3.1.7/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function fetchTrivia(payload, dispatch) {
    let url;
    const arg10 = __SNOWPACK_ENV__.SNOWPACK_PUBLIC_TRIVIA_BACKEND;
    url = toText(printf("%s/api/get-trivia"))(arg10);
    const json = encodeParseRequest(payload);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        let matchValue, err, r;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (err = matchValue.fields[0], new Msg(7, toText(printf("failed to decode response: %A"))(err))) : (r = matchValue.fields[0], new Msg(2, r))) : ((status === 400) ? (new Msg(7, body)) : ((status === 413) ? (new Msg(7, "the input was too large to process")) : (new Msg(7, body)))));
    }));
}

function fetchFSCVersion() {
    let arg10;
    return getText((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_TRIVIA_BACKEND), toText(printf("%s/api/version"))(arg10)));
}

const initialModel = new Model(new ActiveTab_1(0), empty(), empty(), empty(), void 0, true, 0, 0, "", "???", false);

function splitDefines(value) {
    return ofArray(split(value, [" ", ";"], null, 1));
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
            const err = msg.fields[0];
            return [new Model(initialModel.ActiveTab, initialModel.Trivia, initialModel.TriviaNodeCandidates, initialModel.TriviaNodes, err, false, initialModel.ActiveByTriviaNodeIndex, initialModel.ActiveByTriviaIndex, initialModel.Defines, initialModel.Version, initialModel.IsFsi), Cmd_none()];
        }
        case 3: {
            const tab_1 = msg.fields[0];
            const index = msg.fields[1] | 0;
            let patternInput;
            switch (tab_1.tag) {
                case 0: {
                    const range = map((t) => t.Range, tryItem(index, model.TriviaNodes));
                    patternInput = [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, index, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), range];
                    break;
                }
                case 1: {
                    const range_1 = map((tv) => tv.Range, tryItem(index, model.Trivia));
                    patternInput = [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, index, model.Defines, model.Version, model.IsFsi), range_1];
                    break;
                }
                default: {
                    patternInput = [model, void 0];
                }
            }
            const range_2 = patternInput[1];
            const model_1 = patternInput[0];
            const cmd_1 = defaultArg(map((r) => {
                const highLightRange = new HighLightRange(r.StartLine, r.StartColumn, r.EndLine, r.EndColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(highLightRange, arg10$0040);
                });
            }, range_2), Cmd_none());
            return [model_1, cmd_1];
        }
        case 4: {
            const d = msg.fields[0];
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, d, model.Version, model.IsFsi), Cmd_none()];
        }
        case 5: {
            const version = msg.fields[0];
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, false, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, version, model.IsFsi), Cmd_none()];
        }
        case 6: {
            const v = msg.fields[0];
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, v), Cmd_none()];
        }
        default: {
            const tab = msg.fields[0];
            return [new Model(tab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), Cmd_none()];
        }
    }
}

