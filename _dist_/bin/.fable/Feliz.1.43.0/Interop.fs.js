import { some } from "../fable-library.3.1.15/Option.js";
import { create, isLeapYear } from "../fable-library.3.1.15/Date.js";
import { parse } from "../fable-library.3.1.15/Int32.js";
import { isNullOrWhiteSpace } from "../fable-library.3.1.15/String.js";
import { equalsWith } from "../fable-library.3.1.15/Array.js";
import { comparePrimitives } from "../fable-library.3.1.15/Util.js";
import react from "../../../../_snowpack/pkg/react.js";

export function DateParsing_$007CBetween$007C_$007C(x, y, input) {
    if ((input >= x) ? (input <= y) : false) {
        return some(void 0);
    }
    else {
        return void 0;
    }
}

export function DateParsing_isLeapYear(year) {
    return isLeapYear(year);
}

export function DateParsing_$007CInt$007C_$007C(input) {
    try {
        return parse(input, 511, false, 32);
    }
    catch (matchValue) {
        return void 0;
    }
}

export function DateParsing_parse(input) {
    try {
        if (isNullOrWhiteSpace(input)) {
            return void 0;
        }
        else {
            const parts = input.split("-");
            let patternInput;
            let pattern_matching_result, month, year;
            if ((!equalsWith((x, y) => comparePrimitives(x, y), parts, null)) ? (parts.length === 2) : false) {
                const activePatternResult3241 = DateParsing_$007CInt$007C_$007C(parts[0]);
                if (activePatternResult3241 != null) {
                    const activePatternResult3242 = DateParsing_$007CInt$007C_$007C(parts[1]);
                    if (activePatternResult3242 != null) {
                        pattern_matching_result = 0;
                        month = activePatternResult3242;
                        year = activePatternResult3241;
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
                    patternInput = [year, month, 1, 0, 0];
                    break;
                }
                case 1: {
                    let pattern_matching_result_1, day, month_1, year_1;
                    if ((!equalsWith((x_1, y_1) => comparePrimitives(x_1, y_1), parts, null)) ? (parts.length === 3) : false) {
                        const activePatternResult3238 = DateParsing_$007CInt$007C_$007C(parts[0]);
                        if (activePatternResult3238 != null) {
                            const activePatternResult3239 = DateParsing_$007CInt$007C_$007C(parts[1]);
                            if (activePatternResult3239 != null) {
                                const activePatternResult3240 = DateParsing_$007CInt$007C_$007C(parts[2]);
                                if (activePatternResult3240 != null) {
                                    pattern_matching_result_1 = 0;
                                    day = activePatternResult3240;
                                    month_1 = activePatternResult3239;
                                    year_1 = activePatternResult3238;
                                }
                                else {
                                    pattern_matching_result_1 = 1;
                                }
                            }
                            else {
                                pattern_matching_result_1 = 1;
                            }
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
                            patternInput = [year_1, month_1, day, 0, 0];
                            break;
                        }
                        case 1: {
                            let pattern_matching_result_2, day_1, month_2, year_2;
                            if ((!equalsWith((x_2, y_2) => comparePrimitives(x_2, y_2), parts, null)) ? (parts.length === 3) : false) {
                                const activePatternResult3236 = DateParsing_$007CInt$007C_$007C(parts[0]);
                                if (activePatternResult3236 != null) {
                                    const activePatternResult3237 = DateParsing_$007CInt$007C_$007C(parts[1]);
                                    if (activePatternResult3237 != null) {
                                        pattern_matching_result_2 = 0;
                                        day_1 = parts[2];
                                        month_2 = activePatternResult3237;
                                        year_2 = activePatternResult3236;
                                    }
                                    else {
                                        pattern_matching_result_2 = 1;
                                    }
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
                                    if (day_1.indexOf("T") >= 0) {
                                        const matchValue = day_1.split("T");
                                        let pattern_matching_result_3, parsedDay, time;
                                        if ((!equalsWith((x_3, y_3) => comparePrimitives(x_3, y_3), matchValue, null)) ? (matchValue.length === 2) : false) {
                                            const activePatternResult3235 = DateParsing_$007CInt$007C_$007C(matchValue[0]);
                                            if (activePatternResult3235 != null) {
                                                pattern_matching_result_3 = 0;
                                                parsedDay = activePatternResult3235;
                                                time = matchValue[1];
                                            }
                                            else {
                                                pattern_matching_result_3 = 1;
                                            }
                                        }
                                        else {
                                            pattern_matching_result_3 = 1;
                                        }
                                        switch (pattern_matching_result_3) {
                                            case 0: {
                                                const matchValue_1 = time.split(":");
                                                let pattern_matching_result_4, hour, minute;
                                                if ((!equalsWith((x_4, y_4) => comparePrimitives(x_4, y_4), matchValue_1, null)) ? (matchValue_1.length === 2) : false) {
                                                    const activePatternResult3233 = DateParsing_$007CInt$007C_$007C(matchValue_1[0]);
                                                    if (activePatternResult3233 != null) {
                                                        const activePatternResult3234 = DateParsing_$007CInt$007C_$007C(matchValue_1[1]);
                                                        if (activePatternResult3234 != null) {
                                                            pattern_matching_result_4 = 0;
                                                            hour = activePatternResult3233;
                                                            minute = activePatternResult3234;
                                                        }
                                                        else {
                                                            pattern_matching_result_4 = 1;
                                                        }
                                                    }
                                                    else {
                                                        pattern_matching_result_4 = 1;
                                                    }
                                                }
                                                else {
                                                    pattern_matching_result_4 = 1;
                                                }
                                                switch (pattern_matching_result_4) {
                                                    case 0: {
                                                        const matchValue_2 = [hour, minute];
                                                        let pattern_matching_result_5;
                                                        if (DateParsing_$007CBetween$007C_$007C(0, 59, matchValue_2[0]) != null) {
                                                            if (DateParsing_$007CBetween$007C_$007C(0, 59, matchValue_2[1]) != null) {
                                                                pattern_matching_result_5 = 0;
                                                            }
                                                            else {
                                                                pattern_matching_result_5 = 1;
                                                            }
                                                        }
                                                        else {
                                                            pattern_matching_result_5 = 1;
                                                        }
                                                        switch (pattern_matching_result_5) {
                                                            case 0: {
                                                                patternInput = [year_2, month_2, parsedDay, hour, minute];
                                                                break;
                                                            }
                                                            case 1: {
                                                                patternInput = [-1, 1, 1, 0, 0];
                                                                break;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                    case 1: {
                                                        patternInput = [-1, 1, 1, 0, 0];
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                            case 1: {
                                                patternInput = [-1, 1, 1, 0, 0];
                                                break;
                                            }
                                        }
                                    }
                                    else {
                                        patternInput = [-1, 1, 1, 0, 0];
                                    }
                                    break;
                                }
                                case 1: {
                                    patternInput = [-1, 1, 1, 0, 0];
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            const year_3 = patternInput[0] | 0;
            const month_3 = patternInput[1] | 0;
            const minute_1 = patternInput[4] | 0;
            const hour_1 = patternInput[3] | 0;
            const day_2 = patternInput[2] | 0;
            if (year_3 <= 0) {
                return void 0;
            }
            else {
                const matchValue_3 = [month_3, day_2];
                let pattern_matching_result_6;
                if (matchValue_3[0] === 2) {
                    if (DateParsing_$007CBetween$007C_$007C(1, 29, matchValue_3[1]) != null) {
                        if (DateParsing_isLeapYear(year_3)) {
                            pattern_matching_result_6 = 0;
                        }
                        else {
                            pattern_matching_result_6 = 1;
                        }
                    }
                    else {
                        pattern_matching_result_6 = 1;
                    }
                }
                else {
                    pattern_matching_result_6 = 1;
                }
                switch (pattern_matching_result_6) {
                    case 0: {
                        return create(year_3, month_3, day_2, hour_1, minute_1, 0);
                    }
                    case 1: {
                        let pattern_matching_result_7;
                        if (matchValue_3[0] === 2) {
                            if (DateParsing_$007CBetween$007C_$007C(1, 28, matchValue_3[1]) != null) {
                                if (!DateParsing_isLeapYear(year_3)) {
                                    pattern_matching_result_7 = 0;
                                }
                                else {
                                    pattern_matching_result_7 = 1;
                                }
                            }
                            else {
                                pattern_matching_result_7 = 1;
                            }
                        }
                        else {
                            pattern_matching_result_7 = 1;
                        }
                        switch (pattern_matching_result_7) {
                            case 0: {
                                return create(year_3, month_3, day_2, hour_1, minute_1, 0);
                            }
                            case 1: {
                                let pattern_matching_result_8;
                                if (matchValue_3[0] === 1) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 3) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 5) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 7) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 8) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 10) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else if (matchValue_3[0] === 12) {
                                    if (DateParsing_$007CBetween$007C_$007C(1, 31, matchValue_3[1]) != null) {
                                        pattern_matching_result_8 = 0;
                                    }
                                    else {
                                        pattern_matching_result_8 = 1;
                                    }
                                }
                                else {
                                    pattern_matching_result_8 = 1;
                                }
                                switch (pattern_matching_result_8) {
                                    case 0: {
                                        return create(year_3, month_3, day_2, hour_1, minute_1, 0);
                                    }
                                    case 1: {
                                        let pattern_matching_result_9;
                                        if (matchValue_3[0] === 4) {
                                            if (DateParsing_$007CBetween$007C_$007C(1, 30, matchValue_3[1]) != null) {
                                                pattern_matching_result_9 = 0;
                                            }
                                            else {
                                                pattern_matching_result_9 = 1;
                                            }
                                        }
                                        else if (matchValue_3[0] === 6) {
                                            if (DateParsing_$007CBetween$007C_$007C(1, 30, matchValue_3[1]) != null) {
                                                pattern_matching_result_9 = 0;
                                            }
                                            else {
                                                pattern_matching_result_9 = 1;
                                            }
                                        }
                                        else if (matchValue_3[0] === 9) {
                                            if (DateParsing_$007CBetween$007C_$007C(1, 30, matchValue_3[1]) != null) {
                                                pattern_matching_result_9 = 0;
                                            }
                                            else {
                                                pattern_matching_result_9 = 1;
                                            }
                                        }
                                        else if (matchValue_3[0] === 11) {
                                            if (DateParsing_$007CBetween$007C_$007C(1, 30, matchValue_3[1]) != null) {
                                                pattern_matching_result_9 = 0;
                                            }
                                            else {
                                                pattern_matching_result_9 = 1;
                                            }
                                        }
                                        else {
                                            pattern_matching_result_9 = 1;
                                        }
                                        switch (pattern_matching_result_9) {
                                            case 0: {
                                                return create(year_3, month_3, day_2, hour_1, minute_1, 0);
                                            }
                                            case 1: {
                                                return void 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    catch (matchValue_4) {
        return void 0;
    }
}

export const Interop_reactApi = react;

