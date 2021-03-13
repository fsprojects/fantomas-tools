import { class_type } from "../fable-library.3.1.7/Reflection.js";
import { value as value_1, defaultArg, some } from "../fable-library.3.1.7/Option.js";
import { int32ToString, curry } from "../fable-library.3.1.7/Util.js";
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
    const cache = new Map();
    if (typeof module === 'object' && module.hot) {
    module.hot.addStatusHandler(status => {
    if (status === 'apply') (() => {
        cache.clear();
    })();
    });
    };
    Cache.cache = cache;
})();

export function Cache_GetOrAdd_Z3AD3E68D(key, valueFactory) {
    if (Cache.cache.has(key)) {
        return Cache.cache.get(key);
    }
    else {
        const v = valueFactory(key);
        void Cache.cache.set(key, some(v));
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
    const prepareRenderFunction = (_arg1) => {
        const displayName_1 = defaultArg(displayName, value_1(__callingMemberName));
        render.displayName = displayName_1;
        let elemType;
        if (curry(2, memoizeWith) == null) {
            elemType = render;
        }
        else {
            const areEqual = memoizeWith;
            const areEqual_1 = (x, y) => {
                if (!(typeof module === 'object' && module.hot && module.hot.status() === 'apply')) {
                    return areEqual(x, y);
                }
                else {
                    return false;
                }
            };
            const memoElement = ReactElementTypeModule_memoWith(areEqual_1, render);
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
    };
    const cacheKey = (value_1(__callingSourceFile) + "#L") + int32ToString(value_1(__callingSourceLine));
    return Cache_GetOrAdd_Z3AD3E68D(cacheKey, prepareRenderFunction);
}

