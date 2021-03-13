import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { postJson, getText } from "../Http.js";
import { split, printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { encodeUrlModel, encodeInput } from "./Encoders.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, EditorState, Msg } from "./Model.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.1.7/Util.js";
import { Cmd_ofSub, Cmd_batch, Cmd_none, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { Request } from "../shared/ASTViewerShared.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.7/Choice.js";
import { ofArray } from "../.fable/fable-library.3.1.7/List.js";
import { selectRange } from "../Editor.js";

function getVersion() {
    let arg10;
    return getText((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND), toText(printf("%s/%s"))(arg10)("api/version")));
}

function fetchNodeRequest(url, payload, dispatch) {
    const json = encodeInput(payload);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        let matchValue, err, r;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (err = matchValue.fields[0], new Msg(5, toText(printf("failed to decode response: %A"))(err))) : (r = matchValue.fields[0], new Msg(4, r))) : ((status === 400) ? (new Msg(5, body)) : ((status === 413) ? (new Msg(5, "the input was too large to process")) : (new Msg(5, body)))));
    }));
}

function fetchUntypedAST(payload, dispatch) {
    let url;
    const arg10 = __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND;
    url = toText(printf("%s/api/untyped-ast"))(arg10);
    fetchNodeRequest(url, payload, dispatch);
}

function fetchTypedAst(payload, dispatch) {
    let url;
    const arg10 = __SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND;
    url = toText(printf("%s/api/typed-ast"))(arg10);
    fetchNodeRequest(url, payload, dispatch);
}

const initialModel = new Model("", "", false, void 0, false, "", new EditorState(0));

function getMessageFromError(ex) {
    return new Msg(5, ex.message);
}

export function init(isActive) {
    const model = isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel;
    const cmd = Cmd_OfPromise_either(getVersion, void 0, (arg0) => (new Msg(0, arg0)), (ex) => getMessageFromError(ex));
    return [model, cmd];
}

function getDefines(model) {
    return split(model.Defines, [" ", ",", ";"], null, 1);
}

function modelToParseRequest(sourceCode, model) {
    return new Request(sourceCode, getDefines(model), model.IsFsi);
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 4: {
            const x_1 = msg.fields[0];
            const nextModel_1 = new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(0, x_1), false, model.Version, model.FSharpEditorState);
            return [nextModel_1, Cmd_none()];
        }
        case 5: {
            const e = msg.fields[0];
            const nextModel_2 = new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(1, e), false, model.Version, model.FSharpEditorState);
            return [nextModel_2, Cmd_none()];
        }
        case 2: {
            const parseRequest = modelToParseRequest(code, model);
            const cmd = Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                fetchUntypedAST(parseRequest, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]));
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.FSharpEditorState), cmd];
        }
        case 3: {
            const parseRequest_1 = modelToParseRequest(code, model);
            const cmd_1 = Cmd_batch(ofArray([Cmd_ofSub((dispatch_1) => {
                fetchTypedAst(parseRequest_1, dispatch_1);
            }), Cmd_ofSub((arg20$0040_1) => {
                updateUrl(code, model, arg20$0040_1);
            })]));
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.FSharpEditorState), cmd_1];
        }
        case 0: {
            const version = msg.fields[0];
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, model.IsLoading, version, model.FSharpEditorState), Cmd_none()];
        }
        case 6: {
            const defines = msg.fields[0];
            return [new Model(model.Source, defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, model.FSharpEditorState), Cmd_none()];
        }
        case 7: {
            const isFsi = msg.fields[0];
            return [new Model(model.Source, model.Defines, isFsi, model.Parsed, model.IsLoading, model.Version, model.FSharpEditorState), Cmd_none()];
        }
        case 8: {
            const hlr = msg.fields[0];
            return [model, Cmd_ofSub((arg10$0040) => {
                selectRange(hlr, arg10$0040);
            })];
        }
        default: {
            const x = msg.fields[0];
            const nextModel = new Model(x, model.Defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, model.FSharpEditorState);
            return [nextModel, Cmd_none()];
        }
    }
}

