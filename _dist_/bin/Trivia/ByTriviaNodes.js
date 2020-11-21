import { printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { BadgeProps, badge } from "../bin/.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { HTMLAttr } from "../bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { tryItem, map, mapIndexed, ofSeq, ofArray, singleton } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import * as react from "../../../web_modules/react.js";
import { singleton as singleton_1, append, delay } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { map as map_1, toArray } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
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
    const clo1 = toText(printf("(%i,%i - %i,%i)"));
    const clo2 = clo1(r.StartLine);
    const clo3 = clo2(r.StartColumn);
    const clo4 = clo3(r.EndLine);
    return clo4(r.EndColumn);
}

function rangeToBadge(r) {
    let s;
    return badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(64, "px-2 py-1 ml-auto")))], [(s = rangeToText(r), (s))]);
}

const isNotAnEmptyList = (arg) => {
    let value;
    value = (arg.tail == null);
    return !value;
};

function triviaContentToDetail(tc) {
    const wrap = (outer, inner) => {
        let s, clo1;
        return ofArray([(s = (clo1 = toText(printf("%s(")), clo1(outer)), s), react.createElement("code", {}, inner), ")"]);
    };
    switch (tc.tag) {
        case 2: {
            const children_2 = wrap("StringContent", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_2);
        }
        case 9: {
            const children_3 = wrap("CharContent", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_3);
        }
        case 5: {
            const c = tc.fields[0];
            const inner_1 = (c.tag === 1) ? wrap("LineCommentOnSingleLine", c.fields[0]) : ((c.tag === 0) ? wrap("LineCommentAfterSourceCode", c.fields[0]) : wrap("BlockComment", c.fields[0]));
            const children_4 = ofSeq(delay(() => append(singleton_1("Comment("), delay(() => append(inner_1, delay(() => singleton_1(")")))))));
            return react.createElement(react.Fragment, {}, ...children_4);
        }
        case 7: {
            const children_5 = wrap("Directive", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_5);
        }
        case 3: {
            const children_6 = wrap("IdentOperatorAsWord", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_6);
        }
        case 4: {
            const children_7 = wrap("IdentBetweenTicks", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_7);
        }
        case 1: {
            const children_8 = wrap("Number", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_8);
        }
        case 8: {
            return "NewlineAfter";
        }
        case 0: {
            const children_9 = wrap("Keyword", tc.fields[0]);
            return react.createElement(react.Fragment, {}, ...children_9);
        }
        default: {
            return "Newline";
        }
    }
}

function activeTriviaNode(tn) {
    let title;
    const arg10 = typeName(tn.Type);
    const arg20 = rangeToText(tn.Range);
    const clo1 = toText(printf("%s %s"));
    const clo2 = clo1(arg10);
    title = clo2(arg20);
    const contentInfo = (title_1, items) => {
        let children_4;
        if (isNotAnEmptyList(items)) {
            let listItems;
            listItems = mapIndexed((idx, item) => {
                const children = [triviaContentToDetail(item)];
                return react.createElement("li", {
                    key: idx,
                }, ...children);
            }, items);
            const children_6 = [react.createElement("h4", {}, title_1), (children_4 = [Array.from(listItems)], react.createElement("ul", {
                className: "list-unstyled",
            }, ...children_4))];
            return react.createElement(react.Fragment, {}, ...children_6);
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
    const children_9 = [react.createElement("h2", {
        className: "mb-4",
    }, title), contentInfo("Content before", tn.ContentBefore), contentInfo("Content itself", ofArray(toArray(tn.ContentItself))), contentInfo("Content after", tn.ContentAfter)];
    return react.createElement("div", {
        className: "tab-pane active",
    }, ...children_9);
}

export function view(model, dispatch) {
    let children, o, o_1;
    let navItems;
    navItems = map((tn) => {
        const className = (tn.Type.tag === 0) ? "nav-link-main-node" : "nav-link-token";
        const Label = typeName(tn.Type);
        return new MenuItem(className, Label, typeTitle(tn.Type), tn.Range);
    }, model.TriviaNodes);
    let activeNode;
    const option = tryItem(model.ActiveByTriviaNodeIndex, model.TriviaNodes);
    activeNode = map_1(activeTriviaNode, option);
    const children_2 = [menu((idx) => {
        dispatch(new Msg(3, new ActiveTab(0), idx));
    }, model.ActiveByTriviaNodeIndex, navItems), (children = [(o = activeNode, (o == null) ? null : (o_1 = o, o_1))], react.createElement("div", {
        className: "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto",
    }, ...children))];
    return react.createElement("div", {
        className: "d-flex h-100",
    }, ...children_2);
}

