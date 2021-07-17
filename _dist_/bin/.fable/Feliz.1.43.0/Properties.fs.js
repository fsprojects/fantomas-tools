import { join } from "../fable-library.3.2.9/String.js";
import { length, ofArray, map } from "../fable-library.3.2.9/List.js";
import { milliseconds, seconds, minutes, hours } from "../fable-library.3.2.9/TimeSpan.js";
import { map as map_1 } from "../fable-library.3.2.9/Seq.js";

export function PropHelpers_createClockValue(duration) {
    let i_1;
    return (join(":", map((i) => ((i < 10) ? ("0" + i) : i), ofArray([hours(duration), minutes(duration), seconds(duration)]))) + ".") + ((i_1 = (milliseconds(duration) | 0), (i_1 < 10) ? ("0" + i_1) : i_1));
}

export function PropHelpers_createKeySplines(values) {
    return join("; ", map_1((tupledArg) => ((((((tupledArg[0] + " ") + tupledArg[1]) + " ") + tupledArg[2]) + " ") + tupledArg[3]), values));
}

export function PropHelpers_createOnKey(key, handler, ev) {
    const patternInput = key;
    const pressedKey = patternInput[0];
    const matchValue = [patternInput[1], patternInput[2]];
    let pattern_matching_result;
    if (matchValue[0]) {
        if (matchValue[1]) {
            if (((pressedKey.toLocaleLowerCase() === ev.key.toLocaleLowerCase()) ? ev.ctrlKey : false) ? ev.shiftKey : false) {
                pattern_matching_result = 0;
            }
            else {
                pattern_matching_result = 1;
            }
        }
        else {
            pattern_matching_result = 1;
        }
    }
    else {
        pattern_matching_result = 1;
    }
    switch (pattern_matching_result) {
        case 0: {
            handler(ev);
            break;
        }
        case 1: {
            let pattern_matching_result_1;
            if (matchValue[0]) {
                if (matchValue[1]) {
                    pattern_matching_result_1 = 1;
                }
                else if ((pressedKey.toLocaleLowerCase() === ev.key.toLocaleLowerCase()) ? ev.ctrlKey : false) {
                    pattern_matching_result_1 = 0;
                }
                else {
                    pattern_matching_result_1 = 1;
                }
            }
            else {
                pattern_matching_result_1 = 1;
            }
            switch (pattern_matching_result_1) {
                case 0: {
                    handler(ev);
                    break;
                }
                case 1: {
                    let pattern_matching_result_2;
                    if (matchValue[0]) {
                        pattern_matching_result_2 = 1;
                    }
                    else if (matchValue[1]) {
                        if ((pressedKey.toLocaleLowerCase() === ev.key.toLocaleLowerCase()) ? ev.shiftKey : false) {
                            pattern_matching_result_2 = 0;
                        }
                        else {
                            pattern_matching_result_2 = 1;
                        }
                    }
                    else {
                        pattern_matching_result_2 = 1;
                    }
                    switch (pattern_matching_result_2) {
                        case 0: {
                            handler(ev);
                            break;
                        }
                        case 1: {
                            let pattern_matching_result_3;
                            if (matchValue[0]) {
                                pattern_matching_result_3 = 1;
                            }
                            else if (matchValue[1]) {
                                pattern_matching_result_3 = 1;
                            }
                            else if (pressedKey.toLocaleLowerCase() === ev.key.toLocaleLowerCase()) {
                                pattern_matching_result_3 = 0;
                            }
                            else {
                                pattern_matching_result_3 = 1;
                            }
                            switch (pattern_matching_result_3) {
                                case 0: {
                                    handler(ev);
                                    break;
                                }
                                case 1: {
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            break;
        }
    }
}

export function PropHelpers_createPointsFloat(coordinates) {
    return join(" ", map_1((tupledArg) => ((tupledArg[0] + ",") + tupledArg[1]), coordinates));
}

export function PropHelpers_createPointsInt(coordinates) {
    return join(" ", map_1((tupledArg) => ((tupledArg[0] + ",") + tupledArg[1]), coordinates));
}

export function PropHelpers_createSvgPathFloat(path) {
    return join("\n", map_1((tupledArg) => {
        const cmdType = tupledArg[0];
        const cmds = tupledArg[1];
        if (length(cmds) === 0) {
            return cmdType;
        }
        else {
            return (cmdType + " ") + join(" ", map_1((arg) => join(",", arg), cmds));
        }
    }, path));
}

export function PropHelpers_createSvgPathInt(path) {
    return join("\n", map_1((tupledArg) => {
        const cmdType = tupledArg[0];
        const cmds = tupledArg[1];
        if (length(cmds) === 0) {
            return cmdType;
        }
        else {
            return (cmdType + " ") + join(" ", map_1((arg) => join(",", arg), cmds));
        }
    }, path));
}

