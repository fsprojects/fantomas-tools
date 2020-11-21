import { Union, Record } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { union_type, list_type, class_type, array_type, record_type, obj_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { choose, collect } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { empty } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { keyValueList } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import * as react from "../../../../../web_modules/react.js";
import { Col } from "../../../../../web_modules/reactstrap.js";

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
    let customProps;
    let li;
    li = collect((_arg1) => {
        if (_arg1.tag === 7) {
            return _arg1.fields[0];
        }
        else {
            return empty();
        }
    }, props);
    customProps = keyValueList(li, 1);
    let typeProps;
    let li_1;
    li_1 = choose((_arg2) => {
        if (_arg2.tag === 7) {
            return void 0;
        }
        else {
            return _arg2;
        }
    }, props);
    typeProps = keyValueList(li_1, 1);
    const props_2 = Object.assign({}, customProps, typeProps);
    return react.createElement(Col, props_2, ...elems);
}

