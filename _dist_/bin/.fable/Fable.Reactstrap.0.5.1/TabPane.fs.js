import { Union } from "../fable-library.3.1.1/Types.js";
import { union_type, list_type, class_type, obj_type } from "../fable-library.3.1.1/Reflection.js";
import { keyValueList } from "../fable-library.3.1.1/MapUtil.js";
import { choose, collect } from "../fable-library.3.1.1/Seq.js";
import { empty } from "../fable-library.3.1.1/List.js";
import * as react from "../../../../_snowpack/pkg/react.js";
import { TabPane } from "../../../../_snowpack/pkg/reactstrap.js";

export class TabPaneProps extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Tag", "TabId", "CssModule", "Custom"];
    }
}

export function TabPaneProps$reflection() {
    return union_type("Reactstrap.TabPane.TabPaneProps", [], TabPaneProps, () => [[["Item", obj_type]], [["Item", obj_type]], [["Item", class_type("Reactstrap.Common.CSSModule")]], [["Item", list_type(class_type("Fable.React.Props.IHTMLProp"))]]]);
}

export function tabPane(props, elems) {
    const customProps = keyValueList(collect((_arg1) => {
        if (_arg1.tag === 3) {
            return _arg1.fields[0];
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
            return _arg2;
        }
    }, props), 1);
    return react.createElement(TabPane, Object.assign({}, customProps, typeProps), ...elems);
}

