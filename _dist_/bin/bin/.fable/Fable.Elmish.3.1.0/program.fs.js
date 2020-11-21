import { Record } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { record_type, class_type, string_type, tuple_type, list_type, lambda_type, unit_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { Cmd_exec, Cmd_batch, Cmd_none } from "./cmd.fs.js";
import { Log_toConsole, Log_onError } from "./prelude.fs.js";
import { curry, partialApply, uncurry } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { ofArray } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { RingBuffer$1__Pop, RingBuffer$1__Push_2B595, RingBuffer$1_$ctor_Z524259A4 } from "./ring.fs.js";
import { value as value_1, some } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { printf, toText } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";

export class Program$4 extends Record {
    constructor(init, update, subscribe, view, setState, onError, syncDispatch) {
        super();
        this.init = init;
        this.update = update;
        this.subscribe = subscribe;
        this.view = view;
        this.setState = setState;
        this.onError = onError;
        this.syncDispatch = syncDispatch;
    }
}

export function Program$4$reflection(gen0, gen1, gen2, gen3) {
    return record_type("Elmish.Program`4", [gen0, gen1, gen2, gen3], Program$4, () => [["init", lambda_type(gen0, tuple_type(gen1, list_type(lambda_type(lambda_type(gen2, unit_type), unit_type))))], ["update", lambda_type(gen2, lambda_type(gen1, tuple_type(gen1, list_type(lambda_type(lambda_type(gen2, unit_type), unit_type)))))], ["subscribe", lambda_type(gen1, list_type(lambda_type(lambda_type(gen2, unit_type), unit_type)))], ["view", lambda_type(gen1, lambda_type(lambda_type(gen2, unit_type), gen3))], ["setState", lambda_type(gen1, lambda_type(lambda_type(gen2, unit_type), unit_type))], ["onError", lambda_type(tuple_type(string_type, class_type("System.Exception")), unit_type)], ["syncDispatch", lambda_type(lambda_type(gen2, unit_type), lambda_type(gen2, unit_type))]]);
}

export function ProgramModule_mkProgram(init, update, view) {
    return new Program$4(init, update, (_arg1) => Cmd_none(), view, (model, arg) => {
        const value = view(model, arg);
        void value;
    }, (tupledArg) => {
        Log_onError(tupledArg[0], tupledArg[1]);
    }, uncurry(2, (x) => x));
}

export function ProgramModule_mkSimple(init, update, view) {
    return new Program$4((arg) => {
        const state = init(arg);
        return [state, Cmd_none()];
    }, (msg, arg_1) => {
        const state_1 = update(msg, arg_1);
        return [state_1, Cmd_none()];
    }, (_arg1) => Cmd_none(), view, (model, arg_2) => {
        const value = view(model, arg_2);
        void value;
    }, (tupledArg) => {
        Log_onError(tupledArg[0], tupledArg[1]);
    }, uncurry(2, (x) => x));
}

export function ProgramModule_withSubscription(subscribe, program) {
    return new Program$4(program.init, program.update, (model) => Cmd_batch(ofArray([program.subscribe(model), subscribe(model)])), program.view, program.setState, program.onError, program.syncDispatch);
}

export function ProgramModule_withConsoleTrace(program) {
    return new Program$4((arg) => {
        const patternInput = program.init(arg);
        const initModel = patternInput[0];
        Log_toConsole("Initial state:", initModel);
        return [initModel, patternInput[1]];
    }, (msg, model) => {
        Log_toConsole("New message:", msg);
        const patternInput_1 = program.update(msg, model);
        const newModel = patternInput_1[0];
        Log_toConsole("Updated state:", newModel);
        return [newModel, patternInput_1[1]];
    }, program.subscribe, program.view, program.setState, program.onError, program.syncDispatch);
}

export function ProgramModule_withTrace(trace, program) {
    return new Program$4(program.init, (msg, model) => {
        const patternInput = program.update(msg, model);
        const state = patternInput[0];
        trace(msg, state);
        return [state, patternInput[1]];
    }, program.subscribe, program.view, program.setState, program.onError, program.syncDispatch);
}

export function ProgramModule_withErrorHandler(onError, program) {
    return new Program$4(program.init, program.update, program.subscribe, program.view, program.setState, onError, program.syncDispatch);
}

export function ProgramModule_mapErrorHandler(map, program) {
    const onError = partialApply(1, map, [program.onError]);
    return new Program$4(program.init, program.update, program.subscribe, program.view, program.setState, onError, program.syncDispatch);
}

export function ProgramModule_onError(program) {
    return program.onError;
}

export function ProgramModule_withSetState(setState, program) {
    return new Program$4(program.init, program.update, program.subscribe, program.view, setState, program.onError, program.syncDispatch);
}

export function ProgramModule_setState(program) {
    return curry(2, program.setState);
}

export function ProgramModule_view(program) {
    return curry(2, program.view);
}

export function ProgramModule_withSyncDispatch(syncDispatch, program) {
    return new Program$4(program.init, program.update, program.subscribe, program.view, program.setState, program.onError, syncDispatch);
}

export function ProgramModule_map(mapInit, mapUpdate, mapView, mapSetState, mapSubscribe, program) {
    const init = partialApply(1, mapInit, [program.init]);
    const update = partialApply(2, mapUpdate, [program.update]);
    const view = partialApply(2, mapView, [program.view]);
    const setState = partialApply(2, mapSetState, [program.setState]);
    return new Program$4(init, uncurry(2, update), partialApply(1, mapSubscribe, [program.subscribe]), uncurry(2, view), uncurry(2, setState), program.onError, uncurry(2, (x) => x));
}

export function ProgramModule_runWith(arg, program) {
    const patternInput = program.init(arg);
    const model = patternInput[0];
    const rb = RingBuffer$1_$ctor_Z524259A4(10);
    let reentered = false;
    let state = model;
    const dispatch = (msg) => {
        let clo1_1;
        if (reentered) {
            RingBuffer$1__Push_2B595(rb, msg);
        }
        else {
            reentered = true;
            let nextMsg = some(msg);
            while (nextMsg != null) {
                const msg_1 = value_1(nextMsg);
                try {
                    const patternInput_1 = program.update(msg_1, state);
                    const model$0027 = patternInput_1[0];
                    program.setState(model$0027, syncDispatch);
                    Cmd_exec((ex) => {
                        let clo1;
                        program.onError([(clo1 = toText(printf("Error in command while handling: %A")), clo1(msg_1)), ex]);
                    }, syncDispatch, patternInput_1[1]);
                    state = model$0027;
                }
                catch (ex_1) {
                    program.onError([(clo1_1 = toText(printf("Unable to process the message: %A")), clo1_1(msg_1)), ex_1]);
                }
                nextMsg = RingBuffer$1__Pop(rb);
            }
            reentered = false;
        }
    };
    const syncDispatch = partialApply(1, program.syncDispatch, [dispatch]);
    program.setState(model, syncDispatch);
    let sub;
    try {
        sub = program.subscribe(model);
    }
    catch (ex_2) {
        program.onError(["Unable to subscribe:", ex_2]);
        sub = Cmd_none();
    }
    const cmd_2 = Cmd_batch(ofArray([sub, patternInput[1]]));
    Cmd_exec((ex_3) => {
        program.onError(["Error intitializing:", ex_3]);
    }, syncDispatch, cmd_2);
}

export function ProgramModule_run(program) {
    ProgramModule_runWith(void 0, program);
}

