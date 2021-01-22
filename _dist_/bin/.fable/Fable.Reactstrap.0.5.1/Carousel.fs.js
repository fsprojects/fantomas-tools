import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, class_type, string_type, obj_type, bool_type, lambda_type, unit_type, int32_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { Carousel } from "../../../../_snowpack/pkg/reactstrap.js";

export class CarouselProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["ActiveIndex", "Next", "Previous", "Keyboard", "Pause", "Ride", "Interval", "MouseEnter", "MouseLeave", "Slide", "CssModule", "Custom"];
    }
}

export function CarouselProps$reflection() {
    return union_type("Reactstrap.Carousel.CarouselProps", [], CarouselProps, () => [[["Item", int32_type]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", lambda_type(unit_type, unit_type)]], [["Item", bool_type]], [["Item", obj_type]], [["Item", string_type]], [["Item", obj_type]], [["Item", lambda_type(class_type("Browser.Types.MouseEvent"), unit_type)]], [["Item", lambda_type(class_type("Browser.Types.MouseEvent"), unit_type)]], [["Item", bool_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function carousel(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 11) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 11) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Carousel, Object.assign({}, customProps, typeProps), ...elems);
}

