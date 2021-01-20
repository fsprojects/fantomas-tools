import * as react from "../../../_snowpack/pkg/react.js";
import Editor from "../../js/Editor.js";
import { EditorProp } from "../Editor.js";
import { uncurry } from "../.fable/fable-library.3.0.1/Util.js";
import { some } from "../.fable/fable-library.3.0.1/Option.js";
import { singleton, ofArray } from "../.fable/fable-library.3.0.1/List.js";
import { keyValueList } from "../.fable/fable-library.3.0.1/MapUtil.js";
import { loader } from "../Loader.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { DOMAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { Msg } from "./Model.js";
import { versionBar } from "../VersionBar.js";
import { printf, toText } from "../.fable/fable-library.3.0.1/String.js";
import { MultiButtonSettings, multiButton, toggleButton, input } from "../SettingControls.js";

function isEditorView(_arg1) {
    if (_arg1.tag === 0) {
        return true;
    }
    else {
        return false;
    }
}

function isRawView(_arg1) {
    if (_arg1.tag === 1) {
        return true;
    }
    else {
        return false;
    }
}

function results(model, dispatch) {
    const matchValue = model.Parsed;
    if (matchValue.tag === 1) {
        return react.createElement(Editor, {
            language: "fsharp",
            isReadOnly: true,
            value: matchValue.fields[0],
        });
    }
    else if (matchValue.fields[0] == null) {
        return "";
    }
    else {
        const parsed = matchValue.fields[0];
        if (model.View.tag === 0) {
            const props_2 = ofArray([new EditorProp(2, "fsharp"), new EditorProp(3, true), new EditorProp(1, JSON.stringify(parsed.Node, uncurry(2, void 0), some(4)))]);
            return react.createElement(Editor, keyValueList(props_2, 1));
        }
        else {
            return react.createElement(Editor, {
                language: "fsharp",
                isReadOnly: true,
                value: parsed.String,
            });
        }
    }
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
        dispatch(new Msg(8, arg));
    }, "Defines", "Enter your defines separated with a space", model.Defines), toggleButton((_arg1) => {
        dispatch(new Msg(9, true));
    }, (_arg2) => {
        dispatch(new Msg(9, false));
    }, "*.fsi", "*.fs", "File extension", model.IsFsi), multiButton("Mode", ofArray([new MultiButtonSettings("Editor", (_arg3) => {
        dispatch(new Msg(6));
    }, isEditorView(model.View)), new MultiButtonSettings("Raw", (_arg4) => {
        dispatch(new Msg(7));
    }, isRawView(model.View))])));
}

