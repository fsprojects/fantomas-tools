import { Union } from "../fable-library.3.1.15/Types.js";
import { union_type, list_type, class_type, string_type } from "../fable-library.3.1.15/Reflection.js";
import { keyValueList } from "../fable-library.3.1.15/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.15/Seq.js";
import { empty } from "../fable-library.3.1.15/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Spinner } from "../../../../_snowpack/pkg/reactstrap.js";

export class SpinnerProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Type", "Size", "Color", "CssModule", "Custom"];
    }
}

export function SpinnerProps$reflection() {
    return union_type("Reactstrap.Spinner.SpinnerProps", [], SpinnerProps, () => [[["Item", string_type]], [["Item", string_type]], [["Item", string_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function spinner(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 4) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 4) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Spinner, Object.assign({}, customProps, typeProps), ...elems);
}

