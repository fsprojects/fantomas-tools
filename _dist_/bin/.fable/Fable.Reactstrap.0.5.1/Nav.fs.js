import { Union } from "../fable-library.3.1.15/Types.js";
import { union_type, list_type, class_type, string_type, obj_type, bool_type } from "../fable-library.3.1.15/Reflection.js";
import { keyValueList } from "../fable-library.3.1.15/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.15/Seq.js";
import { empty } from "../fable-library.3.1.15/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Nav } from "../../../../_snowpack/pkg/reactstrap.js";

export class NavProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tabs", "Pills", "Card", "Justified", "Fill", "Vertical", "Horizontal", "Navbar", "Tag", "Custom"];
    }
}

export function NavProps$reflection() {
    return union_type("Reactstrap.Nav.NavProps", [], NavProps, () => [[["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function nav(props, elems) {
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
    return react.createElement(Nav, Object.assign({}, customProps, typeProps), ...elems);
}

