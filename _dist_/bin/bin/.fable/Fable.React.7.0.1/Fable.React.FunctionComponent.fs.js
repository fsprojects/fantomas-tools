import { class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { defaultArg, some } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { curry, int32ToString } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { ReactElementTypeModule_memoWith } from "./Fable.React.Helpers.fs.js";
import * as react from "../../../../../web_modules/react.js";

export class Cache {
    constructor() {
    }
}

export function Cache$reflection() {
    return class_type("Fable.React.Cache", void 0, Cache);
}

export function Cache_$ctor() {
    return new Cache();
}

(() => {
    const cache = new Map();
    Cache.cache = cache;
})();

export function Cache_GetOrAdd_Z3AD3E68D(key, valueFactory) {
    if (Cache.cache.has(key)) {
        return Cache.cache.get(key);
    }
    else {
        const v = valueFactory(key);
        const value = Cache.cache.set(key, some(v));
        void value;
        return v;
    }
}

export class FunctionComponent {
    constructor() {
    }
}

export function FunctionComponent$reflection() {
    return class_type("Fable.React.FunctionComponent", void 0, FunctionComponent);
}

export function FunctionComponent_Of_Z5A158BBF(render, displayName, memoizeWith, withKey, __callingMemberName, __callingSourceFile, __callingSourceLine) {
    const cacheKey = (__callingSourceFile + "#L") + int32ToString(__callingSourceLine);
    return Cache_GetOrAdd_Z3AD3E68D(cacheKey, (_arg1) => {
        const displayName_1 = defaultArg(displayName, __callingMemberName);
        render.displayName = displayName_1;
        let elemType;
        if (curry(2, memoizeWith) == null) {
            elemType = render;
        }
        else {
            const areEqual = memoizeWith;
            const memoElement = ReactElementTypeModule_memoWith(areEqual, render);
            memoElement.displayName = (("Memo(" + displayName_1) + ")");
            elemType = memoElement;
        }
        return (props) => {
            let props_1;
            if (withKey == null) {
                props_1 = props;
            }
            else {
                const f_1 = withKey;
                props.key = f_1(props);
                props_1 = props;
            }
            return react.createElement(elemType, props_1);
        };
    });
}

