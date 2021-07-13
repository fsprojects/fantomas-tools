import { Union } from "../fable-library.3.1.15/Types.js";
import { union_type, list_type, class_type, obj_type, bool_type, string_type } from "../fable-library.3.1.15/Reflection.js";
import { FadeProps$reflection } from "./Fade.fs.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Toast } from "../../../../_snowpack/pkg/reactstrap.js";
import { singleton, map, reduce, isEmpty } from "../fable-library.3.1.15/Seq.js";
import { keyValueList } from "../fable-library.3.1.15/MapUtil.js";

export class ToastProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Color", "IsOpen", "Tag", "Transition", "Custom"];
    }
}

export function ToastProps$reflection() {
    return union_type("Reactstrap.Toast.ToastProps", [], ToastProps, () => [[["Item", string_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", class_type("System.Collections.Generic.IEnumerable`1", [FadeProps$reflection()])]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function toast(props, elems) {
    return react.createElement(Toast, isEmpty(props) ? {} : reduce((a, b) => Object.assign(a, b), map((prop) => {
        switch (prop.tag) {
            case 3: {
                return {
                    transition: keyValueList(prop.fields[0], 1),
                };
            }
            case 4: {
                return keyValueList(prop.fields[0], 1);
            }
            default: {
                return keyValueList(singleton(prop), 1);
            }
        }
    }, props)), ...elems);
}

