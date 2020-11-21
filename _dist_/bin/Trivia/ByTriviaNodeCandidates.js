import { mapIndexed } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { HTMLAttr, Prop } from "../bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import * as react from "../../../web_modules/react.js";
import { keyValueList } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";

export function view(model, _dispatch) {
    let children_28, children_24, children_22, children_26;
    let nodes;
    nodes = mapIndexed((idx, node) => {
        let clo1, clo1_1;
        const props_10 = [new Prop(0, (clo1 = toText(printf("node_%d")), clo1(idx))), new HTMLAttr(64, (clo1_1 = toText(printf("trivia-candidate-%s")), clo1_1(node.Type)))];
        const children_10 = [react.createElement("td", {}, node.Name), react.createElement("td", {
            className: "text-center",
        }, node.Range.StartLine), react.createElement("td", {
            className: "text-center",
        }, node.Range.StartColumn), react.createElement("td", {
            className: "text-center",
        }, node.Range.EndLine), react.createElement("td", {
            className: "text-center",
        }, node.Range.EndColumn)];
        return react.createElement("tr", keyValueList(props_10, 1), ...children_10);
    }, model.TriviaNodeCandidates);
    const children_30 = [(children_28 = [(children_24 = [(children_22 = [react.createElement("th", {}, "Name"), react.createElement("th", {
        className: "text-center",
    }, "StartLine"), react.createElement("th", {
        className: "text-center",
    }, "StartCol"), react.createElement("th", {
        className: "text-center",
    }, "EndLine"), react.createElement("th", {
        className: "text-center",
    }, "EndCol")], react.createElement("tr", {}, ...children_22))], react.createElement("thead", {}, ...children_24)), (children_26 = [Array.from(nodes)], react.createElement("tbody", {}, ...children_26))], react.createElement("table", {
        className: "table table-bordered",
    }, ...children_28))];
    return react.createElement("div", {
        className: "d-flex h-100",
    }, ...children_30);
}

