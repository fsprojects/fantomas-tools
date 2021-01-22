import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { postJson, getText } from "../Http.js";
import { split, printf, toText } from "../.fable/fable-library.3.1.1/String.js";
import { encodeUrlModel, encodeInput } from "./Encoders.js";
import { decodeUrlModel, decodeResult } from "./Decoders.js";
import { Model, EditorState, View as View_2, Msg } from "./Model.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.1/Choice.js";
import { updateUrlWithData, restoreModelFromUrl } from "../UrlTools.js";
import { uncurry } from "../.fable/fable-library.3.1.1/Util.js";
import { Cmd_ofSub, Cmd_batch, Cmd_none, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { Input } from "../shared/ASTViewerShared.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { ofArray } from "../.fable/fable-library.3.1.1/List.js";
import { selectRange } from "../Editor.js";

function getVersion() {
    let arg10;
    return getText((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND), toText(printf("%s/%s"))(arg10)("api/version")));
}

function fetchNodeRequest(url, payload, dispatch) {
    const pr = postJson(url, encodeInput(payload));
    pr.then(((tupledArg) => {
        let matchValue;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = decodeResult(body), (matchValue.tag === 1) ? (new Msg(5, toText(printf("failed to decode response: %A"))(matchValue.fields[0]))) : (new Msg(4, matchValue.fields[0]))) : ((status === 400) ? (new Msg(5, body)) : ((status === 413) ? (new Msg(5, "the input was too large to process")) : (new Msg(5, body)))));
    }));
}

function fetchUntypedAST(payload, dispatch) {
    let arg10;
    fetchNodeRequest((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND), toText(printf("%s/api/untyped-ast"))(arg10)), payload, dispatch);
}

function fetchTypedAst(payload, dispatch) {
    let arg10;
    fetchNodeRequest((arg10 = (__SNOWPACK_ENV__.SNOWPACK_PUBLIC_AST_BACKEND), toText(printf("%s/api/typed-ast"))(arg10)), payload, dispatch);
}

const initialModel = new Model("", "", false, new FSharpResult$2(0, void 0), false, "", new View_2(1), new EditorState(0));

function getMessageFromError(ex) {
    return new Msg(5, ex.message);
}

export function init(isActive) {
    return [isActive ? restoreModelFromUrl(uncurry(2, decodeUrlModel(initialModel)), initialModel) : initialModel, Cmd_OfPromise_either(getVersion, void 0, (arg0) => (new Msg(0, arg0)), getMessageFromError)];
}

function getDefines(model) {
    return split(model.Defines, [" ", ",", ";"], null, 1);
}

function modelToParseRequest(sourceCode, model) {
    return new Input(sourceCode, getDefines(model), model.IsFsi);
}

function updateUrl(code, model, _arg1) {
    updateUrlWithData(toString(2, encodeUrlModel(code, model)));
}

export function update(code, msg, model) {
    switch (msg.tag) {
        case 4: {
            return [new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(0, msg.fields[0]), false, model.Version, model.View, model.FSharpEditorState), Cmd_none()];
        }
        case 5: {
            return [new Model(model.Source, model.Defines, model.IsFsi, new FSharpResult$2(1, msg.fields[0]), false, model.Version, model.View, model.FSharpEditorState), Cmd_none()];
        }
        case 2: {
            const parseRequest = modelToParseRequest(code, model);
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.View, model.FSharpEditorState), Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                fetchUntypedAST(parseRequest, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]))];
        }
        case 3: {
            const parseRequest_1 = modelToParseRequest(code, model);
            return [new Model(model.Source, model.Defines, model.IsFsi, model.Parsed, true, model.Version, model.View, model.FSharpEditorState), Cmd_batch(ofArray([Cmd_ofSub((dispatch_1) => {
                fetchTypedAst(parseRequest_1, dispatch_1);
            }), Cmd_ofSub((arg20$0040_1) => {
                updateUrl(code, model, arg20$0040_1);
            })]))];
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
            return [new Model(msg.fields[0], model.Defines, model.IsFsi, model.Parsed, model.IsLoading, model.Version, model.View, model.FSharpEditorState), Cmd_none()];
        }
    }
}

