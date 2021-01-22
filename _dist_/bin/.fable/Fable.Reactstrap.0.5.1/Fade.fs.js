import { Union, Record } from "../fable-library.3.1.1/Types.js";
import { union_type, lambda_type, unit_type, class_type, string_type, obj_type, bool_type, record_type, int32_type } from "../fable-library.3.1.1/Reflection.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Fade } from "../../../../_snowpack/pkg/reactstrap.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";

export class TimeoutEx extends Record {
    constructor(enter, exit) {
        super();
        this.enter = (enter | 0);
        this.exit = (exit | 0);
    }
}

export function TimeoutEx$reflection() {
    return record_type("Reactstrap.Fade.TimeoutEx", [], TimeoutEx, () => [["enter", int32_type], ["exit", int32_type]]);
}

export class FadeProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["In", "MountOnEnter", "UnmountOnExit", "Appear", "Enter", "Exit", "Timeout", "AddEndListener", "OnEnter", "OnEntering", "OnEntered", "OnExit", "OnExiting", "OnExited", "BaseClass"];
    }
}

export function FadeProps$reflection() {
    return union_type("Reactstrap.Fade.FadeProps", [], FadeProps, () => [[["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", lambda_type(string_type, lambda_type(class_type("Browser.Types.Event"), unit_type))]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", string_type]]]);
}

export function fade(props, elems) {
    return react.createElement(Fade, keyValueList(props, 1), ...elems);
}

