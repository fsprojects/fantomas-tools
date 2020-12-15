import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, class_type } from "../fable-library.3.0.1/Reflection.js";
import * as react from "../../../../web_modules/react.js";
import { Form } from "../../../../web_modules/reactstrap.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { collect } from "../fable-library.3.0.1/Seq.js";

export class FormProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Custom"];
    }
}

export function FormProps$reflection() {
    return union_type("Reactstrap.Form.FormProps", [], FormProps, () => [[["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function form(props, elems) {
    return react.createElement(Form, keyValueList(collect((_arg1) => _arg1.fields[0], props), 1), ...elems);
}

