import { isNullOrWhiteSpace } from "./.fable/fable-library.3.1.1/String.js";
import { Cmd_none, Cmd_batch, Cmd_OfFunc_result, Cmd_map } from "./.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { ActiveTab, Msg } from "./Model.js";
import { Msg as Msg_1 } from "./FSharpTokens/Model.js";
import { Msg as Msg_2 } from "./ASTViewer/Model.js";
import { Msg as Msg_3 } from "./Trivia/Model.js";
import { equals } from "./.fable/fable-library.3.1.1/Util.js";
import { getVersionCmd, getOptionsCmd } from "./FantomasOnline/State.js";
import { ofArray } from "./.fable/fable-library.3.1.1/List.js";
import { FantomasMode, Msg as Msg_4 } from "./FantomasOnline/Model.js";
import { Route_$007CQuery$007C_$007C } from "./.fable/Feliz.Router.3.2.0/Router.fs.js";

export function cmdForCurrentTab(tab, model) {
    if (!isNullOrWhiteSpace(model.SourceCode)) {
        if (tab.tag === 1) {
            return Cmd_map((arg0) => (new Msg(3, arg0)), Cmd_OfFunc_result(new Msg_1(0)));
        }
        else if (tab.tag === 2) {
            return Cmd_map((arg0_1) => (new Msg(4, arg0_1)), Cmd_OfFunc_result(new Msg_2(2)));
        }
        else if (tab.tag === 3) {
            return Cmd_map((arg0_2) => (new Msg(2, arg0_2)), Cmd_OfFunc_result(new Msg_3(1)));
        }
        else if (tab.tag === 4) {
            if (!equals(tab.fields[0], model.FantomasModel.Mode)) {
                return Cmd_batch(ofArray([Cmd_map((arg0_3) => (new Msg(5, arg0_3)), getOptionsCmd(tab.fields[0])), Cmd_map((arg0_4) => (new Msg(5, arg0_4)), getVersionCmd(tab.fields[0]))]));
            }
            else {
                let pattern_matching_result;
                if (tab.tag === 4) {
                    if (!(model.FantomasModel.DefaultOptions.tail == null)) {
                        pattern_matching_result = 0;
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
                        return Cmd_map((arg0_5) => (new Msg(5, arg0_5)), Cmd_OfFunc_result(new Msg_4(3)));
                    }
                    case 1: {
                        if (tab.tag === 4) {
                            return Cmd_none();
                        }
                        else {
                            throw (new Error("The match cases were incomplete"));
                        }
                    }
                }
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
                const activePatternResult13004 = Route_$007CQuery$007C_$007C(segments.tail.head);
                if (activePatternResult13004 != null) {
                    if (activePatternResult13004.tail != null) {
                        if (activePatternResult13004.head[0] === "data") {
                            if (activePatternResult13004.tail.tail == null) {
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
                        const activePatternResult13003 = Route_$007CQuery$007C_$007C(segments.tail.head);
                        if (activePatternResult13003 != null) {
                            if (activePatternResult13003.tail != null) {
                                if (activePatternResult13003.head[0] === "data") {
                                    if (activePatternResult13003.tail.tail == null) {
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
                                const activePatternResult13002 = Route_$007CQuery$007C_$007C(segments.tail.head);
                                if (activePatternResult13002 != null) {
                                    if (activePatternResult13002.tail != null) {
                                        if (activePatternResult13002.head[0] === "data") {
                                            if (activePatternResult13002.tail.tail == null) {
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
                                                const activePatternResult13001 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                if (activePatternResult13001 != null) {
                                                    if (activePatternResult13001.tail != null) {
                                                        if (activePatternResult13001.head[0] === "data") {
                                                            if (activePatternResult13001.tail.tail == null) {
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
                                                        const activePatternResult13000 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                        if (activePatternResult13000 != null) {
                                                            if (activePatternResult13000.tail != null) {
                                                                if (activePatternResult13000.head[0] === "data") {
                                                                    if (activePatternResult13000.tail.tail == null) {
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
                                                                const activePatternResult12999 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                                if (activePatternResult12999 != null) {
                                                                    if (activePatternResult12999.tail != null) {
                                                                        if (activePatternResult12999.head[0] === "data") {
                                                                            if (activePatternResult12999.tail.tail == null) {
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
                                                                        const activePatternResult12998 = Route_$007CQuery$007C_$007C(segments.tail.tail.head);
                                                                        if (activePatternResult12998 != null) {
                                                                            if (activePatternResult12998.tail != null) {
                                                                                if (activePatternResult12998.head[0] === "data") {
                                                                                    if (activePatternResult12998.tail.tail == null) {
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

