import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { UncontrolledDropdown } from "../../../../../web_modules/reactstrap.js";

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
    const props_1 = keyValueList(props, 1);
    return react.createElement(UncontrolledDropdown, props_1, ...elems);
}

