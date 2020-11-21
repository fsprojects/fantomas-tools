import { Record } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Types.js";
import { record_type, string_type } from "../../../.fable/fable-library.3.0.0-nagareyama-rc-008/Reflection.js";

export class FragmentProps extends Record {
    constructor(key) {
        super();
        this.key = key;
    }
}

export function FragmentProps$reflection() {
    return record_type("Fable.React.FragmentProps", [], FragmentProps, () => [["key", string_type]]);
}

