import { useFeliz_React__React_useElmish_Static_645B1FB7 } from "./bin/.fable/Feliz.UseElmish.1.5.0/UseElmish.fs.js";
import { update, init } from "./State.js";
import { editor, navigation, tabs } from "./View.js";
import { RowProps, row } from "./bin/.fable/Fable.Reactstrap.0.5.1/Row.fs.js";
import { HTMLAttr } from "./bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { ofArray } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { parseUrl } from "./Navigation.js";
import { Msg } from "./Model.js";
import { createElement } from "../../web_modules/react.js";
import * as react from "../../web_modules/react.js";
import { RouterModule_router } from "./bin/.fable/Feliz.Router.3.2.0/Router.fs.js";
import { createObj } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { render } from "../../web_modules/react-dom.js";

export function App() {
    let props_1;
    const patternInput = useFeliz_React__React_useElmish_Static_645B1FB7(init, update, []);
    const model_1 = patternInput[0];
    const dispatch = patternInput[1];
    const routes = tabs(model_1, dispatch);
    const children_1 = [navigation(dispatch), row([new RowProps(2, ofArray([new HTMLAttr(64, "no-gutters"), new HTMLAttr(99, "main")]))], [editor(model_1, dispatch), (props_1 = ofArray([["onUrlChanged", (url) => {
        const activeTab = parseUrl(url);
        dispatch(new Msg(0, activeTab));
    }], ["application", react.createElement(react.Fragment, {}, routes)]]), RouterModule_router(createObj(props_1)))])];
    return react.createElement(react.Fragment, {}, ...children_1);
}

render(createElement(App, null), document.getElementById("app"));

