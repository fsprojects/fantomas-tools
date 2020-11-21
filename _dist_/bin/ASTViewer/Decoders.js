import { fromString, array, int, bool, string, object } from "../bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { Model } from "./Model.js";
import { Lazy, uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Dto, Node$, Range$ } from "../shared/ASTViewerShared.js";
import { FSharpResult$2 } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => {
        let Defines;
        const objectArg = get$.Required;
        Defines = objectArg.Field("defines", string);
        let IsFsi;
        const objectArg_1 = get$.Required;
        IsFsi = objectArg_1.Field("isFsi", bool);
        return new Model(initialModel.Source, Defines, IsFsi, initialModel.Parsed, initialModel.IsLoading, initialModel.Version, initialModel.View, initialModel.FSharpEditorState);
    }, path_2, v));
}

const rangeDecoder = (path) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3;
    return new Range$((objectArg = get$.Required, objectArg.Field("startLine", uncurry(2, int))), (objectArg_1 = get$.Required, objectArg_1.Field("startCol", uncurry(2, int))), (objectArg_2 = get$.Required, objectArg_2.Field("endLine", uncurry(2, int))), (objectArg_3 = get$.Required, objectArg_3.Field("endCol", uncurry(2, int))));
}, path, v));

export function decodeKeyValue(_arg1) {
    return (arg0) => (new FSharpResult$2(0, arg0));
}

function nodeDecoder$004026() {
    return (path_2) => ((v) => object((get$) => {
        let objectArg, objectArg_1, objectArg_2, arg10_3, decoder, objectArg_3;
        return new Node$((objectArg = get$.Required, objectArg.Field("type", string)), (objectArg_1 = get$.Optional, objectArg_1.Field("range", uncurry(2, rangeDecoder))), (objectArg_2 = get$.Required, objectArg_2.Field("properties", uncurry(2, decodeKeyValue))), (arg10_3 = (decoder = nodeDecoder$004026$002D1.Value, (path_1) => ((value_1) => array(uncurry(2, decoder), path_1, value_1))), (objectArg_3 = get$.Required, objectArg_3.Field("childs", uncurry(2, arg10_3)))));
    }, path_2, v));
}

const nodeDecoder$004026$002D1 = new Lazy(nodeDecoder$004026);

const nodeDecoder = nodeDecoder$004026$002D1.Value;

export const responseDecoder = (path_1) => ((v) => object((get$) => {
    let objectArg, objectArg_1;
    return new Dto((objectArg = get$.Required, objectArg.Field("node", uncurry(2, nodeDecoder))), (objectArg_1 = get$.Required, objectArg_1.Field("string", string)));
}, path_1, v));

export function decodeResult(json) {
    return fromString(uncurry(2, responseDecoder), json);
}

