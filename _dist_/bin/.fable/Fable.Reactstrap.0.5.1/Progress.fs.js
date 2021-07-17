import { Union } from "../fable-library.3.2.9/Types.js";
import { union_type, list_type, class_type, string_type, bool_type, obj_type } from "../fable-library.3.2.9/Reflection.js";
import { keyValueList } from "../fable-library.3.2.9/MapUtil.js";
import { choose, collect } from "../fable-library.3.2.9/Seq.js";
import { empty } from "../fable-library.3.2.9/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Progress } from "../../../../_snowpack/pkg/reactstrap.js";

export class ProgressProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Bar", "Value", "Max", "Animated", "Multi", "Striped", "Color", "BarClassName", "Custom"];
    }
}

export function ProgressProps$reflection() {
    return union_type("Reactstrap.Progress.ProgressProps", [], ProgressProps, () => [[["Item", obj_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", obj_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", string_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function progress(props, elems) {
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
    return react.createElement(Progress, Object.assign({}, customProps, typeProps), ...elems);
}

