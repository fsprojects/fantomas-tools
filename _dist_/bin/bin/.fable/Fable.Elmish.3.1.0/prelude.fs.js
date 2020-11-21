import { some } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import Timer from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Timer.js";
import { add } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Observable.js";

export function Log_onError(text, ex) {
    console.error(some(text), ex);
}

export function Log_toConsole(text, o) {
    console.log(some(text), o);
}

export function Timer_delay(interval, callback) {
    let t;
    let returnVal = new Timer(interval);
    returnVal.AutoReset = false;
    t = returnVal;
    add(callback, t.Elapsed);
    t.Enabled = true;
    t.Start();
}

