import { class_type } from "../fable-library.3.1.15/Reflection.js";
import { render } from "../../../../_snowpack/pkg/react-dom.js";

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

