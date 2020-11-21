import { multiButton, MultiButtonSettings, toggleButton, input } from "../SettingControls.js";
import { isMatch } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/RegExp.js";
import { parse } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Int32.js";
import { FantomasMode, Model__get_SettingsChangedByTheUser, Msg } from "./Model.js";
import { optionValue, getOptionKey, sortByOption, FantomasOption } from "../shared/FantomasOnlineShared.js";
import * as react from "../../../web_modules/react.js";
import { toList } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Map.js";
import { empty, singleton, ofArray, ofSeq, map, sortBy } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { escapeDataString, equalsSafe, comparePrimitives } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { map as map_1, choose, zip } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { isNullOrWhiteSpace, join, printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { DOMAttr, HTMLAttr } from "../bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { ButtonProps, button } from "../bin/.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { loader } from "../Loader.js";
import Editor from "../../js/Editor.js";
import { SpinnerProps, spinner } from "../bin/.fable/Fable.Reactstrap.0.5.1/Spinner.fs.js";
import { versionBar } from "../VersionBar.js";

function mapToOption(dispatch, key, fantomasOption) {
    let editor;
    switch (fantomasOption.tag) {
        case 0: {
            editor = input(key, (nv) => {
                if (isMatch(nv, "\\d+")) {
                    let v_2;
                    v_2 = parse(nv, 511, false, 32);
                    dispatch(new Msg(5, [key, new FantomasOption(0, fantomasOption.fields[0], key, v_2)]));
                }
            }, key, "integer", fantomasOption.fields[2]);
            break;
        }
        case 2: {
            const o_2 = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg3) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "character_width")]));
            }, (_arg4) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "number_of_items")]));
            }, "CharacterWidth", "NumberOfItems", key, fantomasOption.fields[2] === "character_width");
            break;
        }
        case 3: {
            const o_3 = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg5) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "crlr")]));
            }, (_arg6) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "lf")]));
            }, "CRLF", "LF", key, fantomasOption.fields[2] === "crlf");
            break;
        }
        default: {
            const o = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg1) => {
                dispatch(new Msg(5, [key, new FantomasOption(1, o, key, true)]));
            }, (_arg2) => {
                dispatch(new Msg(5, [key, new FantomasOption(1, o, key, false)]));
            }, "true", "false", key, fantomasOption.fields[2]);
        }
    }
    return react.createElement("div", {
        key: key,
        className: "fantomas-setting",
    }, editor);
}

export function options(model, dispatch) {
    let optionList;
    const list = toList(model.UserOptions);
    optionList = sortBy((tuple) => tuple[0], list, {
        Compare: comparePrimitives,
    });
    let els;
    els = map((tupledArg) => mapToOption(dispatch, tupledArg[0], tupledArg[1]), optionList);
    return Array.from(els);
}

export function githubIssueUri(code, model) {
    const location = window.location;
    let config;
    let list_1;
    let list;
    list = toList(model.UserOptions);
    list_1 = map((tuple) => tuple[1], list);
    config = sortBy(sortByOption, list_1, {
        Compare: comparePrimitives,
    });
    let defaultValues;
    defaultValues = sortBy(sortByOption, model.DefaultOptions, {
        Compare: comparePrimitives,
    });
    let options_1;
    let changedOptions;
    let source_2;
    let source_1;
    const source = zip(config, defaultValues);
    source_1 = Array.from(source);
    source_2 = choose((tupledArg) => {
        const userV = tupledArg[0];
        if (!equalsSafe(userV, tupledArg[1])) {
            return userV;
        }
        else {
            return void 0;
        }
    }, source_1);
    changedOptions = ofSeq(source_2);
    if (changedOptions.tail == null) {
        options_1 = "Default Fantomas configuration";
    }
    else {
        let arg10_1;
        let strings;
        strings = map_1((opt) => {
            const arg10 = getOptionKey(opt);
            const arg20 = optionValue(opt);
            const clo1 = toText(printf("                %s = %s"));
            const clo2 = clo1(arg10);
            return clo2(arg20);
        }, changedOptions);
        arg10_1 = join("\n", strings);
        const clo1_1 = toText(printf("```fsharp\r\n    { config with\r\n%s }\r\n```"));
        options_1 = clo1_1(arg10_1);
    }
    const codeTemplate = (header, code_1) => {
        const clo1_2 = toText(printf("\r\n#### %s\r\n\r\n```fsharp\r\n%s\r\n```\r\n            "));
        const clo2_1 = clo1_2(header);
        return clo2_1(code_1);
    };
    let patternInput;
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 4: {
            patternInput = [codeTemplate("Code", code), codeTemplate("Error", matchValue.fields[0])];
            break;
        }
        case 3: {
            patternInput = [codeTemplate("Code", code), codeTemplate("Result", matchValue.fields[0])];
            break;
        }
        default: {
            patternInput = [codeTemplate("Code", code), ""];
        }
    }
    let body;
    let arg00;
    const arg10_3 = location.href;
    const clo1_3 = toText(printf("\r\n\u003c!--\r\n\r\n    Please only use this to create issues.\r\n    If you wish to suggest a feature,\r\n    please fill in the feature request template at https://github.com/fsprojects/fantomas/issues/new/choose\r\n\r\n--\u003e\r\nIssue created from [fantomas-online](%s)\r\n\r\n%s\r\n%s\r\n#### Problem description\r\n\r\nPlease describe here the Fantomas problem you encountered.\r\nCheck out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/master/CONTRIBUTING.md#bug-reports).\r\n\r\n#### Extra information\r\n\r\n- [ ] The formatted result breaks by code.\r\n- [ ] The formatted result gives compiler warnings.\r\n- [ ] I or my company would be willing to help fix this.\r\n\r\n#### Options\r\n\r\nFantomas %s\r\n\r\n%s\r\n        "));
    const clo2_2 = clo1_3(arg10_3);
    const clo3 = clo2_2(patternInput[0]);
    const clo4 = clo3(patternInput[1]);
    const clo5 = clo4(model.Version);
    arg00 = clo5(options_1);
    body = escapeDataString(arg00);
    let uri;
    const clo1_4 = toText(printf("https://github.com/fsprojects/fantomas/issues/new?title=%s\u0026labels=%s\u0026body=%s"));
    const clo2_3 = clo1_4("\u003cInsert meaningful title\u003e");
    const clo3_1 = clo2_3("bug");
    uri = clo3_1(body);
    return new HTMLAttr(94, uri);
}

function createGitHubIssue(code, model) {
    let pattern_matching_result;
    if (model.Mode.tag === 3) {
        if (!isNullOrWhiteSpace(code)) {
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
            return button([new ButtonProps(1, "danger"), new ButtonProps(2, true), new ButtonProps(9, ofArray([githubIssueUri(code, model), new HTMLAttr(64, "rounded-0")]))], ["Looks wrong? Create an issue!"]);
        }
        case 1: {
            return react.createElement("span", {
                className: "text-muted mr-2",
            }, "Looks wrong? Try using the preview version!");
        }
    }
}

export function view(model) {
    let props_1, props_5;
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 0: {
            return loader;
        }
        case 1: {
            return null;
        }
        case 3: {
            const children_1 = [(props_1 = {
                value: matchValue.fields[0],
                isReadOnly: true,
            }, react.createElement(Editor, props_1))];
            return react.createElement("div", {
                className: "tab-result",
            }, ...children_1);
        }
        case 4: {
            const children_4 = [(props_5 = {
                value: matchValue.fields[0],
                isReadOnly: true,
            }, react.createElement(Editor, props_5))];
            return react.createElement("div", {
                className: "tab-result",
            }, ...children_4);
        }
        default: {
            return loader;
        }
    }
}

function userChangedSettings(model) {
    let value;
    const list = Model__get_SettingsChangedByTheUser(model);
    value = (list.tail == null);
    return !value;
}

export function commands(code, model, dispatch) {
    let o, o_1, o_2, o_3;
    const formatButton = button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(3));
    })))], ["Format"]);
    let copySettingButton;
    if (userChangedSettings(model)) {
        const arg0 = button([new ButtonProps(1, "secondary"), new ButtonProps(9, ofArray([new HTMLAttr(64, "text-white"), new DOMAttr(40, (_arg2) => {
            dispatch(new Msg(8));
        })]))], ["Copy settings"]);
        copySettingButton = arg0;
    }
    else {
        copySettingButton = (void 0);
    }
    let children;
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 2: {
            children = ofArray([formatButton, (o = copySettingButton, (o == null) ? null : (o_1 = o, o_1))]);
            break;
        }
        case 1:
        case 3:
        case 4: {
            children = ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : (o_3 = o_2, o_3))]);
            break;
        }
        default: {
            children = empty();
        }
    }
    return react.createElement(react.Fragment, {}, ...children);
}

export function settings(model, dispatch) {
    let clo1;
    if (model.State.tag === 0) {
        return spinner([new SpinnerProps(2, "primary")], []);
    }
    else {
        let fantomasMode;
        let options_1;
        options_1 = map((tupledArg) => {
            const m = tupledArg[0];
            const IsActive = equalsSafe(model.Mode, m);
            return new MultiButtonSettings(tupledArg[1], (_arg1) => {
                dispatch(new Msg(6, m));
            }, IsActive);
        }, ofArray([[new FantomasMode(0), "2.x"], [new FantomasMode(1), "3.x"], [new FantomasMode(2), "4.x"], [new FantomasMode(3), "Preview"]]));
        fantomasMode = multiButton("Mode", options_1);
        const fileExtension = toggleButton((_arg2) => {
            dispatch(new Msg(7, true));
        }, (_arg3) => {
            dispatch(new Msg(7, false));
        }, "*.fsi", "*.fs", "File extension", model.IsFsi);
        const options_2 = options(model, dispatch);
        const children = [versionBar((clo1 = toText(printf("Version: %s")), clo1(model.Version))), fantomasMode, fileExtension, react.createElement("hr", {}), options_2];
        return react.createElement(react.Fragment, {}, ...children);
    }
}

