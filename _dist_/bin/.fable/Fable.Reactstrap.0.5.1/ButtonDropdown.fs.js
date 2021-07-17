import { Union } from "../fable-library.3.2.9/Types.js";
import { union_type, list_type, class_type, lambda_type, unit_type, string_type, bool_type, obj_type } from "../fable-library.3.2.9/Reflection.js";
import { keyValueList } from "../fable-library.3.2.9/MapUtil.js";
import { choose, collect } from "../fable-library.3.2.9/Seq.js";
import { empty } from "../fable-library.3.2.9/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { ButtonDropdown } from "../../../../_snowpack/pkg/reactstrap.js";

export class ButtonDropdownProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Disabled", "Direction", "Group", "IsOpen", "Toggle", "Custom"];
    }
}

export function ButtonDropdownProps$reflection() {
    return union_type("Reactstrap.ButtonDropdown.ButtonDropdownProps", [], ButtonDropdownProps, () => [[["Item", obj_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function buttonDropdown(props, elems) {
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
    return react.createElement(ButtonDropdown, Object.assign({}, customProps, typeProps), ...elems);
}

