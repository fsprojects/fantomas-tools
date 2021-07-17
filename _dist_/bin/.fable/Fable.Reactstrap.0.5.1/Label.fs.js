import { Union } from "../fable-library.3.2.9/Types.js";
import { union_type, list_type, class_type, bool_type } from "../fable-library.3.2.9/Reflection.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Label } from "../../../../_snowpack/pkg/reactstrap.js";
import { keyValueList } from "../fable-library.3.2.9/MapUtil.js";

export class LabelProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Check", "Custom"];
    }
}

export function LabelProps$reflection() {
    return union_type("Reactstrap.Label.LabelProps", [], LabelProps, () => [[["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function label(props, elems) {
    return react.createElement(Label, keyValueList(props, 1), ...elems);
}

