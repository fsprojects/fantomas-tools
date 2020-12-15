import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, obj_type, string_type, class_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../web_modules/react.js";
import { Pagination } from "../../../../web_modules/reactstrap.js";

export class PaginationProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["CssModule", "Size", "ListClassName", "ListTag", "aria-label", "Tag", "Custom"];
    }
}

export function PaginationProps$reflection() {
    return union_type("Reactstrap.Pagination.PaginationProps", [], PaginationProps, () => [[["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", obj_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function pagination(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 6) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 6) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Pagination, Object.assign({}, customProps, typeProps), ...elems);
}

