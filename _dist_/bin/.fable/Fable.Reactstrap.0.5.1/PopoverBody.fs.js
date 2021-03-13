import { Union } from "../fable-library.3.1.7/Types.js";
import { union_type, list_type, class_type } from "../fable-library.3.1.7/Reflection.js";
import { keyValueList } from "../fable-library.3.1.7/MapUtil.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { PopoverBody } from "../../../../_snowpack/pkg/reactstrap.js";

export class PopoverBodyProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Custom"];
    }
}

export function PopoverBodyProps$reflection() {
    return union_type("Reactstrap.PopoverBody.PopoverBodyProps", [], PopoverBodyProps, () => [[["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function popoverBody(props, elems) {
    const props_1 = keyValueList(props, 1);
    return react.createElement(PopoverBody, props_1, ...elems);
}

