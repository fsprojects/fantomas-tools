import { Union } from "../fable-library.3.1.7/Types.js";
import { union_type, list_type, class_type, bool_type, obj_type } from "../fable-library.3.1.7/Reflection.js";
import { keyValueList } from "../fable-library.3.1.7/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.7/Seq.js";
import { empty } from "../fable-library.3.1.7/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { CardImg } from "../../../../_snowpack/pkg/reactstrap.js";

export class CardImgProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Top", "Bottom", "Custom"];
    }
}

export function CardImgProps$reflection() {
    return union_type("Reactstrap.CardImg.CardImgProps", [], CardImgProps, () => [[["Item", obj_type]], [["Item", bool_type]], [["Item", bool_type]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function cardImg(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 3) {
            const props_1 = _arg1.fields[0];
            return props_1;
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 3) {
            return void 0;
        }
        else {
            const prop = _arg2;
            return prop;
        }
    }, props), 1);
    const props_2 = Object.assign({}, customProps, typeProps);
    return react.createElement(CardImg, props_2, ...elems);
}

