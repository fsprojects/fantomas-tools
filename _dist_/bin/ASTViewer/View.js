import { createElement } from "../../../_snowpack/pkg/react.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { MonacoEditorProp, Editor } from "../Editor.js";
import { singleton } from "../.fable/fable-library.3.1.15/List.js";
import { bind } from "../.fable/fable-library.3.1.15/Option.js";
import { mapIndexed } from "../.fable/fable-library.3.1.15/Array.js";
import { printf, toText } from "../.fable/fable-library.3.1.15/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { DOMAttr, HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { isEmpty } from "../.fable/fable-library.3.1.15/Seq.js";
import { loader } from "../Loader.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { Msg } from "./Model.js";
import { versionBar } from "../VersionBar.js";
import { toggleButton, input } from "../SettingControls.js";

function results(model) {
    let result;
    const matchValue = model.Parsed;
    if (matchValue == null) {
        result = "";
    }
    else {
        const copyOfStruct = matchValue;
        result = ((copyOfStruct.tag === 1) ? createElement(Editor, {
            isReadOnly: true,
            props: singleton(new MonacoEditorProp(2, copyOfStruct.fields[0])),
        }) : createElement(Editor, {
            isReadOnly: true,
            props: singleton(new MonacoEditorProp(2, copyOfStruct.fields[0].String)),
        }));
    }
    let astErrors;
    const o_1 = bind((parsed_1) => {
        let pattern_matching_result;
        if (parsed_1.tag === 0) {
            if (!isEmpty(parsed_1.fields[0].Errors)) {
                pattern_matching_result = 0;
            }
            else {
                pattern_matching_result = 1;
            }
        }
        else {
            pattern_matching_result = 1;
        }
        switch (pattern_matching_result) {
            case 0: {
                return react.createElement("ul", {
                    id: "ast-errors",
                    className: "",
                }, mapIndexed((i, e_1) => react.createElement("li", {
                    key: toText(printf("ast-error-%i"))(i),
                }, react.createElement("strong", {}, toText(printf("(%i,%i) (%i, %i)"))(e_1.Range.StartLine)(e_1.Range.StartCol)(e_1.Range.EndLine)(e_1.Range.EndCol)), badge([new BadgeProps(1, (e_1.Severity === "warning") ? "warning" : "danger")], [e_1.Severity]), badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(158, "ErrorNumber")))], [e_1.ErrorNumber]), badge([new BadgeProps(1, "light"), new BadgeProps(5, singleton(new HTMLAttr(158, "SubCategory")))], [e_1.SubCategory]), react.createElement("p", {}, e_1.Message)), parsed_1.fields[0].Errors));
            }
            case 1: {
                return void 0;
            }
        }
    }, model.Parsed);
    astErrors = ((o_1 == null) ? null : o_1);
    return react.createElement("div", {
        id: "ast-content",
    }, react.createElement("div", {
        className: "ast-editor-container",
    }, result), astErrors);
}

export function view(model, dispatch) {
    if (model.IsLoading) {
        return loader;
    }
    else {
        return results(model);
    }
}

export function commands(dispatch) {
    return react.createElement(react.Fragment, {}, button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(2));
    })))], ["Show Untyped AST"]), button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg2) => {
        dispatch(new Msg(3));
    })))], ["Show Typed AST"]));
}

export function settings(model, dispatch) {
    return react.createElement(react.Fragment, {}, versionBar(toText(printf("FSC - %s"))(model.Version)), input("ast-defines", (arg) => {
        dispatch(new Msg(6, arg));
    }, "Defines", "Enter your defines separated with a space", model.Defines), toggleButton((_arg1) => {
        dispatch(new Msg(7, true));
    }, (_arg2) => {
        dispatch(new Msg(7, false));
    }, "*.fsi", "*.fs", "File extension", model.IsFsi));
}

