import { printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { tryItem, map, toArray, mapIndexed, ofArray, isEmpty, singleton } from "../.fable/fable-library.3.1.7/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { singleton as singleton_1, append, delay, toList } from "../.fable/fable-library.3.1.7/Seq.js";
import { map as map_1, toArray as toArray_1 } from "../.fable/fable-library.3.1.7/Option.js";
import { menu, MenuItem } from "./Menu.js";
import { ActiveTab, Msg } from "./Model.js";

function typeName(_arg1) {
    if (_arg1.tag === 0) {
        const mn = _arg1.fields[0];
        return mn;
    }
    else {
        const t = _arg1.fields[0];
        return t;
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

const isNotAnEmptyList = (arg) => (!isEmpty(arg));

function triviaContentToDetail(tc) {
    const wrap = (outer, inner) => ofArray([toText(printf("%s("))(outer), react.createElement("code", {}, inner), ")"]);
    switch (tc.tag) {
        case 2: {
            const sc = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("StringContent", sc));
        }
        case 9: {
            const cc = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("CharContent", cc));
        }
        case 5: {
            const c = tc.fields[0];
            let inner_1;
            switch (c.tag) {
                case 1: {
                    const lc = c.fields[0];
                    inner_1 = wrap("LineCommentOnSingleLine", lc);
                    break;
                }
                case 0: {
                    const lc_1 = c.fields[0];
                    inner_1 = wrap("LineCommentAfterSourceCode", lc_1);
                    break;
                }
                default: {
                    const bc = c.fields[0];
                    inner_1 = wrap("BlockComment", bc);
                }
            }
            return react.createElement(react.Fragment, {}, ...toList(delay(() => append(singleton_1("Comment("), delay(() => append(inner_1, delay(() => singleton_1(")"))))))));
        }
        case 7: {
            const d = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("Directive", d));
        }
        case 3: {
            const ioaw = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("IdentOperatorAsWord", ioaw));
        }
        case 4: {
            const ibt = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("IdentBetweenTicks", ibt));
        }
        case 1: {
            const n = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("Number", n));
        }
        case 8: {
            return "NewlineAfter";
        }
        case 0: {
            const kw = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("Keyword", kw));
        }
        case 10: {
            const eil = tc.fields[0];
            return react.createElement(react.Fragment, {}, ...wrap("EmbeddedIL", eil));
        }
        default: {
            return "Newline";
        }
    }
}

function activeTriviaNode(tn) {
    let title;
    const arg20 = rangeToText(tn.Range);
    const arg10 = typeName(tn.Type);
    title = toText(printf("%s %s"))(arg10)(arg20);
    const contentInfo = (title_1, items) => {
        if (isNotAnEmptyList(items)) {
            const listItems = mapIndexed((idx, item) => react.createElement("li", {
                key: idx,
            }, triviaContentToDetail(item)), items);
            return react.createElement(react.Fragment, {}, react.createElement("h4", {}, title_1), react.createElement("ul", {
                className: "list-unstyled",
            }, toArray(listItems)));
        }
        else {
            const o = void 0;
            if (o == null) {
                return null;
            }
            else {
                const o_1 = o;
                return o_1;
            }
        }
    };
    return react.createElement("div", {
        className: "tab-pane active",
    }, react.createElement("h2", {
        className: "mb-4",
    }, title), contentInfo("Content before", tn.ContentBefore), contentInfo("Content itself", ofArray(toArray_1(tn.ContentItself))), contentInfo("Content after", tn.ContentAfter));
}

export function view(model, dispatch) {
    let o, o_1;
    const navItems = map((tn) => {
        const className = (tn.Type.tag === 0) ? "nav-link-main-node" : "nav-link-token";
        return new MenuItem(className, typeName(tn.Type), typeTitle(tn.Type), tn.Range);
    }, model.TriviaNodes);
    const onClick = (idx) => {
        dispatch(new Msg(3, new ActiveTab(0), idx));
    };
    const activeNode = map_1((tn_1) => activeTriviaNode(tn_1), tryItem(model.ActiveByTriviaNodeIndex, model.TriviaNodes));
    return react.createElement("div", {
        className: "d-flex h-100",
    }, menu(onClick, model.ActiveByTriviaNodeIndex, navItems), react.createElement("div", {
        className: "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto",
    }, (o = activeNode, (o == null) ? null : (o_1 = o, o_1))));
}

