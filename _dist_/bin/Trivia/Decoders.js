import { bool, fromString, string, option as option_2, list, Auto_generateDecoderCached_7848D058, int, object } from "../.fable/Thoth.Json.5.1.0/Decode.fs.js";
import { uncurry } from "../.fable/fable-library.3.2.9/Util.js";
import { ParseResult, TriviaNodeCandidate, TriviaNode, TriviaNodeType$reflection, Trivia, TriviaContent$reflection, Range$ } from "../shared/TriviaShared.js";
import { defaultArg } from "../.fable/fable-library.3.2.9/Option.js";
import { Model } from "./Model.js";

const decodeRange = (path) => ((v) => object((get$) => (new Range$(get$.Required.Field("startLine", uncurry(2, int)), get$.Required.Field("startColumn", uncurry(2, int)), get$.Required.Field("endLine", uncurry(2, int)), get$.Required.Field("endColumn", uncurry(2, int)))), path, v));

const decodeTriviaContent = Auto_generateDecoderCached_7848D058(void 0, void 0, {
    ResolveType: TriviaContent$reflection,
});

const decodeTrivia = (path) => ((v) => object((get$) => (new Trivia(get$.Required.Field("item", uncurry(2, decodeTriviaContent)), get$.Required.Field("range", uncurry(2, decodeRange)))), path, v));

const decodeTriviaNodeType = Auto_generateDecoderCached_7848D058(void 0, void 0, {
    ResolveType: TriviaNodeType$reflection,
});

const decodeTriviaNode = (path_3) => ((v) => object((get$) => (new TriviaNode(get$.Required.Field("type", uncurry(2, decodeTriviaNodeType)), get$.Required.Field("contentBefore", (path, value) => list(uncurry(2, decodeTriviaContent), path, value)), get$.Required.Field("contentItself", (path_1, value_1) => option_2(uncurry(2, decodeTriviaContent), path_1, value_1)), get$.Required.Field("contentAfter", (path_2, value_2) => list(uncurry(2, decodeTriviaContent), path_2, value_2)), get$.Required.Field("range", uncurry(2, decodeRange)))), path_3, v));

const decodeTriviaNodeCandidate = (path_2) => ((v) => object((get$) => (new TriviaNodeCandidate(get$.Required.Field("type", (path, value) => string(path, value)), get$.Required.Field("name", (path_1, value_1) => string(path_1, value_1)), get$.Required.Field("range", uncurry(2, decodeRange)))), path_2, v));

const decodeParseResult = (path_3) => ((v) => object((get$) => (new ParseResult(get$.Required.Field("trivia", (path, value) => list(uncurry(2, decodeTrivia), path, value)), get$.Required.Field("triviaNodeCandidates", (path_1, value_1) => list(uncurry(2, decodeTriviaNodeCandidate), path_1, value_1)), get$.Required.Field("triviaNodes", (path_2, value_2) => list(uncurry(2, decodeTriviaNode), path_2, value_2)))), path_3, v));

export function decodeResult(json) {
    return fromString(uncurry(2, decodeParseResult), json);
}

export function decodeVersion(json) {
    return fromString((path, value) => string(path, value), json);
}

export function decodeUrlModel(initialModel) {
    return (path_2) => ((v) => object((get$) => (new Model(initialModel.ActiveTab, initialModel.Trivia, initialModel.TriviaNodeCandidates, initialModel.TriviaNodes, initialModel.Error, initialModel.IsLoading, initialModel.ActiveByTriviaNodeIndex, initialModel.ActiveByTriviaIndex, defaultArg(get$.Optional.Field("defines", (path, value) => string(path, value)), ""), initialModel.Version, defaultArg(get$.Optional.Field("isFsi", (path_1, value_2) => bool(path_1, value_2)), initialModel.IsFsi))), path_2, v));
}

