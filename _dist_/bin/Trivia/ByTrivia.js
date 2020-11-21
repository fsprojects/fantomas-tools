import { printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { menu, MenuItem, rangeToText } from "./Menu.js";
import { map } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import * as react from "../../../web_modules/react.js";
import { tryItem, map as map_1 } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { ActiveTab, Msg } from "./Model.js";

function contentToClassName(c) {
    const arg10 = (c.tag === 1) ? "number-content" : ((c.tag === 2) ? "string-content" : ((c.tag === 9) ? "char-content" : ((c.tag === 3) ? "ident-operator-keyword" : ((c.tag === 4) ? "ident-between-ticks" : ((c.tag === 5) ? "comment" : ((c.tag === 6) ? "newline" : ((c.tag === 7) ? "directive" : ((c.tag === 8) ? "newline-after" : "keyword"))))))));
    const clo1 = toText(printf("nav-link-%s"));
    return clo1(arg10);
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
            const clo1 = toText(printf("Comment(%s)"));
            return clo1(arg10);
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
        default: {
            return "Keyword";
        }
    }
}

function activeTrivia(trivia) {
    let o, o_1;
    let title;
    const arg10 = typeName(trivia.Item);
    const arg20 = rangeToText(trivia.Range);
    const clo1 = toText(printf("%s %s"));
    const clo2 = clo1(arg10);
    title = clo2(arg20);
    let content;
    let option;
    const matchValue = trivia.Item;
    let pattern_matching_result, i;
    switch (matchValue.tag) {
        case 1: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 2: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 9: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 3: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 4: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 7: {
            pattern_matching_result = 0;
            i = matchValue.fields[0];
            break;
        }
        case 5: {
            pattern_matching_result = 1;
            break;
        }
        default: pattern_matching_result = 2}
    switch (pattern_matching_result) {
        case 0: {
            option = i;
            break;
        }
        case 1: {
            const c = matchValue.fields[0];
            let pattern_matching_result_1, c_1;
            switch (c.tag) {
                case 1: {
                    pattern_matching_result_1 = 0;
                    c_1 = c.fields[0];
                    break;
                }
                case 2: {
                    pattern_matching_result_1 = 1;
                    break;
                }
                default: {
                    pattern_matching_result_1 = 0;
                    c_1 = c.fields[0];
                }
            }
            switch (pattern_matching_result_1) {
                case 0: {
                    option = c_1;
                    break;
                }
                case 1: {
                    let arg0;
                    const clo1_1 = toText(printf("%s (newline before: %b) (newline after: %b)"));
                    const clo2_1 = clo1_1(c.fields[0]);
                    const clo3 = clo2_1(c.fields[1]);
                    arg0 = clo3(c.fields[2]);
                    option = arg0;
                    break;
                }
            }
            break;
        }
        case 2: {
            option = (void 0);
            break;
        }
    }
    content = map((c_3) => react.createElement("code", {}, c_3), option);
    const children_4 = [react.createElement("h2", {
        className: "mb-4",
    }, title), (o = content, (o == null) ? null : (o_1 = o, o_1))];
    return react.createElement("div", {
        className: "tab-pane active",
    }, ...children_4);
}

export function view(model, dispatch) {
    let children, o, o_1;
    let navItems;
    navItems = map_1((t) => {
        const className = contentToClassName(t.Item);
        const label = typeName(t.Item);
        return new MenuItem(className, label, label, t.Range);
    }, model.Trivia);
    let activeTrivia_1;
    const option = tryItem(model.ActiveByTriviaIndex, model.Trivia);
    activeTrivia_1 = map(activeTrivia, option);
    const children_2 = [menu((idx) => {
        dispatch(new Msg(3, new ActiveTab(1), idx));
    }, model.ActiveByTriviaIndex, navItems), (children = [(o = activeTrivia_1, (o == null) ? null : (o_1 = o, o_1))], react.createElement("div", {
        className: "bg-light flex-grow-1 py-2 px-4 tab-content overflow-auto",
    }, ...children))];
    return react.createElement("div", {
        className: "d-flex h-100",
    }, ...children_2);
}

