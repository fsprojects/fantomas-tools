import { printf, toText } from "../.fable/fable-library.3.0.1/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { tryItem, map, mapIndexed, ofSeq, ofArray, singleton } from "../.fable/fable-library.3.0.1/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { singleton as singleton_1, append, delay } from "../.fable/fable-library.3.0.1/Seq.js";
import { map as map_1, toArray } from "../.fable/fable-library.3.0.1/Option.js";
import { menu, MenuItem } from "./Menu.js";
import { ActiveTab, Msg } from "./Model.js";

function typeName(_arg1) {
    if (_arg1.tag === 0) {
        return _arg1.fields[0];
    }
    else {
        return _arg1.fields[0];
    }
}

function typeTitle(_arg1) {
    if (_arg1.tag === 0) {
        return "MainNode";
    }
    else {
        return "Token";
    }
}

function rangeToText(r) {
    return toText(printf("(%i,%i - %i,%i)"))(r.StartLine)(r.StartColumn)(r.EndLine)(r.EndColumn);
}

function rangeToBadge(r) {
    return badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(64, "px-2 py-1 ml-auto")))], [rangeToText(r)]);
}

const isNotAnEmptyList = (arg) => (!(arg.tail == null));

function triviaContentToDetail(tc) {
    const wrap = (outer, inner) => ofArray([toText(printf("%s("))(outer), react.createElement("code", {}, inner), ")"]);
    switch (tc.tag) {
        case 2: {
            return react.createElement(react.Fragment, {}, ...wrap("StringContent", tc.fields[0]));
        }
        case 9: {
            return react.createElement(react.Fragment, {}, ...wrap("CharContent", tc.fields[0]));
        }
        case 5: {
            const c = tc.fields[0];
            const inner_1 = (c.tag === 1) ? wrap("LineCommentOnSingleLine", c.fields[0]) : ((c.tag === 0) ? wrap("LineCommentAfterSourceCode", c.fields[0]) : wrap("BlockComment", c.fields[0]));
            return react.createElement(react.Fragment, {}, ...ofSeq(delay(() => append(singleton_1("Comment("), delay(() => append(inner_1, delay(() => singleton_1(")"))))))));
        }
        case 7: {
            return react.createElement(react.Fragment, {}, ...wrap("Directive", tc.fields[0]));
        }
        case 3: {
            return react.createElement(react.Fragment, {}, ...wrap("IdentOperatorAsWord", tc.fields[0]));
        }
        case 4: {
            return react.createElement(react.Fragment, {}, ...wrap("IdentBetweenTicks", tc.fields[0]));
        }
        case 1: {
            return react.createElement(react.Fragment, {}, ...wrap("Number", tc.fields[0]));
        }
        case 8: {
            return "NewlineAfter";
        }
        case 0: {
            return react.createElement(react.Fragment, {}, ...wrap("Keyword", tc.fields[0]));
        }
        default: {
            return "Newline";
        }
    }
}

function activeTriviaNode(tn) {
    let arg20, arg10;
    const contentInfo = (title_1, items) => {
        if (isNotAnEmptyList(items)) {
            const listItems = mapIndexed((idx, item) => react.createElement("li", {
                key: idx,
            }, triviaContentToDetail(item)), items);
            return react.createElement(react.Fragment, {}, react.createElement("h4", {}, title_1), react.createElement("ul", {
                className: "list-unstyled",
            }, Array.from(listItems)));
        }
        else {
            const o = void 0;
            if (o == null) {
                return null;
            }
            else {
                return o;
            }
        }
    };
    return react.createElement("div", {
        className: "tab-pane active",
    }, react.createElement("h2", {
        className: "mb-4",
    }, (arg20 = rangeToText(tn.Range), (arg10 = typeName(tn.Type), toText(printf("%s %s"))(arg10)(arg20)))), contentInfo("Content before", tn.ContentBefore), contentInfo("Content itself", ofArray(toArray(tn.ContentItself))), contentInfo("Content after", tn.ContentAfter));
}

export function view(model, dispatch) {
    let o;
    const navItems = map((tn) => (new MenuItem((tn.Type.tag === 0) ? "nav-link-main-node" : "nav-link-token", typeName(tn.Type), typeTitle(tn.Type), tn.Range)), model.TriviaNodes);
    const activeNode = map_1(activeTriviaNode, tryItem(model.ActiveByTriviaNodeIndex, model.TriviaNodes));
    return react.createElement("div", {
        className: "d-flex h-100",
    }, menu((idx) => {
        dispatch(new Msg(3, new ActiveTab(0), idx));
    }, model.ActiveByTriviaNodeIndex, navItems), react.createElement("div", {
        className: "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto",
    }, (o = activeNode, (o == null) ? null : o)));
}

