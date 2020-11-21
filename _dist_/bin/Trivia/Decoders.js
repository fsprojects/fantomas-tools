import { bool, fromString, string, option as option_2, list, Auto_generateDecoderCached_7848D058, int, object } from "../bin/.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { ParseResult, TriviaNodeCandidate, TriviaNode, TriviaNodeType$reflection, Trivia, TriviaContent$reflection, Range$ } from "../shared/TriviaShared.js";
import { defaultArg } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { Model } from "./Model.js";

const decodeRange = (path) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3;
    return new Range$((objectArg = get$.Required, objectArg.Field("startLine", uncurry(2, int))), (objectArg_1 = get$.Required, objectArg_1.Field("startColumn", uncurry(2, int))), (objectArg_2 = get$.Required, objectArg_2.Field("endLine", uncurry(2, int))), (objectArg_3 = get$.Required, objectArg_3.Field("endColumn", uncurry(2, int))));
}, path, v));

const decodeTriviaContent = Auto_generateDecoderCached_7848D058(void 0, void 0, {
    ResolveType: TriviaContent$reflection,
});

const decodeTrivia = (path) => ((v) => object((get$) => {
    let objectArg, objectArg_1;
    return new Trivia((objectArg = get$.Required, objectArg.Field("item", uncurry(2, decodeTriviaContent))), (objectArg_1 = get$.Required, objectArg_1.Field("range", uncurry(2, decodeRange))));
}, path, v));

const decodeTriviaNodeType = Auto_generateDecoderCached_7848D058(void 0, void 0, {
    ResolveType: TriviaNodeType$reflection,
});

const decodeTriviaNode = (path_3) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2, objectArg_3, objectArg_4;
    return new TriviaNode((objectArg = get$.Required, objectArg.Field("type", uncurry(2, decodeTriviaNodeType))), (objectArg_1 = get$.Required, objectArg_1.Field("contentBefore", (path, value) => list(uncurry(2, decodeTriviaContent), path, value))), (objectArg_2 = get$.Required, objectArg_2.Field("contentItself", (path_1, value_1) => option_2(uncurry(2, decodeTriviaContent), path_1, value_1))), (objectArg_3 = get$.Required, objectArg_3.Field("contentAfter", (path_2, value_2) => list(uncurry(2, decodeTriviaContent), path_2, value_2))), (objectArg_4 = get$.Required, objectArg_4.Field("range", uncurry(2, decodeRange))));
}, path_3, v));

const decodeTriviaNodeCandidate = (path_2) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2;
    return new TriviaNodeCandidate((objectArg = get$.Required, objectArg.Field("type", string)), (objectArg_1 = get$.Required, objectArg_1.Field("name", string)), (objectArg_2 = get$.Required, objectArg_2.Field("range", uncurry(2, decodeRange))));
}, path_2, v));

const decodeParseResult = (path_3) => ((v) => object((get$) => {
    let objectArg, objectArg_1, objectArg_2;
    return new ParseResult((objectArg = get$.Required, objectArg.Field("trivia", (path, value) => list(uncurry(2, decodeTrivia), path, value))), (objectArg_1 = get$.Required, objectArg_1.Field("triviaNodeCandidates", (path_1, value_1) => list(uncurry(2, decodeTriviaNodeCandidate), path_1, value_1))), (objectArg_2 = get$.Required, objectArg_2.Field("triviaNodes", (path_2, value_2) => list(uncurry(2, decodeTriviaNode), path_2, value_2))));
}, path_3, v));

export function decodeResult(json) {
    return fromString(uncurry(2, decodeParseResult), json);
}

export function decodeVersion(json) {
    return fromString(string, json);
}

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => {
        let defines;
        let option;
        const objectArg = get$.Optional;
        option = objectArg.Field("defines", string);
        defines = defaultArg(option, "");
        let isFsi;
        let option_1;
        const objectArg_1 = get$.Optional;
        option_1 = objectArg_1.Field("isFsi", bool);
        isFsi = defaultArg(option_1, initialModel.IsFsi);
        return new Model(initialModel.ActiveTab, initialModel.Trivia, initialModel.TriviaNodeCandidates, initialModel.TriviaNodes, initialModel.Error, initialModel.IsLoading, initialModel.ActiveByTriviaNodeIndex, initialModel.ActiveByTriviaIndex, defines, initialModel.Version, isFsi);
    }, path_2, v));
}

