import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, lambda_type, unit_type, string_type, class_type, obj_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../web_modules/react.js";
import { ToastHeader } from "../../../../web_modules/reactstrap.js";

export class ToastHeaderProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "CssModule", "WrapTag", "Toggle", "Icon", "Close", "CharCode", "CloseAriaLabel", "Custom"];
    }
}

export function ToastHeaderProps$reflection() {
    return union_type("Reactstrap.ToastHeader.ToastHeaderProps", [], ToastHeaderProps, () => [[["Item", obj_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", string_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", obj_type]], [["Item", class_type("Fable.React.ReactElement")]], [["Item", obj_type]], [["Item", string_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function toastHeader(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 8) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 8) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(ToastHeader, Object.assign({}, customProps, typeProps), ...elems);
}

