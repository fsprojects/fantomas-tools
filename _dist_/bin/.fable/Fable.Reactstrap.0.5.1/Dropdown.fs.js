import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, class_type, lambda_type, unit_type, string_type, bool_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Dropdown } from "../../../../_snowpack/pkg/reactstrap.js";

export class DropdownProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Disabled", "Direction", "Group", "IsOpen", "Nav", "Active", "InNavbar", "Toggle", "SetActiveFromChild", "Custom"];
    }
}

export function DropdownProps$reflection() {
    return union_type("Reactstrap.Dropdown.DropdownProps", [], DropdownProps, () => [[["Item", bool_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function dropdown(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 9) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 9) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Dropdown, Object.assign({}, customProps, typeProps), ...elems);
}
