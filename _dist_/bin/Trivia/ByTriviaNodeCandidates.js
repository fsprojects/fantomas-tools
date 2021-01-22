import { mapIndexed } from "../.fable/fable-library.3.1.1/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { printf, toText } from "../.fable/fable-library.3.1.1/String.js";

export function view(model, _dispatch) {
    const nodes = mapIndexed((idx, node) => react.createElement("tr", {
        key: toText(printf("node_%d"))(idx),
        className: toText(printf("trivia-candidate-%s"))(node.Type),
    }, react.createElement("td", {}, node.Name), react.createElement("td", {
        className: "text-center",
    }, node.Range.StartLine), react.createElement("td", {
        className: "text-center",
    }, node.Range.StartColumn), react.createElement("td", {
        className: "text-center",
    }, node.Range.EndLine), react.createElement("td", {
        className: "text-center",
    }, node.Range.EndColumn)), model.TriviaNodeCandidates);
    return react.createElement("div", {
        className: "d-flex h-100",
    }, react.createElement("table", {
        className: "table table-bordered",
    }, react.createElement("thead", {}, react.createElement("tr", {}, react.createElement("th", {}, "Name"), react.createElement("th", {
        className: "text-center",
    }, "StartLine"), react.createElement("th", {
        className: "text-center",
    }, "StartCol"), react.createElement("th", {
        className: "text-center",
    }, "EndLine"), react.createElement("th", {
        className: "text-center",
    }, "EndCol"))), react.createElement("tbody", {}, Array.from(nodes))));
}

