import { trimEnd, isNullOrWhiteSpace, substring, join, endsWith } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { ofSeq, singleton, empty, collect, ofArray, map } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { equalsWith } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { comparePrimitives } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { useReact_useEffect_Z5ECA432F, React_createDisposable_3A5B6456, useReact_useMemo_CF4EA67, useReact_useCallbackRef_7C4B0DD6, React_memo_62A0F746 } from "../Feliz.1.17.0/React.fs.js";
import { defaultArg } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { Impl_createRemoveOptions, Impl_adjustPassive } from "../Feliz.UseListener.0.6.3/Listener.fs.js";
import { tryParse } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Int32.js";
import { FSharpRef } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { tryParse as tryParse_1, fromInt } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Long.js";
import { tryParse as tryParse_2 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Guid.js";
import { tryParse as tryParse_3 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Double.js";
import { tryParse as tryParse_4 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Decimal.js";
import Decimal from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Decimal.js";
import { map as map_1, delay } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";

export function RouterModule_String_$007CPrefix$007C(prefix, str) {
    if (str.indexOf(prefix) === 0) {
        return str;
    }
    else {
        return void 0;
    }
}

export function RouterModule_String_$007CSuffix$007C(suffix, str) {
    if (endsWith(str, suffix)) {
        return str;
    }
    else {
        return void 0;
    }
}

export function RouterModule_encodeQueryString(queryStringPairs) {
    let _arg1;
    let strings;
    strings = map((tupledArg) => join("=", [encodeURIComponent(tupledArg[0]), encodeURIComponent(tupledArg[1])]), queryStringPairs);
    _arg1 = join("\u0026", strings);
    if (_arg1 === "") {
        return "";
    }
    else {
        return "?" + _arg1;
    }
}

export function RouterModule_encodeQueryStringInts(queryStringIntPairs) {
    let _arg1;
    let strings;
    strings = map((tupledArg) => join("=", [encodeURIComponent(tupledArg[0]), tupledArg[1]]), queryStringIntPairs);
    _arg1 = join("\u0026", strings);
    if (_arg1 === "") {
        return "";
    }
    else {
        return "?" + _arg1;
    }
}

function RouterModule_normalizeRoute(routeMode) {
    if (routeMode === 1) {
        return (_arg1) => {
            let activePatternResult8624, path, activePatternResult8625, path_1, activePatternResult8626, path_2;
            return activePatternResult8624 = RouterModule_String_$007CPrefix$007C("/", _arg1), (activePatternResult8624 != null) ? (path = activePatternResult8624, "#" + path) : (activePatternResult8625 = RouterModule_String_$007CPrefix$007C("#/", _arg1), (activePatternResult8625 != null) ? (path_1 = activePatternResult8625, path_1) : (activePatternResult8626 = RouterModule_String_$007CPrefix$007C("#", _arg1), (activePatternResult8626 != null) ? (path_2 = activePatternResult8626, "#/" + substring(path_2, 1, path_2.length - 1)) : ("#/" + _arg1)));
        };
    }
    else {
        return (_arg2) => {
            let activePatternResult8629, path_4;
            return activePatternResult8629 = RouterModule_String_$007CPrefix$007C("/", _arg2), (activePatternResult8629 != null) ? (path_4 = activePatternResult8629, path_4) : ("/" + _arg2);
        };
    }
}

export function RouterModule_encodeParts(xs, routeMode) {
    let xs_1;
    return RouterModule_normalizeRoute(routeMode)((xs_1 = (map((part) => {
        if (((part.indexOf("?") >= 0) ? true : (part.indexOf("#") === 0)) ? true : (part.indexOf("/") === 0)) {
            return part;
        }
        else {
            return encodeURIComponent(part);
        }
    }, xs)), (join("/", xs_1))));
}

export function RouterModule_nav(xs, mode, routeMode) {
    if (mode === 1) {
        history.pushState(void 0, "", RouterModule_encodeParts(xs, routeMode));
    }
    else {
        history.replaceState(void 0, "", RouterModule_encodeParts(xs, routeMode));
    }
    const ev = document.createEvent("CustomEvent");
    ev.initEvent("CUSTOM_NAVIGATION_EVENT", true, true);
    const value = window.dispatchEvent(ev);
    void value;
}

export function RouterModule_urlSegments(path, mode) {
    let activePatternResult8645, activePatternResult8640, activePatternResult8641;
    let list;
    let array;
    const str = (activePatternResult8645 = RouterModule_String_$007CPrefix$007C("#", path), (activePatternResult8645 != null) ? substring(path, 1, path.length - 1) : ((mode === 1) ? (activePatternResult8640 = RouterModule_String_$007CSuffix$007C("#", path), (activePatternResult8640 != null) ? "" : (activePatternResult8641 = RouterModule_String_$007CSuffix$007C("#/", path), (activePatternResult8641 != null) ? "" : path)) : path));
    array = str.split("/");
    list = ofArray(array);
    return collect((segment) => {
        if (isNullOrWhiteSpace(segment)) {
            return empty();
        }
        else {
            const segment_1 = trimEnd(segment, "#");
            if (segment_1 === "?") {
                return empty();
            }
            else {
                const activePatternResult8647 = RouterModule_String_$007CPrefix$007C("?", segment_1);
                if (activePatternResult8647 != null) {
                    return singleton(segment_1);
                }
                else {
                    const matchValue = segment_1.split("?");
                    if ((!equalsWith(comparePrimitives, matchValue, null)) ? (matchValue.length === 1) : false) {
                        const value = matchValue[0];
                        return singleton(decodeURIComponent(value));
                    }
                    else if ((!equalsWith(comparePrimitives, matchValue, null)) ? (matchValue.length === 2) : false) {
                        if (matchValue[1] === "") {
                            const value_1 = matchValue[0];
                            return singleton(decodeURIComponent(value_1));
                        }
                        else {
                            const value_2 = matchValue[0];
                            const query = matchValue[1];
                            return ofArray([decodeURIComponent(value_2), "?" + query]);
                        }
                    }
                    else {
                        return empty();
                    }
                }
            }
        }
    }, list);
}

export function RouterModule_onUrlChange(routeMode, urlChanged, ev) {
    let path;
    return urlChanged((path = ((routeMode === 2) ? (window.location.pathname + window.location.search) : window.location.hash), (RouterModule_urlSegments(path, routeMode))));
}

export const RouterModule_router = React_memo_62A0F746((input) => {
    const onChange = useReact_useCallbackRef_7C4B0DD6((ev) => {
        const urlChanged = defaultArg(input.onUrlChanged, (value) => {
            void value;
        });
        const routeMode = defaultArg(input.hashMode, 1) | 0;
        RouterModule_onUrlChange(routeMode, urlChanged, ev);
    });
    if (((window.navigator.userAgent).indexOf("Trident") >= 0) ? true : ((window.navigator.userAgent).indexOf("MSIE") >= 0)) {
        const eventType = "hashchange";
        const action_1 = (arg00) => {
            onChange(arg00);
        };
        const options_1 = void 0;
        const addOptions = useReact_useMemo_CF4EA67(() => Impl_adjustPassive(options_1), [options_1]);
        const removeOptions = useReact_useMemo_CF4EA67(() => Impl_createRemoveOptions(options_1), [options_1]);
        const fn = useReact_useMemo_CF4EA67(() => ((arg) => {
            action_1((arg));
        }), [action_1]);
        const listener = useReact_useCallbackRef_7C4B0DD6(() => {
            if (addOptions == null) {
                window.addEventListener(eventType, fn);
            }
            else {
                const options_2 = addOptions;
                window.addEventListener(eventType, fn, options_2);
            }
            return React_createDisposable_3A5B6456(() => {
                if (removeOptions == null) {
                    window.removeEventListener(eventType, fn);
                }
                else {
                    const options_3 = removeOptions;
                    window.removeEventListener(eventType, fn, options_3);
                }
            });
        });
        useReact_useEffect_Z5ECA432F(listener);
    }
    else {
        const eventType_1 = "popstate";
        const action_3 = onChange;
        const options_5 = void 0;
        const addOptions_1 = useReact_useMemo_CF4EA67(() => Impl_adjustPassive(options_5), [options_5]);
        const removeOptions_1 = useReact_useMemo_CF4EA67(() => Impl_createRemoveOptions(options_5), [options_5]);
        const fn_1 = useReact_useMemo_CF4EA67(() => ((arg_1) => {
            action_3((arg_1));
        }), [action_3]);
        const listener_1 = useReact_useCallbackRef_7C4B0DD6(() => {
            if (addOptions_1 == null) {
                window.addEventListener(eventType_1, fn_1);
            }
            else {
                const options_6 = addOptions_1;
                window.addEventListener(eventType_1, fn_1, options_6);
            }
            return React_createDisposable_3A5B6456(() => {
                if (removeOptions_1 == null) {
                    window.removeEventListener(eventType_1, fn_1);
                }
                else {
                    const options_7 = removeOptions_1;
                    window.removeEventListener(eventType_1, fn_1, options_7);
                }
            });
        });
        useReact_useEffect_Z5ECA432F(listener_1);
    }
    const eventType_2 = "CUSTOM_NAVIGATION_EVENT";
    const action_4 = onChange;
    const options_8 = void 0;
    const addOptions_2 = useReact_useMemo_CF4EA67(() => Impl_adjustPassive(options_8), [options_8]);
    const removeOptions_2 = useReact_useMemo_CF4EA67(() => Impl_createRemoveOptions(options_8), [options_8]);
    const fn_2 = useReact_useMemo_CF4EA67(() => ((arg_2) => {
        action_4((arg_2));
    }), [action_4]);
    const listener_2 = useReact_useCallbackRef_7C4B0DD6(() => {
        if (addOptions_2 == null) {
            window.addEventListener(eventType_2, fn_2);
        }
        else {
            const options_9 = addOptions_2;
            window.addEventListener(eventType_2, fn_2, options_9);
        }
        return React_createDisposable_3A5B6456(() => {
            if (removeOptions_2 == null) {
                window.removeEventListener(eventType_2, fn_2);
            }
            else {
                const options_10 = removeOptions_2;
                window.removeEventListener(eventType_2, fn_2, options_10);
            }
        });
    });
    useReact_useEffect_Z5ECA432F(listener_2);
    const matchValue = input.application;
    if (matchValue == null) {
        return null;
    }
    else {
        const elem = matchValue;
        return elem;
    }
});

export function Route_$007CInt$007C_$007C(input) {
    let matchValue;
    let outArg = 0;
    matchValue = [tryParse(input, 511, false, 32, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export function Route_$007CInt64$007C_$007C(input) {
    let matchValue;
    let outArg = fromInt(0);
    matchValue = [tryParse_1(input, 511, false, 64, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export function Route_$007CGuid$007C_$007C(input) {
    let matchValue;
    let outArg = "00000000-0000-0000-0000-000000000000";
    matchValue = [tryParse_2(input, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export function Route_$007CNumber$007C_$007C(input) {
    let matchValue;
    let outArg = 0;
    matchValue = [tryParse_3(input, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export function Route_$007CDecimal$007C_$007C(input) {
    let matchValue;
    let outArg = new Decimal(0);
    matchValue = [tryParse_4(input, new FSharpRef(() => outArg, (v) => {
        outArg = v;
    })), outArg];
    if (matchValue[0]) {
        return matchValue[1];
    }
    else {
        return void 0;
    }
}

export function Route_$007CBool$007C_$007C(input) {
    const matchValue = input.toLocaleLowerCase();
    switch (matchValue) {
        case "1":
        case "true": {
            return true;
        }
        case "0":
        case "false": {
            return false;
        }
        case "": {
            return true;
        }
        default: {
            return void 0;
        }
    }
}

export function Route_$007CQuery$007C_$007C(input) {
    try {
        const urlParams = new URLSearchParams(input);
        return ofSeq(delay(() => map_1((entry) => [entry[0], entry[1]], urlParams.entries())));
    }
    catch (matchValue) {
        return void 0;
    }
}

