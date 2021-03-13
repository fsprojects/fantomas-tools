import { Record } from "../fable-library.3.1.7/Types.js";
import { record_type, string_type } from "../fable-library.3.1.7/Reflection.js";

export class FragmentProps extends Record {
    constructor(key) {
        super();
        this.key = key;
    }
}

export function FragmentProps$reflection() {
    return record_type("Fable.React.FragmentProps", [], FragmentProps, () => [["key", string_type]]);
}

