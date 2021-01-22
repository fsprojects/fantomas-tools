import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, class_type, lambda_type, unit_type, bool_type, obj_type, string_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Tooltip } from "../../../../_snowpack/pkg/reactstrap.js";

export class TooltipProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Trigger", "BoundariesElement", "IsOpen", "HideArrow", "Toggle", "Target", "Container", "Delay", "InnerClassName", "ArrowClassName", "Autohide", "Placement", "Modifiers", "Offset", "InnerRef", "Fade", "Flip", "Custom"];
    }
}

export function TooltipProps$reflection() {
    return union_type("Reactstrap.Tooltip.TooltipProps", [], TooltipProps, () => [[["Item", string_type]], [["Item", obj_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", obj_type]], [["Item", obj_type]], [["Item", obj_type]], [["Item", string_type]], [["Item", string_type]], [["Item", bool_type]], [["Item", string_type]], [["Item", obj_type]], [["Item", obj_type]], [["Item", lambda_type(class_type("Browser.Types.Element"), unit_type)]], [["Item", bool_type]], [["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function tooltip(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 17) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 17) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Tooltip, Object.assign({}, customProps, typeProps), ...elems);
}

