import { useFeliz_React__React_useElmish_Static_645B1FB7 } from "./.fable/Feliz.UseElmish.1.5.0/UseElmish.fs.js";
import { update, init } from "./State.js";
import { editor, navigation, tabs } from "./View.js";
import { createElement } from "../../_snowpack/pkg/react.js";
import * as react from "../../_snowpack/pkg/react.js";
import { RowProps, row } from "./.fable/Fable.Reactstrap.0.5.1/Row.fs.js";
import { HTMLAttr } from "./.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { ofArray } from "./.fable/fable-library.3.1.15/List.js";
import { RouterModule_router } from "./.fable/Feliz.Router.3.2.0/Router.fs.js";
import { parseUrl } from "./Navigation.js";
import { Msg } from "./Model.js";
import { render } from "../../_snowpack/pkg/react-dom.js";

export function App() {
    const patternInput = useFeliz_React__React_useElmish_Static_645B1FB7(init, (msg, model) => update(msg, model), []);
    const model_1 = patternInput[0];
    const dispatch = patternInput[1];
    const routes = tabs(model_1, dispatch);
    return react.createElement(react.Fragment, {}, navigation(dispatch), row([new RowProps(2, ofArray([new HTMLAttr(64, "no-gutters"), new HTMLAttr(99, "main")]))], [editor(model_1, dispatch), RouterModule_router({
        onUrlChanged: (url) => {
            dispatch(new Msg(0, parseUrl(url)));
        },
        application: react.createElement(react.Fragment, {}, routes),
    })]));
}

render(createElement(App, null), document.getElementById("app"));

