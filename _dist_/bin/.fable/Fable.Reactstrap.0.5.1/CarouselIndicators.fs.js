import { Union, Record } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, lambda_type, unit_type, int32_type, class_type, record_type, string_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { CarouselIndicators } from "../../../../_snowpack/pkg/reactstrap.js";

export class Item extends Record {
    constructor(Src, AltText, Caption) {
        super();
        this.Src = Src;
        this.AltText = AltText;
        this.Caption = Caption;
    }
}

export function Item$reflection() {
    return record_type("Reactstrap.Item", [], Item, () => [["Src", string_type], ["AltText", string_type], ["Caption", string_type]]);
}

export class CarouselIndicators_CarouselIndicatorsProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Items", "ActiveIndex", "CssModule", "OnClickHandler", "Custom"];
    }
}

export function CarouselIndicators_CarouselIndicatorsProps$reflection() {
    return union_type("Reactstrap.CarouselIndicators.CarouselIndicatorsProps", [], CarouselIndicators_CarouselIndicatorsProps, () => [[["Item", class_type("System.Collections.Generic.IEnumerable`1", [Item$reflection()])]], [["Item", int32_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", lambda_type(int32_type, unit_type)]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function CarouselIndicators_carouselIndicators(props, elems) {
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
    return react.createElement(CarouselIndicators, Object.assign({}, customProps, typeProps), ...elems);
}

