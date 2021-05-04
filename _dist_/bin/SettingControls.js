import { formGroup } from "./.fable/Fable.Reactstrap.0.5.1/FormGroup.fs.js";
import * as react from "../../_snowpack/pkg/react.js";
import { InputProps, input as input_1 } from "./.fable/Fable.Reactstrap.0.5.1/Input.fs.js";
import { Prop, DOMAttr, HTMLAttr } from "./.fable/Fable.React.7.4.0/Fable.React.Props.fs.js";
import { Browser_Types_Event__Event_get_Value } from "./.fable/Fable.React.7.4.0/Fable.React.Extensions.fs.js";
import { toArray, map, singleton, ofArray } from "./.fable/fable-library.3.1.15/List.js";
import { ButtonProps, button } from "./.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { ButtonGroupProps, buttonGroup } from "./.fable/Fable.Reactstrap.0.5.1/ButtonGroup.fs.js";
import { Record } from "./.fable/fable-library.3.1.15/Types.js";
import { record_type, bool_type, lambda_type, unit_type, obj_type, string_type } from "./.fable/fable-library.3.1.15/Reflection.js";

export function input(key, onChange, labelValue, placeholder, value) {
    return formGroup([], [react.createElement("label", {}, labelValue), input_1([new InputProps(13, ofArray([new HTMLAttr(128, placeholder), new DOMAttr(9, (ev) => {
        onChange(Browser_Types_Event__Event_get_Value(ev));
    }), new HTMLAttr(1, value), new Prop(0, key)]))])]);
}

function toggleButton_(onClick, active, label) {
    return button([new ButtonProps(9, ofArray([new HTMLAttr(64, active ? "rounded-0 text-white" : "rounded-0"), new Prop(0, label), new DOMAttr(40, onClick)])), new ButtonProps(2, !active)], [label]);
}

export function toggleButton(onTrue, onFalse, labelTrue, labelFalse, labelValue, value) {
    return formGroup([], [react.createElement("label", {}, labelValue), react.createElement("br", {}), buttonGroup([new ButtonGroupProps(4, singleton(new HTMLAttr(64, "btn-group-toggle rounded-0")))], [toggleButton_(onTrue, value, labelTrue), toggleButton_(onFalse, !value, labelFalse)])]);
}

export class MultiButtonSettings extends Record {
    constructor(Label, OnClick, IsActive) {
        super();
        this.Label = Label;
        this.OnClick = OnClick;
        this.IsActive = IsActive;
    }
}

export function MultiButtonSettings$reflection() {
    return record_type("FantomasTools.Client.SettingControls.MultiButtonSettings", [], MultiButtonSettings, () => [["Label", string_type], ["OnClick", lambda_type(obj_type, unit_type)], ["IsActive", bool_type]]);
}

export function multiButton(labelValue, options) {
    const buttons = map((_arg1) => toggleButton_((arg00) => {
        _arg1.OnClick(arg00);
    }, _arg1.IsActive, _arg1.Label), options);
    return formGroup([], [react.createElement("label", {}, labelValue), react.createElement("br", {}), buttonGroup([new ButtonGroupProps(4, singleton(new HTMLAttr(64, "btn-group-toggle rounded-0")))], [toArray(buttons)])]);
}

