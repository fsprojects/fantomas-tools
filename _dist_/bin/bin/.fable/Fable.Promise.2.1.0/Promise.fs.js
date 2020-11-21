import { Result_MapError, Result_Map, FSharpResult$2 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Choice.js";
import { class_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { getEnumerator } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { equals } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";

export function reject(reason) {
    return Promise.reject(reason);
}

export function result(a) {
    return a.then(((arg) => {
        let arg0;
        arg0 = (new FSharpResult$2(0, arg));
        return arg0;
    }),((arg_1) => {
        let arg0_2;
        arg0_2 = (new FSharpResult$2(1, arg_1));
        return arg0_2;
    }));
}

export function mapResult(fn, a) {
    return a.then(((result_1) => Result_Map(fn, result_1)));
}

export function bindResult(fn, a) {
    return a.then(((a_1) => {
        if (a_1.tag === 1) {
            return Promise.resolve((new FSharpResult$2(1, a_1.fields[0])));
        }
        else {
            return result(fn(a_1.fields[0]));
        }
    }));
}

export function mapResultError(fn, a) {
    return a.then(((result_1) => Result_MapError(fn, result_1)));
}

export function tap(fn, a) {
    return a.then(((x) => {
        fn(x);
        return x;
    }));
}

export class PromiseBuilder {
    constructor() {
    }
}

export function PromiseBuilder$reflection() {
    return class_type("Promise.PromiseBuilder", void 0, PromiseBuilder);
}

export function PromiseBuilder_$ctor() {
    return new PromiseBuilder();
}

export function PromiseBuilder__For_1565554B(x, seq, body) {
    let p = Promise.resolve(undefined);
    const enumerator = getEnumerator(seq);
    try {
        while (enumerator["System.Collections.IEnumerator.MoveNext"]()) {
            const a = enumerator["System.Collections.Generic.IEnumerator`1.get_Current"]();
            const x_1 = p.then((() => body(a)));
            p = x_1;
        }
    }
    finally {
        enumerator.Dispose();
    }
    return p;
}

export function PromiseBuilder__While_2044D34(x, guard, p) {
    if (guard()) {
        return p.then((() => PromiseBuilder__While_2044D34(x, guard, p)));
    }
    else {
        return Promise.resolve(undefined);
    }
}

export function PromiseBuilder__TryFinally_7D49A2FD(x, p, compensation) {
    return p.then(((x_1) => {
        compensation();
        return x_1;
    }),((er) => {
        compensation();
        throw er;
    }));
}

export function PromiseBuilder__Delay_62FBFDE1(x, generator) {
    const x_6 = {
        then: (f1, f2) => {
            try {
                return generator().then(f1, f2);
            }
            catch (er) {
                if (equals(f2, null)) {
                    const x_1 = Promise.reject(er);
                    return x_1;
                }
                else {
                    try {
                        const x_2 = Promise.resolve(f2(er));
                        return x_2;
                    }
                    catch (er_1) {
                        const x_3 = Promise.reject(er_1);
                        return x_3;
                    }
                }
            }
        },
        catch: (f) => {
            try {
                return generator().catch(f);
            }
            catch (er_2) {
                try {
                    const x_4 = Promise.resolve(f(er_2));
                    return x_4;
                }
                catch (er_3) {
                    const x_5 = Promise.reject(er_3);
                    return x_5;
                }
            }
        },
    };
    return x_6;
}

export function PromiseBuilder__Run_212F1D4B(x, p) {
    return new Promise(((success, fail) => {
        try {
            let p_1;
            const x_1 = Promise.resolve(p);
            p_1 = x_1;
            p_1.then(success, fail);
        }
        catch (er) {
            fail(er);
        }
    }));
}

export function PromiseBuilder__Using_74F7E79D(x, resource, binder) {
    return PromiseBuilder__TryFinally_7D49A2FD(x, binder(resource), () => {
        let copyOfStruct = resource;
        copyOfStruct.Dispose();
    });
}

