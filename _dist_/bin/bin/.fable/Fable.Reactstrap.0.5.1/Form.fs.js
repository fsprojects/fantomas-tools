import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { collect } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { Form } from "../../../../../web_modules/reactstrap.js";

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
    let props_2;
    let li;
    li = collect((_arg1) => _arg1.fields[0], props);
    props_2 = keyValueList(li, 1);
    return react.createElement(Form, props_2, ...elems);
}

