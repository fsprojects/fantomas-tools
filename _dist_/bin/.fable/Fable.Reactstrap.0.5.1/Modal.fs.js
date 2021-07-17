import { Union } from "../fable-library.3.2.9/Types.js";
import { union_type, list_type, class_type, obj_type, lambda_type, unit_type, string_type, bool_type } from "../fable-library.3.2.9/Reflection.js";
import { FadeProps$reflection } from "./Fade.fs.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Modal } from "../../../../_snowpack/pkg/reactstrap.js";
import { singleton, map, reduce, isEmpty } from "../fable-library.3.2.9/Seq.js";
import { keyValueList } from "../fable-library.3.2.9/MapUtil.js";

export class ModalProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["IsOpen", "AutoFocus", "Centered", "Size", "Toggle", "Role", "LabelledBy", "Keyboard", "Backdrop", "Scrollable", "OnEnter", "OnExit", "OnOpened", "OnClosed", "WrapClassName", "ModalClassName", "BackdropClassName", "ContentClassName", "Fade", "CssModule", "ZIndex", "BackdropTransition", "ModalTransition", "InnerRef", "UnmountOnClose", "Custom"];
    }
}

export function ModalProps$reflection() {
    return union_type("Reactstrap.Modal.ModalProps", [], ModalProps, () => [[["Item", bool_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", obj_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", obj_type]], [["Item", class_type("System.Collections.Generic.IEnumerable`1", [FadeProps$reflection()])]], [["Item", class_type("System.Collections.Generic.IEnumerable`1", [FadeProps$reflection()])]], [["Item", lambda_type(class_type("Browser.Types.Element"), unit_type)]], [["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function modal(props, elems) {
    return react.createElement(Modal, isEmpty(props) ? {} : reduce((a, b) => Object.assign(a, b), map((prop) => {
        switch (prop.tag) {
            case 21: {
                return {
                    backdropTransition: keyValueList(prop.fields[0], 1),
                };
            }
            case 22: {
                return {
                    modalTransition: keyValueList(prop.fields[0], 1),
                };
            }
            default: {
                return keyValueList(singleton(prop), 1);
            }
        }
    }, props)), ...elems);
}

