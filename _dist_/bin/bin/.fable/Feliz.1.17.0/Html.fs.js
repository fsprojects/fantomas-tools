import { reactApi, reactElement } from "./Interop.fs.js";

export function Html_h1_C6540BC(children) {
    return reactElement("h1", {
        children: reactApi.Children.toArray(Array.from(children)),
    });
}

