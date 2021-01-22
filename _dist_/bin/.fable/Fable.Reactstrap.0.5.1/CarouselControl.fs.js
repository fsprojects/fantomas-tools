import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, lambda_type, unit_type, class_type, string_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { CarouselControl } from "../../../../_snowpack/pkg/reactstrap.js";

export class CarouselControlProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Direction", "OnClickHandler", "DirectionText", "CssModule", "Custom"];
    }
}

export function CarouselControlProps$reflection() {
    return union_type("Reactstrap.CarouselControl.CarouselControlProps", [], CarouselControlProps, () => [[["Item", string_type]], [["Item", lambda_type(class_type("Browser.Types.MouseEvent"), unit_type)]], [["Item", string_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function carouselControl(props, elems) {
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
    return react.createElement(CarouselControl, Object.assign({}, customProps, typeProps), ...elems);
}

