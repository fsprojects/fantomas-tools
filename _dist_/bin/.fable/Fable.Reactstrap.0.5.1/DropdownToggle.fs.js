import { Union } from "../fable-library.3.2.9/Types.js";
import { union_type, list_type, class_type, string_type, bool_type } from "../fable-library.3.2.9/Reflection.js";
import { keyValueList } from "../fable-library.3.2.9/MapUtil.js";
import { choose, collect } from "../fable-library.3.2.9/Seq.js";
import { empty } from "../fable-library.3.2.9/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { DropdownToggle } from "../../../../_snowpack/pkg/reactstrap.js";

export class DropdownToggleProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Caret", "Color", "data-toggle", "aria-haspopup", "Size", "Custom"];
    }
}

export function DropdownToggleProps$reflection() {
    return union_type("Reactstrap.DropdownToggle.DropdownToggleProps", [], DropdownToggleProps, () => [[["Item", bool_type]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function dropdownToggle(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 5) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 5) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(DropdownToggle, Object.assign({}, customProps, typeProps), ...elems);
}

