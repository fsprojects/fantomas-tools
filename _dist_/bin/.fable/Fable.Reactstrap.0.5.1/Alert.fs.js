import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, lambda_type, unit_type, bool_type, string_type, class_type, obj_type } from "../fable-library.3.0.1/Reflection.js";
import { TransitionProps$reflection } from "./Common.fs.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Alert } from "../../../../_snowpack/pkg/reactstrap.js";

export class AlertProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "CSSModule", "Color", "Transition", "IsOpen", "Toggle", "Custom"];
    }
}

export function AlertProps$reflection() {
    return union_type("Reactstrap.Alert.AlertProps", [], AlertProps, () => [[["Item", obj_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", string_type]], [["Item", TransitionProps$reflection()]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function alert(props, elems) {
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
    return react.createElement(Alert, Object.assign({}, customProps, typeProps), ...elems);
}

