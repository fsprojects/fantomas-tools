import { fromString, array, int, bool, string, object } from "../.fable/Thoth.Json.5.1.0/Decode.fs.js";
import { Model } from "./Model.js";
import { uncurry } from "../.fable/fable-library.3.1.15/Util.js";
import { Response, ASTError, Range$ } from "../shared/ASTViewerShared.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.15/Choice.js";

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => (new Model(initialModel.Source, get$.Required.Field("defines", (path, value) => string(path, value)), get$.Required.Field("isFsi", (path_1, value_1) => bool(path_1, value_1)), initialModel.Parsed, initialModel.IsLoading, initialModel.Version, initialModel.FSharpEditorState)), path_2, v));
}

const rangeDecoder = (path) => ((v) => object((get$) => (new Range$(get$.Required.Field("startLine", uncurry(2, int)), get$.Required.Field("startCol", uncurry(2, int)), get$.Required.Field("endLine", uncurry(2, int)), get$.Required.Field("endCol", uncurry(2, int)))), path, v));

export function decodeKeyValue(_arg1) {
    return (arg0) => (new FSharpResult$2(0, arg0));
}

const decodeASTError = (path_3) => ((v) => object((get$) => (new ASTError(get$.Required.Field("subcategory", (path, value) => string(path, value)), get$.Required.Field("range", uncurry(2, rangeDecoder)), get$.Required.Field("severity", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("errorNumber", uncurry(2, int)), get$.Required.Field("message", (path_2, value_2) => string(path_2, value_2)))), path_3, v));

export const responseDecoder = (path_2) => ((v) => object((get$) => (new Response(get$.Required.Field("string", (path, value) => string(path, value)), get$.Required.Field("errors", (path_1, value_1) => array(uncurry(2, decodeASTError), path_1, value_1)))), path_2, v));

export function decodeResult(json) {
    return fromString(uncurry(2, responseDecoder), json);
}

