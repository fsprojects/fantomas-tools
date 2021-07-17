import { printf, toText } from "../.fable/fable-library.3.2.9/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { DOMAttr, Prop, HTMLAttr } from "../.fable/Fable.React.7.4.0/Fable.React.Props.fs.js";
import { mapIndexed, toArray, ofArray, singleton } from "../.fable/fable-library.3.2.9/List.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { singleton as singleton_1, append, delay, toList } from "../.fable/fable-library.3.2.9/Seq.js";
import { Record } from "../.fable/fable-library.3.2.9/Types.js";
import { record_type, string_type } from "../.fable/fable-library.3.2.9/Reflection.js";
import { Range$$reflection } from "../shared/TriviaShared.js";
import { NavProps, nav } from "../.fable/Fable.Reactstrap.0.5.1/Nav.fs.js";
import { NavItemProps, navItem } from "../.fable/Fable.Reactstrap.0.5.1/NavItem.fs.js";
import { NavLinkProps, navLink } from "../.fable/Fable.Reactstrap.0.5.1/NavLink.fs.js";

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
            return react.createElement(react.Fragment, {}, ...wrap("StringContent", tc.fields[0]));
        }
        case 9: {
            return react.createElement(react.Fragment, {}, ...wrap("CharContent", tc.fields[0]));
        }
        case 5: {
            const c = tc.fields[0];
            const inner_1 = (c.tag === 1) ? wrap("LineCommentOnSingleLine", c.fields[0]) : ((c.tag === 0) ? wrap("LineCommentAfterSourceCode", c.fields[0]) : wrap("BlockComment", c.fields[0]));
            return react.createElement(react.Fragment, {}, ...toList(delay(() => append(singleton_1("Comment("), delay(() => append(inner_1, delay(() => singleton_1(")"))))))));
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
        case 10: {
            return react.createElement(react.Fragment, {}, ...wrap("EmbeddedIL", tc.fields[0]));
        }
        case 11: {
            return react.createElement(react.Fragment, {}, ...wrap("KeywordString", tc.fields[0]));
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
    return nav([new NavProps(1, true), new NavProps(9, singleton(new HTMLAttr(64, "flex-column")))], [toArray(mapIndexed((idx, mi) => {
        let arg10;
        return navItem([new NavItemProps(2, ofArray([new Prop(0, idx), new HTMLAttr(158, mi.Title), new DOMAttr(40, (ev) => {
            ev.preventDefault();
            onItemClick(idx);
        })]))], [navLink([new NavLinkProps(3, ofArray([new HTMLAttr(94, "#"), new HTMLAttr(64, (arg10 = ((idx === activeIndex) ? "active" : ""), toText(printf("d-flex %s %s"))(arg10)(mi.ClassName)))]))], [react.createElement("span", {
            className: "mr-4",
        }, mi.Label), rangeToBadge(mi.Range)])]);
    }, items))]);
}

