import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, class_type, string_type, bool_type, obj_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Navbar } from "../../../../_snowpack/pkg/reactstrap.js";

export class NavbarProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Light", "Dark", "Fixed", "Color", "Role", "Expand", "Custom"];
    }
}

export function NavbarProps$reflection() {
    return union_type("Reactstrap.Navbar.NavbarProps", [], NavbarProps, () => [[["Item", obj_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", obj_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function navbar(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 7) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 7) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Navbar, Object.assign({}, customProps, typeProps), ...elems);
}

