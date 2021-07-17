import { toArray, mapIndexed } from "../.fable/fable-library.3.2.9/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { interpolate, printf, toText } from "../.fable/fable-library.3.2.9/String.js";
import { HighLightRange } from "../Editor.js";
import { Msg } from "./Model.js";

export function view(model, dispatch) {
    const nodes = mapIndexed((idx, node) => react.createElement("tr", {
        key: toText(printf("node_%d"))(idx),
        className: toText(interpolate("trivia-candidate-%P() pointer", [node.Type])),
        onClick: (_arg1) => {
            let r;
            dispatch(new Msg(8, (r = node.Range, new HighLightRange(r.StartLine, r.StartColumn, r.EndLine, r.EndColumn))));
        },
    }, react.createElement("td", {}, node.Name), react.createElement("td", {
        className: "text-center",
    }, node.Range.StartLine), react.createElement("td", {
        className: "text-center",
    }, node.Range.StartColumn), react.createElement("td", {
        className: "text-center",
    }, node.Range.EndLine), react.createElement("td", {
        className: "text-center",
    }, node.Range.EndColumn)), model.TriviaNodeCandidates);
    return react.createElement("div", {}, react.createElement("table", {
        className: "table table-bordered",
    }, react.createElement("thead", {}, react.createElement("tr", {}, react.createElement("th", {}, "Name"), react.createElement("th", {
        className: "text-center",
    }, "StartLine"), react.createElement("th", {
        className: "text-center",
    }, "StartCol"), react.createElement("th", {
        className: "text-center",
    }, "EndLine"), react.createElement("th", {
        className: "text-center",
    }, "EndCol"))), react.createElement("tbody", {}, toArray(nodes))));
}

