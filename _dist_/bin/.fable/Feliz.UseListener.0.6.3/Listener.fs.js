import { bind, map } from "../fable-library.3.1.1/Option.js";

export const Impl_allowsPassiveEvents = (() => {
    let passive = false;
    try {
        if ((typeof window !== 'undefined') ? (typeof window.addEventListener === 'function') : false) {
            window.addEventListener("testPassiveEventSupport", (value) => {
                void value;
            }, {
                passive: true,
            });
            window.removeEventListener("testPassiveEventSupport", (value_1) => {
                void value_1;
            });
        }
    }
    catch (matchValue) {
    }
    return passive;
})();

export const Impl_defaultPassive = {
    passive: true,
};

export function Impl_adjustPassive(maybeOptions) {
    return map((options) => {
        if (options.passive ? (!Impl_allowsPassiveEvents) : false) {
            return {
                capture: options.capture,
                once: options.once,
                passive: false,
            };
        }
        else {
            return options;
        }
    }, maybeOptions);
}

export function Impl_createRemoveOptions(maybeOptions) {
    return bind((options) => {
        if (options.capture) {
            return {
                capture: true,
            };
        }
        else {
            return void 0;
        }
    }, maybeOptions);
}

