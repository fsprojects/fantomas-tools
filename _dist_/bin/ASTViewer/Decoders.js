import { fromString, array, int, bool, string, object } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { Model } from "./Model.js";
import { Lazy, uncurry } from "../.fable/fable-library.3.0.1/Util.js";
import { Dto, Node$, Range$ } from "../shared/ASTViewerShared.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.0.1/Choice.js";

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => (new Model(initialModel.Source, get$.Required.Field("defines", string), get$.Required.Field("isFsi", bool), initialModel.Parsed, initialModel.IsLoading, initialModel.Version, initialModel.View, initialModel.FSharpEditorState)), path_2, v));
}

const rangeDecoder = (path) => ((v) => object((get$) => (new Range$(get$.Required.Field("startLine", uncurry(2, int)), get$.Required.Field("startCol", uncurry(2, int)), get$.Required.Field("endLine", uncurry(2, int)), get$.Required.Field("endCol", uncurry(2, int)))), path, v));

export function decodeKeyValue(_arg1) {
    return (arg0) => (new FSharpResult$2(0, arg0));
}

function nodeDecoder$004026() {
    return (path_2) => ((v) => object((get$) => {
        let decoder;
        return new Node$(get$.Required.Field("type", string), get$.Optional.Field("range", uncurry(2, rangeDecoder)), get$.Required.Field("properties", uncurry(2, decodeKeyValue)), get$.Required.Field("childs", uncurry(2, (decoder = nodeDecoder$004026$002D1.Value, (path_1) => ((value_1) => array(uncurry(2, decoder), path_1, value_1))))));
    }, path_2, v));
}

const nodeDecoder$004026$002D1 = new Lazy(nodeDecoder$004026);

const nodeDecoder = nodeDecoder$004026$002D1.Value;

export const responseDecoder = (path_1) => ((v) => object((get$) => (new Dto(get$.Required.Field("node", uncurry(2, nodeDecoder)), get$.Required.Field("string", string))), path_1, v));

export function decodeResult(json) {
    return fromString(uncurry(2, responseDecoder), json);
}

