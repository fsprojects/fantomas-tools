import * as react from "../../../_snowpack/pkg/react.js";
import { MultiButtonSettings, multiButton, toggleButton, input } from "../SettingControls.js";
import { isMatch } from "../.fable/fable-library.3.0.1/RegExp.js";
import { FantomasMode, Model__get_SettingsChangedByTheUser, Msg } from "./Model.js";
import { getOptionKey, optionValue, sortByOption, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { parse } from "../.fable/fable-library.3.0.1/Int32.js";
import { empty, singleton, ofArray, ofSeq, sortBy, map } from "../.fable/fable-library.3.0.1/List.js";
import { toList } from "../.fable/fable-library.3.0.1/Map.js";
import { escapeDataString, equals, comparePrimitives } from "../.fable/fable-library.3.0.1/Util.js";
import { map as map_1, zip, choose } from "../.fable/fable-library.3.0.1/Seq.js";
import { isNullOrWhiteSpace, printf, toText, join } from "../.fable/fable-library.3.0.1/String.js";
import { DOMAttr, HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { loader } from "../Loader.js";
import Editor from "../../js/Editor.js";
import { SpinnerProps, spinner } from "../.fable/Fable.Reactstrap.0.5.1/Spinner.fs.js";
import { versionBar } from "../VersionBar.js";

function mapToOption(dispatch, key, fantomasOption) {
    let o_2, o_3, o;
    return react.createElement("div", {
        key: key,
        className: "fantomas-setting",
    }, (fantomasOption.tag === 0) ? input(key, (nv) => {
        if (isMatch(nv, "\\d+")) {
            dispatch(new Msg(5, [key, new FantomasOption(0, fantomasOption.fields[0], key, parse(nv, 511, false, 32))]));
        }
    }, key, "integer", fantomasOption.fields[2]) : ((fantomasOption.tag === 2) ? (o_2 = (fantomasOption.fields[0] | 0), toggleButton((_arg3) => {
        dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "character_width")]));
    }, (_arg4) => {
        dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "number_of_items")]));
    }, "CharacterWidth", "NumberOfItems", key, fantomasOption.fields[2] === "character_width")) : ((fantomasOption.tag === 3) ? (o_3 = (fantomasOption.fields[0] | 0), toggleButton((_arg5) => {
        dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "crlr")]));
    }, (_arg6) => {
        dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "lf")]));
    }, "CRLF", "LF", key, fantomasOption.fields[2] === "crlf")) : (o = (fantomasOption.fields[0] | 0), toggleButton((_arg1) => {
        dispatch(new Msg(5, [key, new FantomasOption(1, o, key, true)]));
    }, (_arg2) => {
        dispatch(new Msg(5, [key, new FantomasOption(1, o, key, false)]));
    }, "true", "false", key, fantomasOption.fields[2])))));
}

export function options(model, dispatch) {
    return Array.from(map((tupledArg) => mapToOption(dispatch, tupledArg[0], tupledArg[1]), sortBy((tuple) => tuple[0], toList(model.UserOptions), {
        Compare: comparePrimitives,
    })));
}

export function githubIssueUri(code, model) {
    let arg10_3;
    const location = window.location;
    let options_1;
    const changedOptions = ofSeq(choose((tupledArg) => {
        const userV = tupledArg[0];
        if (!equals(userV, tupledArg[1])) {
            return userV;
        }
        else {
            return void 0;
        }
    }, Array.from(zip(sortBy(sortByOption, map((tuple) => tuple[1], toList(model.UserOptions)), {
        Compare: comparePrimitives,
    }), sortBy(sortByOption, model.DefaultOptions, {
        Compare: comparePrimitives,
    })))));
    if (changedOptions.tail == null) {
        options_1 = "Default Fantomas configuration";
    }
    else {
        const arg10_1 = join("\n", map_1((opt) => {
            const arg20 = optionValue(opt);
            const arg10 = getOptionKey(opt);
            return toText(printf("                %s = %s"))(arg10)(arg20);
        }, changedOptions));
        options_1 = toText(printf("```fsharp\n    { config with\n%s }\n```"))(arg10_1);
    }
    const codeTemplate = (header, code_1) => toText(printf("\n#### %s\n\n```fsharp\n%s\n```\n            "))(header)(code_1);
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
    const body = escapeDataString((arg10_3 = location.href, toText(printf("\n\u003c!--\n\n    Please only use this to create issues.\n    If you wish to suggest a feature,\n    please fill in the feature request template at https://github.com/fsprojects/fantomas/issues/new/choose\n\n--\u003e\nIssue created from [fantomas-online](%s)\n\n%s\n%s\n#### Problem description\n\nPlease describe here the Fantomas problem you encountered.\nCheck out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/master/CONTRIBUTING.md#bug-reports).\n\n#### Extra information\n\n- [ ] The formatted result breaks by code.\n- [ ] The formatted result gives compiler warnings.\n- [ ] I or my company would be willing to help fix this.\n\n#### Options\n\nFantomas %s\n\n%s\n\n\u003csub\u003eDid you know that you can ignore files when formatting from fantomas-tool or the FAKE targets by using a [.fantomasignore file](https://github.com/fsprojects/fantomas/blob/master/docs/Documentation.md#ignore-files-fantomasignore)?\u003c/sub\u003e\n        "))(arg10_3)(patternInput[0])(patternInput[1])(model.Version)(options_1)));
    return new HTMLAttr(94, toText(printf("https://github.com/fsprojects/fantomas/issues/new?title=%s\u0026labels=%s\u0026body=%s"))("\u003cInsert meaningful title\u003e")("bug")(body));
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
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 0: {
            return loader;
        }
        case 1: {
            return null;
        }
        case 3: {
            return react.createElement("div", {
                className: "tab-result fantomas-result",
            }, react.createElement(Editor, {
                value: matchValue.fields[0],
                isReadOnly: true,
            }));
        }
        case 4: {
            return react.createElement("div", {
                className: "tab-result",
            }, react.createElement(Editor, {
                value: matchValue.fields[0],
                isReadOnly: true,
            }));
        }
        default: {
            return loader;
        }
    }
}

function userChangedSettings(model) {
    return !(Model__get_SettingsChangedByTheUser(model).tail == null);
}

export function commands(code, model, dispatch) {
    let matchValue, o, o_2;
    const formatButton = button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(3));
    })))], ["Format"]);
    const copySettingButton = userChangedSettings(model) ? button([new ButtonProps(1, "secondary"), new ButtonProps(9, ofArray([new HTMLAttr(64, "text-white"), new DOMAttr(40, (_arg2) => {
        dispatch(new Msg(8));
    })]))], ["Copy settings"]) : (void 0);
    return react.createElement(react.Fragment, {}, ...(matchValue = model.State, (matchValue.tag === 2) ? ofArray([formatButton, (o = copySettingButton, (o == null) ? null : o)]) : ((matchValue.tag === 1) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : o_2)]) : ((matchValue.tag === 3) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : o_2)]) : ((matchValue.tag === 4) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : o_2)]) : empty())))));
}

export function settings(model, dispatch) {
    if (model.State.tag === 0) {
        return spinner([new SpinnerProps(2, "primary")], []);
    }
    else {
        const fantomasMode = multiButton("Mode", map((tupledArg) => {
            const m = tupledArg[0];
            return new MultiButtonSettings(tupledArg[1], (_arg1) => {
                dispatch(new Msg(6, m));
            }, equals(model.Mode, m));
        }, ofArray([[new FantomasMode(0), "2.x"], [new FantomasMode(1), "3.x"], [new FantomasMode(2), "4.x"], [new FantomasMode(3), "Preview"]])));
        const fileExtension = toggleButton((_arg2) => {
            dispatch(new Msg(7, true));
        }, (_arg3) => {
            dispatch(new Msg(7, false));
        }, "*.fsi", "*.fs", "File extension", model.IsFsi);
        const options_2 = options(model, dispatch);
        return react.createElement(react.Fragment, {}, versionBar(toText(printf("Version: %s"))(model.Version)), fantomasMode, fileExtension, react.createElement("hr", {}), options_2);
    }
}

