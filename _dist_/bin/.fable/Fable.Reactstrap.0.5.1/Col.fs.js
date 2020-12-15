import { Union, Record } from "../fable-library.3.0.1/Types.js";
import { union_type, list_type, class_type, array_type, record_type, obj_type } from "../fable-library.3.0.1/Reflection.js";
import { keyValueList } from "../fable-library.3.0.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.0.1/Seq.js";
import { empty } from "../fable-library.3.0.1/List.js";
import * as react from "../../../../web_modules/react.js";
import { Col } from "../../../../web_modules/reactstrap.js";

export class ColumnProps extends Record {
    constructor(size, order, offset) {
        super();
        this.size = size;
        this.order = order;
        this.offset = offset;
    }
}

export function ColumnProps$reflection() {
    return record_type("Reactstrap.Col.ColumnProps", [], ColumnProps, () => [["size", obj_type], ["order", obj_type], ["offset", obj_type]]);
}

export function mkCol(size) {
    return new ColumnProps(size, 0, 0);
}

export function withOffset(offset, col_1) {
    return new ColumnProps(col_1.size, col_1.order, offset);
}

export function withOrder(order, col_1) {
    return new ColumnProps(col_1.size, order, col_1.offset);
}

export class ColProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "Xs", "Sm", "Md", "Lg", "Xl", "Widths", "Custom"];
    }
}

export function ColProps$reflection() {
    return union_type("Reactstrap.Col.ColProps", [], ColProps, () => [[["Item", obj_type]], [["Item", ColumnProps$reflection()]], [["Item", ColumnProps$reflection()]], [["Item", ColumnProps$reflection()]], [["Item", ColumnProps$reflection()]], [["Item", ColumnProps$reflection()]], [["Item", array_type(obj_type)]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function col(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 7) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props), 1);
    const typeProps = keyValueList(choose((_arg2) => {
        if (_arg2.tag === 7) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props), 1);
    return react.createElement(Col, Object.assign({}, customProps, typeProps), ...elems);
}

