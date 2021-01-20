import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, lambda_type, unit_type, class_type, bool_type, string_type, obj_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Button } from "../../../../_snowpack/pkg/reactstrap.js";

export class ButtonProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Color", "Outline", "Active", "Block", "InnerRef", "Size", "CSSModule", "Close", "Custom"];
    }
}

export function ButtonProps$reflection() {
    return union_type("Reactstrap.Button.ButtonProps", [], ButtonProps, () => [[["Item", obj_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(class_type("Browser.Types.Element"), unit_type)]], [["Item", string_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function button(props, elems) {
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
    return react.createElement(Button, Object.assign({}, customProps, typeProps), ...elems);
}

