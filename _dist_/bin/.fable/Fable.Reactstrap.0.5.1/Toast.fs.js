import { Union } from "../fable-library.3.1.7/Types.js";
import { union_type, list_type, class_type, obj_type, bool_type, string_type } from "../fable-library.3.1.7/Reflection.js";
import { FadeProps$reflection } from "./Fade.fs.js";
import { singleton, map, reduce, isEmpty } from "../fable-library.3.1.7/Seq.js";
import { keyValueList } from "../fable-library.3.1.7/MapUtil.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Toast } from "../../../../_snowpack/pkg/reactstrap.js";

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
    const toastProps = isEmpty(props) ? {} : reduce((a, b) => Object.assign(a, b), map((prop) => {
        switch (prop.tag) {
            case 3: {
                const fade = prop.fields[0];
                return {
                    transition: keyValueList(fade, 1),
                };
            }
            case 4: {
                const customProps = prop.fields[0];
                return keyValueList(customProps, 1);
            }
            default: {
                const prop_1 = prop;
                return keyValueList(singleton(prop_1), 1);
            }
        }
    }, props));
    return react.createElement(Toast, toastProps, ...elems);
}

