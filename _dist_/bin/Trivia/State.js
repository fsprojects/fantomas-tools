import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { getText, postJson } from "../Http.js";
import { split, printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { encodeUrlModel, encodeParseRequest } from "./Encoders.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, ActiveTab as ActiveTab_1, Msg } from "./Model.js";
import { tryItem, ofArray, empty } from "../.fable/fable-library.3.1.1/List.js";
import { ParseRequest } from "../shared/TriviaShared.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.1.1/Util.js";
import { Cmd_none, Cmd_ofSub, Cmd_batch, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { defaultArg, map } from "../.fable/fable-library.3.1.1/Option.js";
import { selectRange, HighLightRange } from "../Editor.js";

function fetchTrivia(payload, dispatch) {
    let arg10;
    const pr = postJson((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_TRIVIA_BACKEND), toText(printf("%s/api/get-trivia"))(arg10)), encodeParseRequest(payload));
    pr.then(((tupledArg) => {
        let matchValue;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (new Msg(7, toText(printf("failed to decode response: %A"))(matchValue.fields[0]))) : (new Msg(2, matchValue.fields[0]))) : ((status === 400) ? (new Msg(7, body)) : ((status === 413) ? (new Msg(7, "the input was too large to process")) : (new Msg(7, body)))));
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
    return [isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel, Cmd_OfPromise_either(fetchFSCVersion, void 0, (arg0) => (new Msg(5, arg0)), (ex) => (new Msg(7, ex.message)))];
}

function updateUrl(code, model, _arg1) {
    updateUrlWithData(toString(2, encodeUrlModel(code, model)));
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const parseRequest = modelToParseRequest(code, model);
            return [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, true, model.ActiveByTriviaNodeIndex, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                fetchTrivia(parseRequest, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]))];
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
            const patternInput = (tab_1.tag === 0) ? [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, index, model.ActiveByTriviaIndex, model.Defines, model.Version, model.IsFsi), map((t) => t.Range, tryItem(index, model.TriviaNodes))] : ((tab_1.tag === 1) ? [new Model(model.ActiveTab, model.Trivia, model.TriviaNodeCandidates, model.TriviaNodes, model.Error, model.IsLoading, model.ActiveByTriviaNodeIndex, index, model.Defines, model.Version, model.IsFsi), map((tv) => tv.Range, tryItem(index, model.Trivia))] : [model, void 0]);
            return [patternInput[0], defaultArg(map((r) => {
                const highLightRange = new HighLightRange(r.StartLine, r.StartColumn, r.EndLine, r.EndColumn);
                return Cmd_ofSub((arg10$0040) => {
                    selectRange(highLightRange, arg10$0040);
                });
            }, patternInput[1]), Cmd_none())];
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

