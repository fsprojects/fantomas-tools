import { Union } from "../fable-library.3.1.7/Types.js";
import { union_type, list_type, class_type, string_type, bool_type, obj_type } from "../fable-library.3.1.7/Reflection.js";
import { keyValueList } from "../fable-library.3.1.7/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.7/Seq.js";
import { empty } from "../fable-library.3.1.7/List.js";
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
            const props_1 = _arg1.fields[0];
            return props_1;
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
            const prop = _arg2;
            return prop;
        }
    }, props), 1);
    const props_2 = Object.assign({}, customProps, typeProps);
    return react.createElement(Progress, props_2, ...elems);
}

