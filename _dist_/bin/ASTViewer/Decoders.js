import { fromString, array, int, bool, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { Model } from "./Model.js";
import { uncurry } from "../.fable/fable-library.3.1.1/Util.js";
import { Response, ASTError, Range$ } from "../shared/ASTViewerShared.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.1.1/Choice.js";

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => (new Model(initialModel.Source, get$.Required.Field("defines", string), get$.Required.Field("isFsi", bool), initialModel.Parsed, initialModel.IsLoading, initialModel.Version, initialModel.FSharpEditorState)), path_2, v));
}

const rangeDecoder = (path) => ((v) => object((get$) => (new Range$(get$.Required.Field("startLine", uncurry(2, int)), get$.Required.Field("startCol", uncurry(2, int)), get$.Required.Field("endLine", uncurry(2, int)), get$.Required.Field("endCol", uncurry(2, int)))), path, v));

export function decodeKeyValue(_arg1) {
    return (arg0) => (new FSharpResult$2(0, arg0));
}

const decodeASTError = (path_3) => ((v) => object((get$) => (new ASTError(get$.Required.Field("subcategory", string), get$.Required.Field("range", uncurry(2, rangeDecoder)), get$.Required.Field("severity", string), get$.Required.Field("errorNumber", uncurry(2, int)), get$.Required.Field("message", string))), path_3, v));

export const responseDecoder = (path_2) => ((v) => object((get$) => (new Response(get$.Required.Field("string", string), get$.Required.Field("errors", (path_1, value_1) => array(uncurry(2, decodeASTError), path_1, value_1)))), path_2, v));

export function decodeResult(json) {
    return fromString(uncurry(2, responseDecoder), json);
}

