import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, class_type } from "../fable-library.3.0.1/Reflection.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { InputGroupText } from "../../../../_snowpack/pkg/reactstrap.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";

export class InputGroupTextProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Custom"];
    }
}

export function InputGroupTextProps$reflection() {
    return union_type("Reactstrap.InputGroupText.InputGroupTextProps", [], InputGroupTextProps, () => [[["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function inputGroupText(props, elems) {
    return react.createElement(InputGroupText, keyValueList(props, 1), ...elems);
}

