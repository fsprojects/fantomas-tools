import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type, obj_type, lambda_type, unit_type, string_type, bool_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { FadeProps$reflection } from "./Fade.fs.js";
import { reduce, singleton, map, isEmpty } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { Modal } from "../../../../../web_modules/reactstrap.js";

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
    let modalProps;
    if (isEmpty(props)) {
        modalProps = {};
    }
    else {
        let source_1;
        source_1 = map((prop) => {
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
        }, props);
        modalProps = reduce((a, b) => Object.assign(a, b), source_1);
    }
    return react.createElement(Modal, modalProps, ...elems);
}

