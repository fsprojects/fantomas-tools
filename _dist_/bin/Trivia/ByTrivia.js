import { printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { menu, MenuItem, rangeToText } from "./Menu.js";
import { map } from "../.fable/fable-library.3.1.7/Option.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { tryItem, map as map_1 } from "../.fable/fable-library.3.1.7/List.js";
import { ActiveTab, Msg } from "./Model.js";

function contentToClassName(c) {
    const arg10 = (c.tag === 1) ? "number-content" : ((c.tag === 2) ? "string-content" : ((c.tag === 9) ? "char-content" : ((c.tag === 3) ? "ident-operator-keyword" : ((c.tag === 4) ? "ident-between-ticks" : ((c.tag === 5) ? "comment" : ((c.tag === 6) ? "newline" : ((c.tag === 7) ? "directive" : ((c.tag === 8) ? "newline-after" : ((c.tag === 10) ? "embedded-il" : "keyword")))))))));
    return toText(printf("nav-link-%s"))(arg10);
}

function typeName(c) {
    switch (c.tag) {
        case 1: {
            return "Number";
        }
        case 2: {
            return "StringContent";
        }
        case 9: {
            return "CharContent";
        }
        case 3: {
            return "IdentOperatorAsWord";
        }
        case 4: {
            return "IdentBetweenTicks";
        }
        case 5: {
            const c_1 = c.fields[0];
            const arg10 = (c_1.tag === 1) ? "LineCommentOnSingleLine" : ((c_1.tag === 2) ? "BlockComment" : "LineCommentAfterSourceCode");
            return toText(printf("Comment(%s)"))(arg10);
        }
        case 6: {
            return "Newline";
        }
        case 7: {
            return "Directive";
        }
        case 8: {
            return "Newline-after";
        }
        case 10: {
            return "EmbeddedIL";
        }
        default: {
            return "Keyword";
        }
    }
}

function activeTrivia(trivia) {
    let matchValue, i, c, c_1, nb, na, c_2, o, o_1;
    let title;
    const arg20 = rangeToText(trivia.Range);
    const arg10 = typeName(trivia.Item);
    title = toText(printf("%s %s"))(arg10)(arg20);
    const content = map((c_3) => react.createElement("code", {}, c_3), (matchValue = trivia.Item, (matchValue.tag === 1) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 2) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 9) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 3) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 4) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 7) ? (i = matchValue.fields[0], i) : ((matchValue.tag === 5) ? (c = matchValue.fields[0], (c.tag === 1) ? (c_1 = c.fields[0], c_1) : ((c.tag === 2) ? (nb = c.fields[1], (na = c.fields[2], (c_2 = c.fields[0], toText(printf("%s (newline before: %b) (newline after: %b)"))(c_2)(nb)(na)))) : (c_1 = c.fields[0], c_1))) : (void 0)))))))));
    return react.createElement("div", {
        className: "tab-pane active",
    }, react.createElement("h2", {
        className: "mb-4",
    }, title), (o = content, (o == null) ? null : (o_1 = o, o_1)));
}

export function view(model, dispatch) {
    let o, o_1;
    const navItems = map_1((t) => {
        const className = contentToClassName(t.Item);
        const label = typeName(t.Item);
        return new MenuItem(className, label, label, t.Range);
    }, model.Trivia);
    const onClick = (idx) => {
        dispatch(new Msg(3, new ActiveTab(1), idx));
    };
    const activeTrivia_1 = map((trivia) => activeTrivia(trivia), tryItem(model.ActiveByTriviaIndex, model.Trivia));
    return react.createElement("div", {
        className: "d-flex h-100",
    }, menu(onClick, model.ActiveByTriviaIndex, navItems), react.createElement("div", {
        className: "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto",
    }, (o = activeTrivia_1, (o == null) ? null : (o_1 = o, o_1))));
}

