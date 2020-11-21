import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { InputGroupText } from "../../../../../web_modules/reactstrap.js";

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
    const props_1 = keyValueList(props, 1);
    return react.createElement(InputGroupText, props_1, ...elems);
}

