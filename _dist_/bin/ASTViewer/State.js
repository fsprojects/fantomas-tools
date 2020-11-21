import __SNOWPACK_ENV__ from '../../../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import { split, printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { postJson, getText } from "../Http.js";
import { encodeUrlModel, encodeInput } from "./Encoders.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, EditorState, View as View_2, Msg } from "./Model.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Cmd_ofSub, Cmd_batch, Cmd_none, Cmd_OfPromise_either } from "../bin/.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { Input } from "../shared/ASTViewerShared.js";
import { toString } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { ofArray } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { selectRange } from "../Editor.js";

function getVersion() {
    let _url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_AST_BACKEND;
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    _url = clo2("api/version");
    return getText(_url);
}

function fetchNodeRequest(url, payload, dispatch) {
    const json = encodeInput(payload);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        let matchValue, clo1;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (new Msg(5, (clo1 = toText(printf("failed to decode response: %A")), clo1(matchValue.fields[0])))) : (new Msg(4, matchValue.fields[0]))) : ((status === 400) ? (new Msg(5, body)) : ((status === 413) ? (new Msg(5, "the input was too large to process")) : (new Msg(5, body)))));
    }));
}

function fetchUntypedAST(payload, dispatch) {
    let url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_AST_BACKEND;
    const clo1 = toText(printf("%s/api/untyped-ast"));
    url = clo1(arg10);
    fetchNodeRequest(url, payload, dispatch);
}

function fetchTypedAst(payload, dispatch) {
    let url;
    const arg10 = import.meta.env.SNOWPACK_PUBLIC_AST_BACKEND;
    const clo1 = toText(printf("%s/api/typed-ast"));
    url = clo1(arg10);
    fetchNodeRequest(url, payload, dispatch);
}

const initialModel = new Model("", "", false, new FSharpResult$2(0, void 0), false, "", new View_2(1), new EditorState(0));

function getMessageFromError(ex) {
    return new Msg(5, ex.message);
}

export function init(isActive) {
    const model = isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel;
    const cmd = Cmd_OfPromise_either(getVersion, void 0, (arg0) => (new Msg(0, arg0)), getMessageFromError);
    return [model, cmd];
}

function getDefines(model) {
    return split(model.Defines, [" ", ",", ";"], null, 1);
}

function modelToParseRequest(sourceCode, model) {
    return new Input(sourceCode, getDefines(model), model.IsFsi);
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 4: {
            const nextModel_1 = new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(0, msg.fields[0]), false, model.Version, model.View, model.FSharpEditorState);
            return [nextModel_1, Cmd_none()];
        }
        case 5: {
            const nextModel_2 = new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(1, msg.fields[0]), false, model.Version, model.View, model.FSharpEditorState);
            return [nextModel_2, Cmd_none()];
        }
        case 2: {
            const parseRequest = modelToParseRequest(code, model);
            const cmd = Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                fetchUntypedAST(parseRequest, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]));
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.View, model.FSharpEditorState), cmd];
        }
        case 3: {
            const parseRequest_1 = modelToParseRequest(code, model);
            const cmd_1 = Cmd_batch(ofArray([Cmd_ofSub((dispatch_1) => {
                fetchTypedAst(parseRequest_1, dispatch_1);
            }), Cmd_ofSub((arg20$0040_1) => {
                updateUrl(code, model, arg20$0040_1);
            })]));
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.View, model.FSharpEditorState), cmd_1];
        }
        case 0: {
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, model.IsLoading, msg.fields[0], model.View, model.FSharpEditorState), Cmd_none()];
        }
        case 6: {
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, new View_2(0), model.FSharpEditorState), Cmd_none()];
        }
        case 7: {
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, new View_2(1), model.FSharpEditorState), Cmd_none()];
        }
        case 8: {
            return [new Model(model.Source, msg.fields[0], model.IsFsi, model.Parsed, model.IsLoading, model.Version, model.View, model.FSharpEditorState), Cmd_none()];
        }
        case 9: {
            return [new Model(model.Source, model.Defines, msg.fields[0], model.Parsed, model.IsLoading, model.Version, model.View, model.FSharpEditorState), Cmd_none()];
        }
        case 10: {
            return [model, Cmd_ofSub((arg10$0040) => {
                selectRange(msg.fields[0], arg10$0040);
            })];
        }
        default: {
            const nextModel = new Model(msg.fields[0], model.Defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, model.View, model.FSharpEditorState);
            return [nextModel, Cmd_none()];
        }
    }
}

