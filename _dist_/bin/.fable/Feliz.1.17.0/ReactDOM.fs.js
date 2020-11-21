import { class_type } from "../docs/.fable/fable-library.3.0.0-nagareyama-rc-007/Reflection.js";
import { render } from "../../../../web_modules/react-dom.js";

export class ReactDOM {
    constructor() {
    }
}

export function ReactDOM$reflection() {
    return class_type("Feliz.ReactDOM", void 0, ReactDOM);
}

export function ReactDOM_render_Z3D10464(element, container) {
    return render(element(), container);
}

export class ReactDOMServer {
    constructor() {
    }
}

export function ReactDOMServer$reflection() {
    return class_type("Feliz.ReactDOMServer", void 0, ReactDOMServer);
}

