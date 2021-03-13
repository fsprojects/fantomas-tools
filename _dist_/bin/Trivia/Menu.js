import { printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { DOMAttr, Prop, HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { toArray, mapIndexed, ofArray, singleton } from "../.fable/fable-library.3.1.7/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { singleton as singleton_1, append, delay, toList } from "../.fable/fable-library.3.1.7/Seq.js";
import { Record } from "../.fable/fable-library.3.1.7/Types.js";
import { record_type, string_type } from "../.fable/fable-library.3.1.7/Reflection.js";
import { Range$$reflection } from "../shared/TriviaShared.js";
import { NavItemProps, navItem } from "../.fable/Fable.Reactstrap.0.5.1/NavItem.fs.js";
import { NavLinkProps, navLink } from "../.fable/Fable.Reactstrap.0.5.1/NavLink.fs.js";
import { NavProps, nav } from "../.fable/Fable.Reactstrap.0.5.1/Nav.fs.js";

export function rangeToText(r) {
    return toText(printf("(%i,%i - %i,%i)"))(r.StartLine)(r.StartColumn)(r.EndLine)(r.EndColumn);
}

function rangeToBadge(r) {
    return badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(64, "px-2 py-1 ml-auto")))], [rangeToText(r)]);
}

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

export class MenuItem extends Record {
    constructor(ClassName, Label, Title, Range$) {
        super();
        this.ClassName = ClassName;
        this.Label = Label;
        this.Title = Title;
        this.Range = Range$;
    }
}

export function MenuItem$reflection() {
    return record_type("FantomasTools.Client.Trivia.Menu.MenuItem", [], MenuItem, () => [["ClassName", string_type], ["Label", string_type], ["Title", string_type], ["Range", Range$$reflection()]]);
}

export function menu(onItemClick, activeIndex, items) {
    const navItems = mapIndexed((idx, mi) => {
        let className;
        const arg10 = (idx === activeIndex) ? "active" : "";
        className = toText(printf("d-flex %s %s"))(arg10)(mi.ClassName);
        return navItem([new NavItemProps(2, ofArray([new Prop(0, idx), new HTMLAttr(158, mi.Title), new DOMAttr(40, (ev) => {
            ev.preventDefault();
            onItemClick(idx);
        })]))], [navLink([new NavLinkProps(3, ofArray([new HTMLAttr(94, "#"), new HTMLAttr(64, className)]))], [react.createElement("span", {
            className: "mr-4",
        }, mi.Label), rangeToBadge(mi.Range)])]);
    }, items);
    return nav([new NavProps(1, true), new NavProps(9, singleton(new HTMLAttr(64, "flex-column")))], [toArray(navItems)]);
}

