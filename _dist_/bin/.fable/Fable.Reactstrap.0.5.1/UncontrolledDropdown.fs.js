import { Union } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, class_type } from "../fable-library.3.0.1/Reflection.js";
import * as react from "../../../../web_modules/react.js";
import { UncontrolledDropdown } from "../../../../web_modules/reactstrap.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";

export class UncontrolledProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Custom"];
    }
}

export function UncontrolledProps$reflection() {
    return union_type("Reactstrap.UncontrolledDropdown.UncontrolledProps", [], UncontrolledProps, () => [[["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function uncontrolledDropdown(props, elems) {
    return react.createElement(UncontrolledDropdown, keyValueList(props, 1), ...elems);
}

