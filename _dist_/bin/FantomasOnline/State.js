import * as __SNOWPACK_ENV__ from '../../../_snowpack/env.js';

import { add, empty as empty_1, find, ofList } from "../.fable/fable-library.3.1.7/Map.js";
import { Model__get_SettingsChangedByTheUser, Model, EditorState, Msg, FantomasMode } from "./Model.js";
import { contains, tryFind, isEmpty, map, empty, singleton, ofArray } from "../.fable/fable-library.3.1.7/List.js";
import { postJson, getText } from "../Http.js";
import { isNullOrWhiteSpace, toConsole, join, trimStart, toFail, printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import { Types_RequestProperties, fetch$ } from "../.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { decodeOptionsFromUrl, decodeFormatResponse, decodeOptions } from "./Decoders.js";
import { encodeUrlModel, encodeRequest } from "./Encoders.js";
import { fromString } from "../.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { stringHash, uncurry } from "../.fable/fable-library.3.1.7/Util.js";
import { toString } from "../.fable/Thoth.Json.5.0.0/Encode.fs.js";
import { restoreModelFromUrl, updateUrlWithData } from "../UrlTools.js";
import { Cmd_ofSub, Cmd_none, Cmd_OfFunc_result, Cmd_batch, Cmd_OfPromise_either } from "../.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { getOptionKey, FantomasOption } from "../shared/FantomasOnlineShared.js";
import { showSuccess as showSuccess_1 } from "../../js/notifications.js";
import { map as map_1 } from "../.fable/fable-library.3.1.7/Seq.js";
import { isUpper } from "../.fable/fable-library.3.1.7/Char.js";

const backend = ofList(ofArray([[new FantomasMode(0), __SNOWPACK_ENV__.SNOWPACK_PUBLIC_FANTOMAS_V2], [new FantomasMode(1), __SNOWPACK_ENV__.SNOWPACK_PUBLIC_FANTOMAS_V3], [new FantomasMode(2), __SNOWPACK_ENV__.SNOWPACK_PUBLIC_FANTOMAS_V4], [new FantomasMode(3), __SNOWPACK_ENV__.SNOWPACK_PUBLIC_FANTOMAS_PREVIEW]]));

function getVersion(mode) {
    let arg10;
    return getText((arg10 = find(mode, backend), toText(printf("%s/%s"))(arg10)("api/version")));
}

function getOptions(mode) {
    let url;
    const arg10 = find(mode, backend);
    url = toText(printf("%s/%s"))(arg10)("api/options");
    let pr_1;
    const pr = fetch$(url, singleton(new Types_RequestProperties(0, "GET")));
    pr_1 = (pr.then(((res) => res.text())));
    return pr_1.then(((json) => {
        const matchValue = decodeOptions(json);
        if (matchValue.tag === 1) {
            const e = matchValue.fields[0];
            return toFail(printf("%A"))(e);
        }
        else {
            const v = matchValue.fields[0];
            return v;
        }
    }));
}

function getFormattedCode(code, model, dispatch) {
    let url;
    const arg10 = find(model.Mode, backend);
    url = toText(printf("%s/%s"))(arg10)("api/format");
    const json = encodeRequest(code, model);
    const pr = postJson(url, json);
    pr.then(((tupledArg) => {
        let matchValue, err, res;
        const status = tupledArg[0] | 0;
        const body = tupledArg[1];
        dispatch((status === 200) ? (matchValue = fromString(uncurry(2, decodeFormatResponse), body), (matchValue.tag === 1) ? (err = matchValue.fields[0], new Msg(2, err)) : (res = matchValue.fields[0], new Msg(4, res))) : ((status === 400) ? (new Msg(2, body)) : ((status === 413) ? (new Msg(2, "the input was too large to process")) : (new Msg(2, body)))));
    }));
}

function updateUrl(code, model, _arg1) {
    const json = toString(2, encodeUrlModel(code, model));
    updateUrlWithData(json);
}

export function getOptionsCmd(mode) {
    return Cmd_OfPromise_either((mode_1) => getOptions(mode_1), mode, (arg0) => (new Msg(1, arg0)), (exn) => (new Msg(2, exn.message)));
}

export function getVersionCmd(mode) {
    return Cmd_OfPromise_either((mode_1) => getVersion(mode_1), mode, (arg0) => (new Msg(0, arg0)), (exn) => (new Msg(2, exn.message)));
}

export function init(mode) {
    let cmd;
    const versionCmd = getVersionCmd(mode);
    const optionsCmd = getOptionsCmd(mode);
    cmd = Cmd_batch(ofArray([versionCmd, optionsCmd]));
    return [new Model(false, "???", empty(), empty_1(), mode, new EditorState(0)), cmd];
}

export function optionsListToMap(options) {
    return ofList(map((_arg1) => {
        switch (_arg1.tag) {
            case 0: {
                const k_1 = _arg1.fields[1];
                const fo_1 = _arg1;
                return [k_1, fo_1];
            }
            case 2: {
                const k_2 = _arg1.fields[1];
                const fo_2 = _arg1;
                return [k_2, fo_2];
            }
            case 3: {
                const k_3 = _arg1.fields[1];
                const fo_3 = _arg1;
                return [k_3, fo_3];
            }
            default: {
                const k = _arg1.fields[1];
                const fo = _arg1;
                return [k, fo];
            }
        }
    }, options));
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
    const isFsi = patternInput[1];
    let reconstructedOptions;
    if (isEmpty(userOptions)) {
        reconstructedOptions = optionsListToMap(defaultOptions);
    }
    else {
        const uo = userOptions;
        reconstructedOptions = optionsListToMap(map((defOpt) => {
            const key = getOptionKey(defOpt);
            const matchingUserOption = tryFind((uOpt) => (getOptionKey(uOpt) === key), uo);
            if (matchingUserOption == null) {
                return defOpt;
            }
            else {
                const muo = matchingUserOption;
                return updateOptionValue(defOpt, muo);
            }
        }, defaultOptions));
    }
    return [reconstructedOptions, isFsi];
}

const showSuccess = showSuccess_1;

const showError = showSuccess_1;

function copySettings(model, _arg1) {
    const supportedProperties = ofArray(["max_line_length", "indent_size", "end_of_line"]);
    const toEditorConfigName = (value) => {
        const name = trimStart(join("", map_1((c) => {
            if (isUpper(c)) {
                const arg10 = c.toLocaleLowerCase();
                return toText(printf("_%s"))(arg10);
            }
            else {
                return c;
            }
        }, value)), "_");
        if (contains(name, supportedProperties, {
            Equals: (x, y) => (x === y),
            GetHashCode: (x) => stringHash(x),
        })) {
            return name;
        }
        else {
            return toText(printf("fsharp_%s"))(name);
        }
    };
    let editorconfig;
    const arg10_6 = join("\n", map((_arg2) => {
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
                const v = _arg2.fields[2];
                const k = _arg2.fields[1];
                if (v) {
                    const arg10_2 = toEditorConfigName(k.split(""));
                    return toText(printf("%s=true"))(arg10_2);
                }
                else {
                    const arg10_3 = toEditorConfigName(k.split(""));
                    return toText(printf("%s=false"))(arg10_3);
                }
            }
            case 1: {
                const v_1 = _arg2.fields[2] | 0;
                const k_1 = _arg2.fields[1];
                const arg10_4 = toEditorConfigName(k_1.split(""));
                return toText(printf("%s=%i"))(arg10_4)(v_1);
            }
            case 2: {
                const arg10_5 = toEditorConfigName(k_2.split(""));
                return toText(printf("%s=%s"))(arg10_5)(v_2);
            }
        }
    }, Model__get_SettingsChangedByTheUser(model)));
    editorconfig = toText(printf("[*.fs]\n%s"))(arg10_6);
    let pr_1;
    const pr = navigator.clipboard.writeText(editorconfig);
    pr_1 = (pr.then(void 0, ((err) => {
        showError("Something went wrong while copying settings to the clipboard.");
        toConsole(printf("%A"))(err);
    })));
    pr_1.then((() => showSuccess("Copied fantomas-config settings to clipboard!")));
}

export function update(isActiveTab, code, msg, model) {
    switch (msg.tag) {
        case 1: {
            const options = msg.fields[0];
            const patternInput = isActiveTab ? restoreUserOptionsFromUrl(options) : [optionsListToMap(options), model.IsFsi];
            const userOptions = patternInput[0];
            const isFsi = patternInput[1];
            const cmd = ((!isNullOrWhiteSpace(code)) ? isActiveTab : false) ? Cmd_OfFunc_result(new Msg(3)) : Cmd_none();
            return [new Model(isFsi, model.Version, options, userOptions, model.Mode, new EditorState(1)), cmd];
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
            const error = msg.fields[0];
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, new EditorState(4, error)), Cmd_none()];
        }
        case 4: {
            const result = msg.fields[0];
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, new EditorState(3, result)), Cmd_none()];
        }
        case 5: {
            const value = msg.fields[0][1];
            const key = msg.fields[0][0];
            const userOptions_1 = add(key, value, model.UserOptions);
            return [new Model(model.IsFsi, model.Version, model.DefaultOptions, userOptions_1, model.Mode, model.State), Cmd_none()];
        }
        case 6: {
            return [model, Cmd_none()];
        }
        case 7: {
            const isFsi_1 = msg.fields[0];
            return [new Model(isFsi_1, model.Version, model.DefaultOptions, model.UserOptions, model.Mode, model.State), Cmd_none()];
        }
        case 8: {
            return [model, Cmd_ofSub((arg10$0040) => {
                copySettings(model, arg10$0040);
            })];
        }
        default: {
            const version = msg.fields[0];
            return [new Model(model.IsFsi, version, model.DefaultOptions, model.UserOptions, model.Mode, model.State), Cmd_none()];
        }
    }
}

