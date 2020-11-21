import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type, obj_type, bool_type, string_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { FadeProps$reflection } from "./Fade.fs.js";
import { reduce, singleton, map, isEmpty } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { Toast } from "../../../../../web_modules/reactstrap.js";

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
    let toastProps;
    if (isEmpty(props)) {
        toastProps = {};
    }
    else {
        let source_1;
        source_1 = map((prop) => {
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
        }, props);
        toastProps = reduce((a, b) => Object.assign(a, b), source_1);
    }
    return react.createElement(Toast, toastProps, ...elems);
}

