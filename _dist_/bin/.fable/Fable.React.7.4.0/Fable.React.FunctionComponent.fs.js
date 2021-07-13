import { class_type } from "../fable-library.3.1.15/Reflection.js";
import { defaultArg, value as value_1 } from "../fable-library.3.1.15/Option.js";
import { curry, int32ToString } from "../fable-library.3.1.15/Util.js";
import { ReactElementTypeModule_memoWith } from "./Fable.React.Helpers.fs.js";
import * as react from "../../../../_snowpack/pkg/react.js";

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
    Cache.cache = (new Map());
})();

export function Cache_GetOrAdd_Z3AD3E68D(key, valueFactory) {
    if (Cache.cache.has(key)) {
        return Cache.cache.get(key);
    }
    else {
        const v = valueFactory(key);
        void Cache.cache.set(key, v);
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
    return Cache_GetOrAdd_Z3AD3E68D((value_1(__callingSourceFile) + "#L") + int32ToString(value_1(__callingSourceLine)), (_arg1) => {
        const displayName_1 = defaultArg(displayName, value_1(__callingMemberName));
        render.displayName = displayName_1;
        let elemType;
        if (curry(2, memoizeWith) == null) {
            elemType = render;
        }
        else {
            const memoElement = ReactElementTypeModule_memoWith(memoizeWith, render);
            memoElement.displayName = (("Memo(" + displayName_1) + ")");
            elemType = memoElement;
        }
        return (props) => {
            let f_1;
            return react.createElement(elemType, (withKey == null) ? props : (f_1 = withKey, (props.key = f_1(props), props)));
        };
    });
}

