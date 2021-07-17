import { Record, Union } from "../fable-library.3.2.9/Types.js";
import { record_type, union_type, lambda_type, unit_type, int32_type, class_type, string_type, bool_type } from "../fable-library.3.2.9/Reflection.js";

export class TransitionProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["in", "baseClass", "baseClassIn", "cssModule", "transitionAppearTimeout", "transitionEnterTimeout", "transitionLeaveTimeout", "transitionAppear", "transitionEnter", "transitionLeave", "onLeave", "onEnter"];
    }
}

export function TransitionProps$reflection() {
    return union_type("Reactstrap.Common.TransitionProps", [], TransitionProps, () => [[["Item", bool_type]], [["Item", string_type]], [["Item", string_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", int32_type]], [["Item", int32_type]], [["Item", int32_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]]]);
}

export class DelayEx extends Record {
    constructor(show, hide) {
        super();
        this.show = (show | 0);
        this.hide = (hide | 0);
    }
}

export function DelayEx$reflection() {
    return record_type("Reactstrap.Common.DelayEx", [], DelayEx, () => [["show", int32_type], ["hide", int32_type]]);
}

