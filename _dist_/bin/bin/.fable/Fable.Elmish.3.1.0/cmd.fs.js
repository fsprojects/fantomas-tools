import { singleton, concat, map, empty, iterate } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { singleton as singleton_1 } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/AsyncBuilder.js";
import { startImmediate, catchAsync } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Async.js";
import { Timer_delay } from "./prelude.fs.js";

export function Cmd_exec(onError, dispatch, cmd) {
    iterate((call) => {
        try {
            call(dispatch);
        }
        catch (ex) {
            onError(ex);
        }
    }, cmd);
}

export function Cmd_none() {
    return empty();
}

export function Cmd_map(f, cmd) {
    return map((g) => ((arg_1) => {
        g((arg) => {
            arg_1(f(arg));
        });
    }), cmd);
}

export function Cmd_batch(cmds) {
    return concat(cmds);
}

export function Cmd_ofSub(sub) {
    return singleton(sub);
}

export function Cmd_OfFunc_either(task, arg, ofSuccess, ofError) {
    return singleton((dispatch) => {
        try {
            const arg_1 = task(arg);
            return dispatch(ofSuccess(arg_1));
        }
        catch (x) {
            return dispatch(ofError(x));
        }
    });
}

export function Cmd_OfFunc_perform(task, arg, ofSuccess) {
    return singleton((dispatch) => {
        try {
            const arg_1 = task(arg);
            dispatch(ofSuccess(arg_1));
        }
        catch (x) {
        }
    });
}

export function Cmd_OfFunc_attempt(task, arg, ofError) {
    return singleton((dispatch) => {
        try {
            task(arg);
        }
        catch (x) {
            dispatch(ofError(x));
        }
    });
}

export function Cmd_OfFunc_result(msg) {
    return singleton((dispatch) => {
        dispatch(msg);
    });
}

export function Cmd_OfAsyncWith_either(start, task, arg, ofSuccess, ofError) {
    return singleton((arg_1) => {
        let builder$0040;
        start((builder$0040 = singleton_1, builder$0040.Delay(() => {
            let arg00;
            return builder$0040.Bind((arg00 = task(arg), (catchAsync(arg00))), (_arg1) => {
                const r = _arg1;
                arg_1((r.tag === 1) ? ofError(r.fields[0]) : ofSuccess(r.fields[0]));
                return builder$0040.Zero();
            });
        })));
    });
}

export function Cmd_OfAsyncWith_perform(start, task, arg, ofSuccess) {
    return singleton((arg_1) => {
        let builder$0040;
        start((builder$0040 = singleton_1, builder$0040.Delay(() => {
            let arg00;
            return builder$0040.Bind((arg00 = task(arg), (catchAsync(arg00))), (_arg1) => {
                const r = _arg1;
                if (r.tag === 0) {
                    arg_1(ofSuccess(r.fields[0]));
                    return builder$0040.Zero();
                }
                else {
                    return builder$0040.Zero();
                }
            });
        })));
    });
}

export function Cmd_OfAsyncWith_attempt(start, task, arg, ofError) {
    return singleton((arg_1) => {
        let builder$0040;
        start((builder$0040 = singleton_1, builder$0040.Delay(() => {
            let arg00;
            return builder$0040.Bind((arg00 = task(arg), (catchAsync(arg00))), (_arg1) => {
                const r = _arg1;
                if (r.tag === 1) {
                    arg_1(ofError(r.fields[0]));
                    return builder$0040.Zero();
                }
                else {
                    return builder$0040.Zero();
                }
            });
        })));
    });
}

export function Cmd_OfAsyncWith_result(start, task) {
    return singleton((arg) => {
        let builder$0040;
        start((builder$0040 = singleton_1, builder$0040.Delay(() => builder$0040.Bind(task, (_arg1) => {
            arg(_arg1);
            return builder$0040.Zero();
        }))));
    });
}

export function Cmd_OfAsync_start(x) {
    Timer_delay(0, (_arg1) => {
        startImmediate(x);
    });
}

export function Cmd_OfPromise_either(task, arg, ofSuccess, ofError) {
    return singleton((dispatch) => {
        const value_1 = task(arg).then((arg_1) => dispatch(ofSuccess(arg_1))).catch((arg_3) => dispatch((ofError((arg_3)))));
        void value_1;
    });
}

export function Cmd_OfPromise_perform(task, arg, ofSuccess) {
    return singleton((dispatch) => {
        const value = task(arg).then((arg_1) => dispatch(ofSuccess(arg_1)));
        void value;
    });
}

export function Cmd_OfPromise_attempt(task, arg, ofError) {
    return singleton((dispatch) => {
        const value_1 = task(arg).catch((arg_2) => {
            dispatch((ofError((arg_2))));
        });
        void value_1;
    });
}

export function Cmd_OfPromise_result(task) {
    return singleton((dispatch) => {
        const value = task.then(dispatch);
        void value;
    });
}

export function Cmd_attemptFunc(task, arg, ofError) {
    return Cmd_OfFunc_attempt(task, arg, ofError);
}

