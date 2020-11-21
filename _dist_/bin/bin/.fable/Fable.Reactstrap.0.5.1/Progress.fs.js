import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type, string_type, bool_type, obj_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { choose, collect } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { empty } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { Progress } from "../../../../../web_modules/reactstrap.js";

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
    let customProps;
    let li;
    li = collect((_arg1) => {
        if (_arg1.tag === 9) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props);
    customProps = keyValueList(li, 1);
    let typeProps;
    let li_1;
    li_1 = choose((_arg2) => {
        if (_arg2.tag === 9) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props);
    typeProps = keyValueList(li_1, 1);
    const props_2 = Object.assign({}, customProps, typeProps);
    return react.createElement(Progress, props_2, ...elems);
}

