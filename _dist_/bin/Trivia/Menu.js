import { printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { BadgeProps, badge } from "../bin/.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { DOMAttr, Prop, HTMLAttr } from "../bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { mapIndexed, ofSeq, ofArray, singleton } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import * as react from "../../../web_modules/react.js";
import { singleton as singleton_1, append, delay } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { Record } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { record_type, string_type } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { Range$$reflection } from "../shared/TriviaShared.js";
import { NavItemProps, navItem } from "../bin/.fable/Fable.Reactstrap.0.5.1/NavItem.fs.js";
import { NavLinkProps, navLink } from "../bin/.fable/Fable.Reactstrap.0.5.1/NavLink.fs.js";
import { NavProps, nav } from "../bin/.fable/Fable.Reactstrap.0.5.1/Nav.fs.js";

export function rangeToText(r) {
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
    let navItems;
    navItems = mapIndexed((idx, mi) => {
        let className;
        const arg10 = (idx === activeIndex) ? "active" : "";
        const clo1 = toText(printf("d-flex %s %s"));
        const clo2 = clo1(arg10);
        className = clo2(mi.ClassName);
        return navItem([new NavItemProps(2, ofArray([new Prop(0, idx), new HTMLAttr(158, mi.Title), new DOMAttr(40, (ev) => {
            ev.preventDefault();
            onItemClick(idx);
        })]))], [navLink([new NavLinkProps(3, ofArray([new HTMLAttr(94, "#"), new HTMLAttr(64, className)]))], [react.createElement("span", {
            className: "mr-4",
        }, mi.Label), rangeToBadge(mi.Range)])]);
    }, items);
    return nav([new NavProps(1, true), new NavProps(9, singleton(new HTMLAttr(64, "flex-column")))], [Array.from(navItems)]);
}

