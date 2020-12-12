import { isNullOrWhiteSpace } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { Cmd_none, Cmd_map, Cmd_OfFunc_result } from "./bin/.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { Msg } from "./FSharpTokens/Model.js";
import { ActiveTab, Msg as Msg_1 } from "./Model.js";
import { Msg as Msg_2 } from "./ASTViewer/Model.js";
import { Msg as Msg_3 } from "./Trivia/Model.js";
import { equalsSafe } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { getOptionsCmd } from "./FantomasOnline/State.js";
import { FantomasMode } from "./FantomasOnline/Model.js";
import { Route_$007CQuery$007C_$007C } from "./bin/.fable/Feliz.Router.3.2.0/Router.fs.js";

export function cmdForCurrentTab(tab, model) {
    if (!isNullOrWhiteSpace(model.SourceCode)) {
        if (tab.tag === 1) {
            const cmd = Cmd_OfFunc_result(new Msg(0));
            return Cmd_map((arg0) => (new Msg_1(3, arg0)), cmd);
        }
        else if (tab.tag === 2) {
            const cmd_1 = Cmd_OfFunc_result(new Msg_2(2));
            return Cmd_map((arg0_1) => (new Msg_1(4, arg0_1)), cmd_1);
        }
        else if (tab.tag === 3) {
            const cmd_2 = Cmd_OfFunc_result(new Msg_3(1));
            return Cmd_map((arg0_2) => (new Msg_1(2, arg0_2)), cmd_2);
        }
        else if (tab.tag === 4) {
            if (!equalsSafe(tab.fields[0], model.FantomasModel.Mode)) {
                return Cmd_map((arg0_3) => (new Msg_1(5, arg0_3)), getOptionsCmd(tab.fields[0]));
            }
            else if (tab.tag === 4) {
                return Cmd_none();
            }
            else {
                throw (new Error("The match cases were incomplete"));
            }
        }
        else {
            return Cmd_none();
        }
    }
    else {
        return Cmd_none();
    }
}

export function toHash(_arg1) {
    if (_arg1.tag === 3) {
        return "#/trivia";
    }
    else if (_arg1.tag === 1) {
        return "#/tokens";
    }
    else if (_arg1.tag === 2) {
        return "#/ast";
    }
    else if (_arg1.tag === 4) {
        if (_arg1.fields[0].tag === 1) {
            return "#/fantomas/v3";
        }
        else if (_arg1.fields[0].tag === 2) {
            return "#/fantomas/v4";
        }
        else if (_arg1.fields[0].tag === 3) {
            return "#/fantomas/preview";
        }
        else {
            return "#/fantomas/v2";
        }
    }
    else {
        return "#/";
    }
}

export function parseUrl(segments) {
    let pattern_matching_result;
    if (segments.tail != null) {
        if (segments.head === "tokens") {
            if (segments.tail.tail != null) {
                const activePatternResult12201 = Route_$007CQuery$007C_$007C(segments.tail.head);
                if (activePatternResult12201 != null) {
                    if (activePatternResult12201.tail != null) {
                        if (activePatternResult12201.head[0] === "data") {
                            if (activePatternResult12201.tail.tail == null) {
                                if (segments.tail.tail.tail == null) {
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
                pattern_matching_result = 0;
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
            return new ActiveTab(1);
        }
        case 1: {
            let pattern_matching_result_1;
            if (segments.tail != null) {
                if (segments.head === "ast") {
                    if (segments.tail.tail != null) {
                        const activePatternResult12200 = Route_$007CQuery$007C_$007C(segments.tail.head);
                        if (activePatternResult12200 != null) {
                            if (activePatternResult12200.tail != null) {
                                if (activePatternResult12200.head[0] === "data") {
                                    if (activePatternResult12200.tail.tail == null) {
                                        if (segments.tail.tail.tail == null) {
                                            pattern_matching_result_1 = 0;
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
                        }
                        else {
                            pattern_matching_result_1 = 1;
                        }
                    }
                    else {
                        pattern_matching_result_1 = 0;
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
                    return new ActiveTab(2);
                }
                case 1: {
                    let pattern_matching_result_2;
                    if (segments.tail != null) {
                        if (segments.head === "trivia") {
                            if (segments.tail.tail != null) {
                                const activePatternResult12199 = Route_$007CQuery$007C_$007C(segments.tail.head);
                                if (activePatternResult12199 != null) {
                                    if (activePatternResult12199.tail != null) {
                                        if (activePatternResult12199.head[0] === "data") {
                                            if (activePatternResult12199.tail.tail == null) {
                                                if (segments.tail.tail.tail == null) {
                                                    pattern_matching_result_2 = 0;
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
                                pattern_matching_result_2 = 0;
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
                            return new ActiveTab(3);
                        }
                        case 1: {
                            let pattern_matching_result_3;
                            if (segments.tail != null) {
                                if (segments.head === "fantomas") {
                                    if (segments.tail.tail != null) {
                                        if (segments.tail.head === "v2") {
                                            if (segments.tail.tail.tail != null) {
                                                const activePatternResult12198 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                if (activePatternResult12198 != null) {
                                                    if (activePatternResult12198.tail != null) {
                                                        if (activePatternResult12198.head[0] === "data") {
                                                            if (activePatternResult12198.tail.tail == null) {
                                                                if (segments.tail.tail.tail.tail == null) {
                                                                    pattern_matching_result_3 = 0;
                                                                }
                                                                else {
                                                                    pattern_matching_result_3 = 1;
                                                                }
                                                            }
                                                            else {
                                                                pattern_matching_result_3 = 1;
                                                            }
                                                        }
                                                        else {
                                                            pattern_matching_result_3 = 1;
                                                        }
                                                    }
                                                    else {
                                                        pattern_matching_result_3 = 1;
                                                    }
                                                }
                                                else {
                                                    pattern_matching_result_3 = 1;
                                                }
                                            }
                                            else {
                                                pattern_matching_result_3 = 0;
                                            }
                                        }
                                        else {
                                            pattern_matching_result_3 = 1;
                                        }
                                    }
                                    else {
                                        pattern_matching_result_3 = 1;
                                    }
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
                                    return new ActiveTab(4, new FantomasMode(0));
                                }
                                case 1: {
                                    let pattern_matching_result_4;
                                    if (segments.tail != null) {
                                        if (segments.head === "fantomas") {
                                            if (segments.tail.tail != null) {
                                                if (segments.tail.head === "v3") {
                                                    if (segments.tail.tail.tail != null) {
                                                        const activePatternResult12197 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                        if (activePatternResult12197 != null) {
                                                            if (activePatternResult12197.tail != null) {
                                                                if (activePatternResult12197.head[0] === "data") {
                                                                    if (activePatternResult12197.tail.tail == null) {
                                                                        if (segments.tail.tail.tail.tail == null) {
                                                                            pattern_matching_result_4 = 0;
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
                                                        pattern_matching_result_4 = 0;
                                                    }
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
                                    }
                                    else {
                                        pattern_matching_result_4 = 1;
                                    }
                                    switch (pattern_matching_result_4) {
                                        case 0: {
                                            return new ActiveTab(4, new FantomasMode(1));
                                        }
                                        case 1: {
                                            let pattern_matching_result_5;
                                            if (segments.tail != null) {
                                                if (segments.head === "fantomas") {
                                                    if (segments.tail.tail != null) {
                                                        if (segments.tail.head === "v4") {
                                                            if (segments.tail.tail.tail != null) {
                                                                const activePatternResult12196 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                                if (activePatternResult12196 != null) {
                                                                    if (activePatternResult12196.tail != null) {
                                                                        if (activePatternResult12196.head[0] === "data") {
                                                                            if (activePatternResult12196.tail.tail == null) {
                                                                                if (segments.tail.tail.tail.tail == null) {
                                                                                    pattern_matching_result_5 = 0;
                                                                                }
                                                                                else {
                                                                                    pattern_matching_result_5 = 1;
                                                                                }
                                                                            }
                                                                            else {
                                                                                pattern_matching_result_5 = 1;
                                                                            }
                                                                        }
                                                                        else {
                                                                            pattern_matching_result_5 = 1;
                                                                        }
                                                                    }
                                                                    else {
                                                                        pattern_matching_result_5 = 1;
                                                                    }
                                                                }
                                                                else {
                                                                    pattern_matching_result_5 = 1;
                                                                }
                                                            }
                                                            else {
                                                                pattern_matching_result_5 = 0;
                                                            }
                                                        }
                                                        else {
                                                            pattern_matching_result_5 = 1;
                                                        }
                                                    }
                                                    else {
                                                        pattern_matching_result_5 = 1;
                                                    }
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
                                                    return new ActiveTab(4, new FantomasMode(2));
                                                }
                                                case 1: {
                                                    let pattern_matching_result_6;
                                                    if (segments.tail != null) {
                                                        if (segments.head === "fantomas") {
                                                            if (segments.tail.tail != null) {
                                                                if (segments.tail.head === "preview") {
                                                                    if (segments.tail.tail.tail != null) {
                                                                        const activePatternResult12195 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                                        if (activePatternResult12195 != null) {
                                                                            if (activePatternResult12195.tail != null) {
                                                                                if (activePatternResult12195.head[0] === "data") {
                                                                                    if (activePatternResult12195.tail.tail == null) {
                                                                                        if (segments.tail.tail.tail.tail == null) {
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
                                                                        pattern_matching_result_6 = 0;
                                                                    }
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
                                                    }
                                                    else {
                                                        pattern_matching_result_6 = 1;
                                                    }
                                                    switch (pattern_matching_result_6) {
                                                        case 0: {
                                                            return new ActiveTab(4, new FantomasMode(3));
                                                        }
                                                        case 1: {
                                                            return new ActiveTab(0);
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
            }
        }
    }
}

