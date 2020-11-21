import { reactApi, reactElement } from "./Interop.fs.js";
import { createObj } from "../docs/.fable/fable-library.3.0.0-nagareyama-rc-007/Util.js";
import { singleton } from "../docs/.fable/fable-library.3.0.0-nagareyama-rc-007/List.js";

export function Html_h1_C6540BC(children) {
    return reactElement("h1", createObj(singleton(["children", reactApi.Children.toArray(Array.from(children))])));
}

