import { Union } from "../fable-library.3.1.7/Types.js";
import { union_type, list_type, class_type } from "../fable-library.3.1.7/Reflection.js";
import { keyValueList } from "../fable-library.3.1.7/MapUtil.js";
import { collect } from "../fable-library.3.1.7/Seq.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Form } from "../../../../_snowpack/pkg/reactstrap.js";

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
    const props_2 = keyValueList(collect((_arg1) => {
        const props_1 = _arg1.fields[0];
        return props_1;
    }, props), 1);
    return react.createElement(Form, props_2, ...elems);
}

