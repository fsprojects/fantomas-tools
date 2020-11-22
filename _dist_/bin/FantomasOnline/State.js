import __SNOWPACK_ENV__ from '../../../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import { add, empty as empty_1, find, ofList } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Map.js";
import { Model__get_SettingsChangedByTheUser, Model, EditorState, Msg, FantomasMode } from "./Model.js";
import { contains, tryFind, map, empty, singleton, ofArray } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { isNullOrWhiteSpace, toConsole, trimStart, join, toFail, printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { postJson, getText } from "../Http.js";
import { Types_RequestProperties, fetch$ } from "../bin/.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { decodeOptionsFromUrl, decodeOptions } from "./Decoders.js";
import { encodeUrlModel, encodeRequest } from "./Encoders.js";
import { toString } from "../bin/.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { restoreModelFromUrl, updateUrlWithData } from "../UrlTools.js";
import { Cmd_ofSub, Cmd_none, Cmd_OfFunc_result, Cmd_batch, Cmd_OfPromise_either } from "../bin/.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { getOptionKey, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { structuralHash, uncurry } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { showSuccess as showSuccess_1 } from "../../js/notifications.js";
import { map as map_1 } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { isUpper } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Char.js";

const backend = ofList(ofArray([[new FantomasMode(0), import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V2], [new FantomasMode(1), import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V3], [new FantomasMode(2), import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_V4], [new FantomasMode(3), import.meta.env.SNOWPACK_PUBLIC_FANTOMAS_PREVIEW]]));

function getVersion(mode) {
    let _url;
    const arg10 = find(mode, backend);
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    _url = clo2("api/version");
    return getText(_url);
}

function getOptions(mode) {
    let url;
    const arg10 = find(mode, backend);
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    url = clo2("api/options");
    let pr_1;
    const pr = fetch$(url, singleton(new Types_RequestProperties(0, "GET")));
    pr_1 = (pr.then(((res) => res.text())));
    return pr_1.then(((json) => {
        const matchValue = decodeOptions(json);
        if (matchValue.tag === 1) {
            const clo1_1 = toFail(printf("%A"));
            return clo1_1(matchValue.fields[0]);
        }
        else {
            return matchValue.fields[0];
        }
    }));
}

function getFormattedCode(code, model, dispatch) {
    let url;
    const arg10 = find(model.Mode, backend);
    const clo1 = toText(printf("%s/%s"));
    const clo2 = clo1(arg10);
    url = clo2("api/format");
    const json = encodeRequest(code, model);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (new Msg(4, body)) : ((status === 400) ? (new Msg(2, body)) : ((status === 413) ? (new Msg(2, "the input was too large to process")) : (new Msg(2, body)))));
    }));
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function getOptionsCmd(mode) {
    return Cmd_OfPromise_either(getOptions, mode, (arg0) => (new Msg(1, arg0)), (exn) => (new Msg(2, exn.message)));
}

export function init(mode) {
    let cmd;
    const versionCmd = Cmd_OfPromise_either(getVersion, mode, (arg0) => (new Msg(0, arg0)), (exn) => (new Msg(2, exn.message)));
    const optionsCmd = getOptionsCmd(mode);
    cmd = Cmd_batch(ofArray([versionCmd, optionsCmd]));
    return [new Model(false, "???", empty(), empty_1(), mode, new EditorState(0)), cmd];
}

export function optionsListToMap(options) {
    let elements;
    elements = map((_arg1) => {
        switch (_arg1.tag) {
            case 0: {
                return [_arg1.fields[1], _arg1];
            }
            case 2: {
                return [_arg1.fields[1], _arg1];
            }
            case 3: {
                return [_arg1.fields[1], _arg1];
            }
            default: {
                return [_arg1.fields[1], _arg1];
            }
        }
    }, options);
    return ofList(elements);
}

function updateOptionValue(defaultOption, userOption) {
    const matchValue = [defaultOption, userOption];
    let pattern_matching_result, k, o, v, k_1, o_1, v_1;
    if (matchValue[0].tag === 0) {
        if (matchValue[1].tag === 0) {
            pattern_matching_result = 0;
            k = matchValue[0].fields[1];
            o = matchValue[0].fields[0];
            v = matchValue[1].fields[2];
        }
        else {
            pattern_matching_result = 2;
        }
    }
    else if (matchValue[0].tag === 1) {
        if (matchValue[1].tag === 1) {
            pattern_matching_result = 1;
            k_1 = matchValue[0].fields[1];
            o_1 = matchValue[0].fields[0];
            v_1 = matchValue[1].fields[2];
        }
        else {
            pattern_matching_result = 2;
        }
    }
    else {
        pattern_matching_result = 2;
    }
    switch (pattern_matching_result) {
        case 0: {
            return new FantomasOption(0, o, k, v);
        }
        case 1: {
            return new FantomasOption(1, o_1, k_1, v_1);
        }
        case 2: {
            return defaultOption;
        }
    }
}

function restoreUserOptionsFromUrl(defaultOptions) {
    const patternInput = restoreModelFromUrl(uncurry(2, decodeOptionsFromUrl), [empty(), false]);
    const userOptions = patternInput[0];
    let reconstructedOptions;
    if (userOptions.tail == null) {
        reconstructedOptions = optionsListToMap(defaultOptions);
    }
    else {
        let options;
        options = map((defOpt) => {
            const key = getOptionKey(defOpt);
            const matchingUserOption = tryFind((uOpt) => (getOptionKey(uOpt) === key), userOptions);
            if (matchingUserOption == null) {
                return defOpt;
            }
            else {
                const muo = matchingUserOption;
                return updateOptionValue(defOpt, muo);
            }
        }, defaultOptions);
        reconstructedOptions = optionsListToMap(options);
    }
    return [reconstructedOptions, patternInput[1]];
}

const showSuccess = showSuccess_1;

const showError = showSuccess_1;

function copySettings(model, _arg1) {
    const toEditorConfigName = (value) => {
        let name;
        let s;
        let strings;
        strings = map_1((c) => {
            if (isUpper(c)) {
                const arg10 = c.toLocaleLowerCase();
                const clo1 = toText(printf("_%s"));
                return clo1(arg10);
            }
            else {
                return c;
            }
        }, value);
        s = join("", strings);
        name = trimStart(s, "_");
        if (contains(name, ofArray(["max_line_length", "indent_size", "end_of_line"]), {
            Equals: (x, y) => (x === y),
            GetHashCode: structuralHash,
        })) {
            return name;
        }
        else {
            const clo1_1 = toText(printf("fsharp_%s"));
            return clo1_1(name);
        }
    };
    let editorconfig;
    let arg10_6;
    let strings_1;
    const list = Model__get_SettingsChangedByTheUser(model);
    strings_1 = map((_arg2) => {
        let pattern_matching_result, k_2, v_2;
        switch (_arg2.tag) {
            case 0: {
                pattern_matching_result = 1;
                break;
            }
            case 2: {
                pattern_matching_result = 2;
                k_2 = _arg2.fields[1];
                v_2 = _arg2.fields[2];
                break;
            }
            case 3: {
                pattern_matching_result = 2;
                k_2 = _arg2.fields[1];
                v_2 = _arg2.fields[2];
                break;
            }
            default: pattern_matching_result = 0}
        switch (pattern_matching_result) {
            case 0: {
                const k = _arg2.fields[1];
                if (_arg2.fields[2]) {
                    let arg10_2;
                    arg10_2 = toEditorConfigName(k.split(""));
                    const clo1_2 = toText(printf("%s=true"));
                    return clo1_2(arg10_2);
                }
                else {
                    let arg10_3;
                    arg10_3 = toEditorConfigName(k.split(""));
                    const clo1_3 = toText(printf("%s=false"));
                    return clo1_3(arg10_3);
                }
            }
            case 1: {
                let arg10_4;
                arg10_4 = toEditorConfigName(_arg2.fields[1].split(""));
                const clo1_4 = toText(printf("%s=%i"));
                const clo2 = clo1_4(arg10_4);
                return clo2(_arg2.fields[2]);
            }
            case 2: {
                let arg10_5;
                arg10_5 = toEditorConfigName(k_2.split(""));
                const clo1_5 = toText(printf("%s=%s"));
                const clo2_1 = clo1_5(arg10_5);
                return clo2_1(v_2);
            }
        }
    }, list);
    arg10_6 = join("\n", strings_1);
    const clo1_6 = toText(printf("[*.fs]\n%s"));
    editorconfig = clo1_6(arg10_6);
    let pr_1;
    const pr = navigator.clipboard.writeText(editorconfig);
    pr_1 = (pr.then(void 0, ((err) => {
        showError("Something went wrong while copying settings to the clipboard.");
        const clo1_7 = toConsole(printf("%A"));
        clo1_7(err);
    })));
    pr_1.then((() => {
        showSuccess("Copied fantomas-config settings to clipboard!");
    }));
}

export function update(isActiveTab, code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const options = msg.fields[0];
            const patternInput = isActiveTab ? restoreUserOptionsFromUrl(options) : [optionsListToMap(options), model.IsFsi];
            const cmd = ((!isNullOrWhiteSpace(code)) ? isActiveTab : false) ? Cmd_OfFunc_result(new Msg(3)) : Cmd_none();
            return [new Model(patternInput[1], model.Version, options, patternInput[0], model.Mode, new EditorState(1)), cmd];
        }
        case 3: {
            const cmd_1 = Cmd_batch(ofArray([Cmd_ofSub((dispatch) => {
                getFormattedCode(code, model, dispatch);
            }), Cmd_ofSub((arg20$0040) => {
                updateUrl(code, model, arg20$0040);
            })]));
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, new EditorState(2)), cmd_1];
        }
        case 2: {
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, new EditorState(4, msg.fields[0])), Cmd_none()];
        }
        case 4: {
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, new EditorState(3, msg.fields[0])), Cmd_none()];
        }
        case 5: {
            const userOptions_1 = add(msg.fields[0][0], msg.fields[0][1], model.UserOptions);
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, userOptions_1, model.Mode, model.State), Cmd_none()];
        }
        case 6: {
            return [model, Cmd_none()];
        }
        case 7: {
            return [new Model(msg.fields[0], model.Version, model.DefaultOptions, model.UserOptions, model.Mode, model.State), Cmd_none()];
        }
        case 8: {
            return [model, Cmd_ofSub((arg10$0040) => {
                copySettings(model, arg10$0040);
            })];
        }
        default: {
            return [new Model(model.IsFsi, msg.fields[0], model.DefaultOptions, model.UserOptions, model.Mode, model.State), Cmd_none()];
        }
    }
}

