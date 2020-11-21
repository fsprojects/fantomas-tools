import { Union } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { class_type, union_type, int32_type, array_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";
import { fill } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { isDisposable, comparePrimitives, max } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { toArray, value as value_1, some } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { iterate as iterate_1, rangeNumber, singleton, collect, take, skip, append, delay } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Seq.js";
import { useReact_useMemo_CF4EA67, useReact_useEffect_3A5B6456, useReact_useEffect_Z101E1A95, useReact_useEffect_Z5234A374, useReact_useCallbackRef_7C4B0DD6, React_createDisposable_3A5B6456, useReact_useEffectOnce_Z5ECA432F, useFeliz_React__React_useState_Static_1505, useReact_useRef_1505 } from "../Feliz.1.17.0/React.fs.js";
import { isCancellationRequested, cancel, createCancellationToken } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Async.js";
import { promise } from "../Fable.Promise.2.1.0/PromiseImpl.fs.js";
import { PromiseBuilder__While_2044D34, PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "../Fable.Promise.2.1.0/Promise.fs.js";
import { iterate } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";

export class RingState$1 extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["Writable", "ReadWritable"];
    }
}

export function RingState$1$reflection(gen0) {
    return union_type("Feliz.UseElmish.RingState`1", [gen0], RingState$1, () => [[["wx", array_type(gen0)], ["ix", int32_type]], [["rw", array_type(gen0)], ["wix", int32_type], ["rix", int32_type]]]);
}

export class RingBuffer$1 {
    constructor(size) {
        this.state = (new RingState$1(0, fill(new Array(max(comparePrimitives, size, 10)), 0, max(comparePrimitives, size, 10), null), 0));
    }
}

export function RingBuffer$1$reflection(gen0) {
    return class_type("Feliz.UseElmish.RingBuffer`1", [gen0], RingBuffer$1);
}

export function RingBuffer$1_$ctor_Z524259A4(size) {
    return new RingBuffer$1(size);
}

export function RingBuffer$1__Pop(_) {
    const matchValue = _.state;
    if (matchValue.tag === 1) {
        const wix = matchValue.fields[1] | 0;
        const rix = matchValue.fields[2] | 0;
        const items = matchValue.fields[0];
        const rix$0027 = ((rix + 1) % items.length) | 0;
        const matchValue_1 = rix$0027 === wix;
        if (matchValue_1) {
            _.state = (new RingState$1(0, items, wix));
        }
        else {
            _.state = (new RingState$1(1, items, wix, rix$0027));
        }
        return some(items[rix]);
    }
    else {
        return void 0;
    }
}

export function RingBuffer$1__Push_2B595(_, item) {
    const matchValue = _.state;
    if (matchValue.tag === 1) {
        const wix_1 = matchValue.fields[1] | 0;
        const rix = matchValue.fields[2] | 0;
        const items_1 = matchValue.fields[0];
        items_1[wix_1] = item;
        const wix$0027 = ((wix_1 + 1) % items_1.length) | 0;
        const matchValue_1 = wix$0027 === rix;
        if (matchValue_1) {
            _.state = (new RingState$1(1, (RingBuffer$1__doubleSize(_, rix, items_1)), items_1.length, 0));
        }
        else {
            _.state = (new RingState$1(1, items_1, wix$0027, rix));
        }
    }
    else {
        const ix = matchValue.fields[1] | 0;
        const items = matchValue.fields[0];
        items[ix] = item;
        const wix = ((ix + 1) % items.length) | 0;
        _.state = (new RingState$1(1, items, wix, ix));
    }
}

function RingBuffer$1__doubleSize(this$, ix, items) {
    const source_2 = delay(() => append((skip(ix, items)), delay(() => append((take(ix, items)), delay(() => collect((matchValue) => singleton(null), rangeNumber(0, 1, items.length)))))));
    return Array.from(source_2);
}

export function useFeliz_React__React_useElmish_Static_17DC4F1D(init, update, dependencies) {
    const state = useReact_useRef_1505(init[0]);
    const ring = useReact_useRef_1505(RingBuffer$1_$ctor_Z524259A4(10));
    const patternInput = useFeliz_React__React_useState_Static_1505(init[0]);
    let token_1;
    const cts = useReact_useRef_1505(createCancellationToken());
    const token = useReact_useRef_1505(cts.current);
    useReact_useEffectOnce_Z5ECA432F(() => React_createDisposable_3A5B6456(() => {
        cancel(cts.current);
    }));
    token_1 = token;
    const setChildState_1 = () => {
        const value = setTimeout(() => {
            let copyOfStruct;
            if (!(copyOfStruct = token_1.current, isCancellationRequested(copyOfStruct))) {
                patternInput[1](state.current);
            }
        }, 0) | 0;
        void value;
    };
    const dispatch = (msg) => {
        let pr;
        const builder$0040 = promise;
        pr = PromiseBuilder__Run_212F1D4B(builder$0040, PromiseBuilder__Delay_62FBFDE1(builder$0040, () => {
            let nextMsg = some(msg);
            return PromiseBuilder__While_2044D34(builder$0040, () => {
                let copyOfStruct_1;
                return (nextMsg != null) ? (!(copyOfStruct_1 = token_1.current, isCancellationRequested(copyOfStruct_1))) : false;
            }, PromiseBuilder__Delay_62FBFDE1(builder$0040, () => {
                const msg_1 = value_1(nextMsg);
                const patternInput_1 = update(msg_1, state.current);
                iterate((sub) => {
                    sub(dispatch);
                }, patternInput_1[1]);
                nextMsg = RingBuffer$1__Pop(ring.current);
                state.current = patternInput_1[0];
                setChildState_1();
                return Promise.resolve();
            }));
        }));
        pr.then();
    };
    const dispatch_1 = useReact_useCallbackRef_7C4B0DD6(dispatch);
    useReact_useEffect_Z5234A374(() => React_createDisposable_3A5B6456(() => {
        let option;
        const record = state.current;
        const matchValue = record;
        option = (isDisposable(matchValue) ? matchValue : (void 0));
        iterate_1((o) => {
            o.Dispose();
        }, toArray(option));
    }), dependencies);
    useReact_useEffect_Z101E1A95(() => {
        state.current = init[0];
        setChildState_1();
        iterate((sub_1) => {
            sub_1(dispatch_1);
        }, init[1]);
    }, dependencies);
    useReact_useEffect_3A5B6456(() => {
        const option_1 = RingBuffer$1__Pop(ring.current);
        iterate_1(dispatch_1, toArray(option_1));
    });
    return [patternInput[0], dispatch_1];
}

export function useFeliz_React__React_useElmish_Static_645B1FB7(init, update, dependencies) {
    const init_1 = useReact_useMemo_CF4EA67(init, dependencies);
    return useFeliz_React__React_useElmish_Static_17DC4F1D(init_1, update, dependencies);
}

