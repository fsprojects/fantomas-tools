import { isMatch } from "../.fable/fable-library.3.1.7/RegExp.js";
import { parse } from "../.fable/fable-library.3.1.7/Int32.js";
import { FantomasMode, Model__get_SettingsChangedByTheUser, Msg } from "./Model.js";
import { getOptionKey, optionValue, sortByOption, FantomasOption$reflection, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { MultiButtonSettings, multiButton, toggleButton, input } from "../SettingControls.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { singleton, mapIndexed, empty, ofArray, isEmpty, map, toArray, sortBy } from "../.fable/fable-library.3.1.7/List.js";
import { toList } from "../.fable/fable-library.3.1.7/Map.js";
import { escapeDataString, equals, comparePrimitives } from "../.fable/fable-library.3.1.7/Util.js";
import { toString, Record } from "../.fable/fable-library.3.1.7/Types.js";
import { record_type, bool_type, class_type, list_type, string_type } from "../.fable/fable-library.3.1.7/Reflection.js";
import { map as map_1, zip, toArray as toArray_1, choose, toList as toList_1 } from "../.fable/fable-library.3.1.7/Seq.js";
import { isNullOrWhiteSpace, printf, toText, join } from "../.fable/fable-library.3.1.7/String.js";
import { DOMAttr, HTMLAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { defaultArg } from "../.fable/fable-library.3.1.7/Option.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { BadgeProps, badge } from "../.fable/Fable.Reactstrap.0.5.1/Badge.fs.js";
import { loader } from "../Loader.js";
import Editor from "../../js/Editor.js";
import { SpinnerProps, spinner } from "../.fable/Fable.Reactstrap.0.5.1/Spinner.fs.js";
import { versionBar } from "../VersionBar.js";

function mapToOption(dispatch, key, fantomasOption) {
    let editor;
    switch (fantomasOption.tag) {
        case 0: {
            const v_1 = fantomasOption.fields[2] | 0;
            const o_1 = fantomasOption.fields[0] | 0;
            const onChange = (nv) => {
                if (isMatch(nv, "\\d+")) {
                    const v_2 = parse(nv, 511, false, 32) | 0;
                    dispatch(new Msg(5, [key, new FantomasOption(0, o_1, key, v_2)]));
                }
            };
            editor = input(key, onChange, key, "integer", v_1);
            break;
        }
        case 2: {
            const v_3 = fantomasOption.fields[2];
            const o_2 = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg3) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "character_width")]));
            }, (_arg4) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_2, key, "number_of_items")]));
            }, "CharacterWidth", "NumberOfItems", key, v_3 === "character_width");
            break;
        }
        case 3: {
            const v_4 = fantomasOption.fields[2];
            const o_3 = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg5) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "crlr")]));
            }, (_arg6) => {
                dispatch(new Msg(5, [key, new FantomasOption(2, o_3, key, "lf")]));
            }, "CRLF", "LF", key, v_4 === "crlf");
            break;
        }
        default: {
            const v = fantomasOption.fields[2];
            const o = fantomasOption.fields[0] | 0;
            editor = toggleButton((_arg1) => {
                dispatch(new Msg(5, [key, new FantomasOption(1, o, key, true)]));
            }, (_arg2) => {
                dispatch(new Msg(5, [key, new FantomasOption(1, o, key, false)]));
            }, "true", "false", key, v);
        }
    }
    return react.createElement("div", {
        key: key,
        className: "fantomas-setting",
    }, editor);
}

export function options(model, dispatch) {
    const optionList = sortBy((tuple) => tuple[0], toList(model.UserOptions), {
        Compare: (x, y) => comparePrimitives(x, y),
    });
    return toArray(map((tupledArg) => mapToOption(dispatch, tupledArg[0], tupledArg[1]), optionList));
}

export class GithubIssue extends Record {
    constructor(BeforeHeader, BeforeContent, AfterHeader, AfterContent, Description, Title, DefaultOptions, UserOptions, Version, IsFsi) {
        super();
        this.BeforeHeader = BeforeHeader;
        this.BeforeContent = BeforeContent;
        this.AfterHeader = AfterHeader;
        this.AfterContent = AfterContent;
        this.Description = Description;
        this.Title = Title;
        this.DefaultOptions = DefaultOptions;
        this.UserOptions = UserOptions;
        this.Version = Version;
        this.IsFsi = IsFsi;
    }
}

export function GithubIssue$reflection() {
    return record_type("FantomasTools.Client.FantomasOnline.View.GithubIssue", [], GithubIssue, () => [["BeforeHeader", string_type], ["BeforeContent", string_type], ["AfterHeader", string_type], ["AfterContent", string_type], ["Description", string_type], ["Title", string_type], ["DefaultOptions", list_type(FantomasOption$reflection())], ["UserOptions", class_type("Microsoft.FSharp.Collections.FSharpMap`2", [string_type, FantomasOption$reflection()])], ["Version", string_type], ["IsFsi", bool_type]]);
}

export function githubIssueUri(githubIssue) {
    let arg10_3;
    const location = window.location;
    const config = sortBy((_arg1) => sortByOption(_arg1), map((tuple) => tuple[1], toList(githubIssue.UserOptions)), {
        Compare: (x, y) => comparePrimitives(x, y),
    });
    const defaultValues = sortBy((_arg1_1) => sortByOption(_arg1_1), githubIssue.DefaultOptions, {
        Compare: (x_1, y_1) => comparePrimitives(x_1, y_1),
    });
    let options_1;
    const changedOptions = toList_1(choose((tupledArg) => {
        const userV = tupledArg[0];
        const defV = tupledArg[1];
        if (!equals(userV, defV)) {
            return userV;
        }
        else {
            return void 0;
        }
    }, toArray_1(zip(config, defaultValues))));
    if (isEmpty(changedOptions)) {
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
    const codeTemplate = (header, code) => toText(printf("\n#### %s\n\n```fsharp\n%s\n```\n            "))(header)(code);
    const patternInput = [codeTemplate(githubIssue.BeforeHeader, githubIssue.BeforeContent), codeTemplate(githubIssue.AfterHeader, githubIssue.AfterContent)];
    const right = patternInput[1];
    const left = patternInput[0];
    const fileType = githubIssue.IsFsi ? "\n*Signature file*" : "";
    const body = escapeDataString((arg10_3 = location.href, toText(printf("\n\u003c!--\n\n    Please only use this to create issues.\n    If you wish to suggest a feature,\n    please fill in the feature request template at https://github.com/fsprojects/fantomas/issues/new/choose\n\n--\u003e\nIssue created from [fantomas-online](%s)\n\n%s\n%s\n#### Problem description\n\n%s\n\n#### Extra information\n\n- [ ] The formatted result breaks by code.\n- [ ] The formatted result gives compiler warnings.\n- [ ] I or my company would be willing to help fix this.\n\n#### Options\n\nFantomas %s\n\n%s\n%s\n\n\u003csub\u003eDid you know that you can ignore files when formatting from fantomas-tool or the FAKE targets by using a [.fantomasignore file](https://github.com/fsprojects/fantomas/blob/master/docs/Documentation.md#ignore-files-fantomasignore)?\u003c/sub\u003e\n        "))(arg10_3)(left)(right)(githubIssue.Description)(githubIssue.Version)(options_1)(fileType)));
    const uri = toText(printf("https://github.com/fsprojects/fantomas/issues/new?title=%s\u0026body=%s"))(githubIssue.Title)(body);
    return new HTMLAttr(94, uri);
}

function createGitHubIssue(code, model) {
    const description = "Please describe here the Fantomas problem you encountered.\n                    Check out our [Contribution Guidelines](https://github.com/fsprojects/fantomas/blob/master/CONTRIBUTING.md#bug-reports).";
    let patternInput;
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 4: {
            const e = matchValue.fields[0];
            patternInput = ["Code", code, "Error", e];
            break;
        }
        case 3: {
            const result = matchValue.fields[0];
            patternInput = ["Code", code, "Result", defaultArg(result.SecondFormat, result.FirstFormat)];
            break;
        }
        default: {
            patternInput = ["Code", code, "", ""];
        }
    }
    const bh = patternInput[0];
    const bc = patternInput[1];
    const ah = patternInput[2];
    const ac = patternInput[3];
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
            const githubIssue = new GithubIssue(bh, bc, ah, ac, description, "\u003cInsert meaningful title\u003e", model.DefaultOptions, model.UserOptions, model.Version, model.IsFsi);
            return button([new ButtonProps(1, "danger"), new ButtonProps(2, true), new ButtonProps(9, ofArray([githubIssueUri(githubIssue), new HTMLAttr(157, "_blank"), new HTMLAttr(64, "rounded-0")]))], ["Looks wrong? Create an issue!"]);
        }
        case 1: {
            return react.createElement("span", {
                className: "text-muted mr-2",
            }, "Looks wrong? Try using the preview version!");
        }
    }
}

function viewErrors(model, result, isIdempotent, errors) {
    let o, o_1;
    let errors_2;
    if (isEmpty(errors)) {
        errors_2 = empty();
    }
    else {
        const errors_1 = errors;
        const badgeColor = (e) => {
            const matchValue = e.Severity;
            switch (matchValue.tag) {
                case 0: {
                    return "danger";
                }
                case 1: {
                    return "warning";
                }
                default: {
                    return "info";
                }
            }
        };
        errors_2 = mapIndexed((i, e_1) => react.createElement("li", {
            key: toText(printf("ast-error-%i"))(i),
        }, react.createElement("strong", {}, toText(printf("(%i,%i) (%i, %i)"))(e_1.Range.StartLine)(e_1.Range.StartCol)(e_1.Range.EndLine)(e_1.Range.EndCol)), badge([new BadgeProps(1, badgeColor(e_1))], [toString(e_1.Severity)]), badge([new BadgeProps(1, "dark"), new BadgeProps(5, singleton(new HTMLAttr(158, "ErrorNumber")))], [e_1.ErrorNumber]), badge([new BadgeProps(1, "light"), new BadgeProps(5, singleton(new HTMLAttr(158, "SubCategory")))], [e_1.SubCategory]), react.createElement("p", {}, e_1.Message)), errors_1);
    }
    let idempotency;
    if (isIdempotent) {
        idempotency = (void 0);
    }
    else {
        const githubIssue = new GithubIssue("Formatted code", result.FirstFormat, "Reformatted code", defaultArg(result.SecondFormat, result.FirstFormat), "Fantomas was not able to produce the same code after reformatting the result.", "Idempotency problem when \u003cadd use-case\u003e", model.DefaultOptions, model.UserOptions, model.Version, model.IsFsi);
        idempotency = react.createElement("div", {
            className: "idempotent-error",
        }, react.createElement("h6", {}, "The result was not idempotent"), "Fantomas was able to format the code, but when formatting the result again, the code changed.", react.createElement("br", {}), "The result after the first format is being displayed.", react.createElement("br", {}), button([new ButtonProps(1, "danger"), new ButtonProps(9, ofArray([githubIssueUri(githubIssue), new HTMLAttr(157, "_blank"), new HTMLAttr(64, "rounded-0")]))], ["Report idempotancy issue"]));
    }
    if ((!isIdempotent) ? true : (!isEmpty(errors_2))) {
        return react.createElement("ul", {
            id: "ast-errors",
            className: "",
        }, (o = idempotency, (o == null) ? null : (o_1 = o, o_1)), toArray(errors_2));
    }
    else {
        return void 0;
    }
}

export function view(model) {
    let sf, o, o_1;
    const matchValue = model.State;
    switch (matchValue.tag) {
        case 0: {
            return loader;
        }
        case 1: {
            return null;
        }
        case 3: {
            const result = matchValue.fields[0];
            let patternInput;
            const matchValue_1 = result.SecondFormat;
            let pattern_matching_result, sf_1;
            if (matchValue_1 != null) {
                if (sf = matchValue_1, sf === result.FirstFormat) {
                    pattern_matching_result = 0;
                    sf_1 = matchValue_1;
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
                    patternInput = [sf_1, true, result.SecondValidation];
                    break;
                }
                case 1: {
                    patternInput = ((matchValue_1 == null) ? [result.FirstFormat, true, result.FirstValidation] : [result.FirstFormat, false, result.FirstValidation]);
                    break;
                }
            }
            const isIdempotent = patternInput[1];
            const formattedCode = patternInput[0];
            const astErrors = patternInput[2];
            return react.createElement("div", {
                className: "tab-result fantomas-result",
            }, react.createElement("div", {
                className: "fantomas-editor-container",
            }, react.createElement(Editor, {
                value: formattedCode,
                isReadOnly: true,
            })), (o = viewErrors(model, result, isIdempotent, astErrors), (o == null) ? null : (o_1 = o, o_1)));
        }
        case 4: {
            const error = matchValue.fields[0];
            return react.createElement("div", {
                className: "tab-result",
            }, react.createElement(Editor, {
                value: error,
                isReadOnly: true,
            }));
        }
        default: {
            return loader;
        }
    }
}

function userChangedSettings(model) {
    return !isEmpty(Model__get_SettingsChangedByTheUser(model));
}

export function commands(code, model, dispatch) {
    let matchValue, o, o_1, o_2, o_3;
    const formatButton = button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(3));
    })))], ["Format"]);
    const copySettingButton = userChangedSettings(model) ? button([new ButtonProps(1, "secondary"), new ButtonProps(9, ofArray([new HTMLAttr(64, "text-white"), new DOMAttr(40, (_arg2) => {
        dispatch(new Msg(8));
    })]))], ["Copy settings"]) : (void 0);
    return react.createElement(react.Fragment, {}, ...(matchValue = model.State, (matchValue.tag === 2) ? ofArray([formatButton, (o = copySettingButton, (o == null) ? null : (o_1 = o, o_1))]) : ((matchValue.tag === 1) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : (o_3 = o_2, o_3))]) : ((matchValue.tag === 3) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : (o_3 = o_2, o_3))]) : ((matchValue.tag === 4) ? ofArray([createGitHubIssue(code, model), formatButton, (o_2 = copySettingButton, (o_2 == null) ? null : (o_3 = o_2, o_3))]) : empty())))));
}

export function settings(model, dispatch) {
    if (model.State.tag === 0) {
        return spinner([new SpinnerProps(2, "primary")], []);
    }
    else {
        const fantomasMode = multiButton("Mode", map((tupledArg) => {
            const m = tupledArg[0];
            const l = tupledArg[1];
            return new MultiButtonSettings(l, (_arg1) => {
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

