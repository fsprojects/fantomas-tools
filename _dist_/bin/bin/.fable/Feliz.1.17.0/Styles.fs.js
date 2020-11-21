import { class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { mkStyle } from "./Interop.fs.js";

export class styleModule_listStyleImage {
    constructor() {
    }
}

export function styleModule_listStyleImage$reflection() {
    return class_type("Feliz.styleModule.listStyleImage", void 0, styleModule_listStyleImage);
}

export function styleModule_emptyCells_get_show() {
    return mkStyle("emptyCells", "show");
}

export function styleModule_emptyCells_get_hide() {
    return mkStyle("emptyCells", "hide");
}

export function styleModule_emptyCells_get_initial() {
    return mkStyle("emptyCells", "initial");
}

export function styleModule_emptyCells_get_inheritFromParent() {
    return mkStyle("emptyCells", "inherit");
}

