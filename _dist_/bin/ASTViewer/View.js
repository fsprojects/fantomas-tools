import * as react from "../../../_snowpack/pkg/react.js";
import Editor from "../../js/Editor.js";
import { bind } from "../.fable/fable-library.3.1.7/Option.js";
import { mapIndexed } from "../.fable/fable-library.3.1.7/Array.js";
import { printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { DOMAttr, HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { singleton } from "../.fable/fable-library.3.1.7/List.js";
import { isEmpty } from "../.fable/fable-library.3.1.7/Seq.js";
import { loader } from "../Loader.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { Msg } from "./Model.js";
import { versionBar } from "../VersionBar.js";
import { toggleButton, input } from "../SettingControls.js";

function results(model, dispatch) {
    let errors, parsed;
    let result;
    const matchValue = model.Parsed;
    if (matchValue == null) {
        result = "";
    }
    else {
        const copyOfStruct = matchValue;
        result = ((copyOfStruct.tag === 1) ? (errors = copyOfStruct.fields[0], react.createElement(Editor, {
            language: "fsharp",
            isReadOnly: true,
            value: errors,
        })) : (parsed = copyOfStruct.fields[0], react.createElement(Editor, {
            language: "fsharp",
            isReadOnly: true,
            value: parsed.String,
        })));
    }
    let astErrors;
    const o_1 = bind((parsed_1) => {
        let parsed_2;
        let pattern_matching_result;
        if (parsed_1.tag === 0) {
            if (parsed_2 = parsed_1.fields[0], !isEmpty(parsed_2.Errors)) {
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
                const parsed_3 = parsed_1.fields[0];
                const badgeColor = (e) => {
                    if (e.Severity === "warning") {
                        return "warning";
                    }
                    else {
                        return "danger";
                    }
                };
                const errors_1 = mapIndexed((i, e_1) => react.createElement("li", {
                    key: toText(printf("ast-error-%i"))(i),
                }, react.createElement("strong", {}, toText(printf("(%i,%i) (%i, %i)"))(e_1.Range.StartLine)(e_1.Range.StartCol)(e_1.Range.EndLine)(e_1.Range.EndCol)), badge([new BadgeProps(1, badgeColor(e_1))], [e_1.Severity]), badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(158, "ErrorNumber")))], [e_1.ErrorNumber]), badge([new BadgeProps(1, "light"), new BadgeProps(5, singleton(new HTMLAttr(158, "SubCategory")))], [e_1.SubCategory]), react.createElement("p", {}, e_1.Message)), parsed_3.Errors);
                return react.createElement("ul", {
                    id: "ast-errors",
                    className: "",
                }, errors_1);
            }
            case 1: {
                return void 0;
            }
        }
    }, model.Parsed);
    if (o_1 == null) {
        astErrors = null;
    }
    else {
        const o_2 = o_1;
        astErrors = o_2;
    }
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
        return results(model, dispatch);
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

