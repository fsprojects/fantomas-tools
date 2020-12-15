import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, obj_type, lambda_type, unit_type, int32_type, class_type, string_type, bool_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../web_modules/react.js";
import { Collapse } from "../../../../web_modules/reactstrap.js";

export class CollapseProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["In", "BaseClass", "BaseClassIn", "CssModule", "TransitionAppearTimeout", "TransitionEnterTimeout", "TransitionLeaveTimeout", "TransitionAppear", "TransitionEnter", "TransitionLeave", "OnLeave", "OnEnter", "IsOpen", "InnerRef", "Navbar", "OnEntering", "OnEntered", "OnExiting", "OnExited", "Tag", "Custom"];
    }
}

export function CollapseProps$reflection() {
    return union_type("Reactstrap.Collapse.CollapseProps", [], CollapseProps, () => [[["Item", bool_type]], [["Item", string_type]], [["Item", string_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", int32_type]], [["Item", int32_type]], [["Item", int32_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", bool_type]], [["Item", lambda_type(class_type("Browser.Types.Element"), unit_type)]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", obj_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function collapse(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 20) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 20) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Collapse, Object.assign({}, customProps, typeProps), ...elems);
}

