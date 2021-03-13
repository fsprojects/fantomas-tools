import { printf, toText } from "../.fable/fable-library.3.1.7/String.js";
import * as react from "../../../_snowpack/pkg/react.js";
import { numberHash, int32ToString } from "../.fable/fable-library.3.1.7/Util.js";
import { Msg } from "./Model.js";
import { map, mapIndexed } from "../.fable/fable-library.3.1.7/Array.js";
import { Array_groupBy } from "../.fable/fable-library.3.1.7/Seq2.js";
import { map as map_1 } from "../.fable/fable-library.3.1.7/Option.js";
import { loader } from "../Loader.js";
import { ButtonProps, button } from "../.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { DOMAttr } from "../.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { singleton } from "../.fable/fable-library.3.1.7/List.js";
import { versionBar } from "../VersionBar.js";
import { input } from "../SettingControls.js";

function tokenNameClass(token) {
    const arg10 = token.TokenInfo.TokenName.toLocaleLowerCase();
    return toText(printf("is-%s"))(arg10);
}

function lineToken(dispatch, index, token) {
    let copyOfStruct, arg10;
    return react.createElement("div", {
        className: "token",
        key: (copyOfStruct = (index | 0), int32ToString(copyOfStruct)),
        onClick: (_arg1) => {
            dispatch(new Msg(3, index));
        },
    }, react.createElement("span", {
        className: (arg10 = tokenNameClass(token), toText(printf("tag %s"))(arg10)),
    }, token.TokenInfo.TokenName));
}

function line(dispatch, activeLine, lineNumber, tokens_1) {
    let al, al_1;
    const tokens_2 = mapIndexed((index, token) => lineToken(dispatch, index, token), tokens_1);
    const className = (activeLine != null) ? ((al = (activeLine | 0), al === lineNumber) ? (al_1 = (activeLine | 0), "line active") : "line") : "line";
    return react.createElement("div", {
        className: className,
        key: toText(printf("line-%d"))(lineNumber),
        onClick: (_arg1) => {
            dispatch(new Msg(2, lineNumber));
        },
    }, react.createElement("div", {
        className: "line-number",
    }, lineNumber), react.createElement("div", {
        className: "tokens",
    }, tokens_2));
}

function tokens(model, dispatch) {
    const lines = map((tupledArg) => line(dispatch, model.ActiveLine, tupledArg[0], tupledArg[1]), Array_groupBy((t) => t.LineNumber, model.Tokens, {
        Equals: (x, y) => (x === y),
        GetHashCode: (x) => numberHash(x),
    }));
    return react.createElement("div", {
        id: "tokens",
    }, react.createElement("div", {
        className: "lines",
    }, lines));
}

function tokenDetailRow(label, content) {
    return react.createElement("tr", {}, react.createElement("td", {}, react.createElement("strong", {}, label)), react.createElement("td", {}, content));
}

function tokenDetail(dispatch, index, token) {
    let copyOfStruct;
    let className;
    const arg10 = tokenNameClass(token);
    className = toText(printf("tag is-large %s"))(arg10);
    const patternInput = token.TokenInfo;
    const tokenName = patternInput.TokenName;
    const tag = patternInput.Tag | 0;
    const rightColumn = patternInput.RightColumn | 0;
    const leftColumn = patternInput.LeftColumn | 0;
    const fullMatchedLength = patternInput.FullMatchedLength | 0;
    const colorClass = patternInput.ColorClass;
    const charClass = patternInput.CharClass;
    return react.createElement("div", {
        className: "detail",
        key: (copyOfStruct = (index | 0), int32ToString(copyOfStruct)),
    }, react.createElement("h3", {
        className: className,
        onClick: (_arg1) => {
            dispatch(new Msg(3, index));
        },
    }, token.TokenInfo.TokenName, react.createElement("small", {}, toText(printf("(%d)"))(index))), react.createElement("table", {
        className: "table table-striped table-hover mb-0",
    }, react.createElement("tbody", {}, tokenDetailRow("TokenName", tokenName), tokenDetailRow("LeftColumn", leftColumn), tokenDetailRow("RightColumn", rightColumn), tokenDetailRow("Content", react.createElement("pre", {}, react.createElement("code", {}, token.Content))), tokenDetailRow("ColorClass", colorClass), tokenDetailRow("CharClass", charClass), tokenDetailRow("Tag", tag), tokenDetailRow("FullMatchedLength", react.createElement("span", {
        className: "has-text-weight-semibold",
    }, fullMatchedLength)))));
}

function details(model, dispatch) {
    const o_1 = map_1((activeLine) => {
        const details_1 = mapIndexed((index, token) => tokenDetail(dispatch, index, token), model.Tokens.filter((t) => (t.LineNumber === activeLine)));
        return react.createElement("div", {
            id: "details",
        }, react.createElement("h4", {
            className: "p-2",
        }, "Details of line ", react.createElement("span", {
            className: "has-text-grey",
        }, activeLine)), react.createElement("div", {
            className: "detail-container",
        }, details_1));
    }, model.ActiveLine);
    if (o_1 == null) {
        return null;
    }
    else {
        const o_2 = o_1;
        return o_2;
    }
}

export function view(model, dispatch) {
    if (model.IsLoading) {
        return loader;
    }
    else {
        return react.createElement("div", {
            className: "tab-result",
        }, tokens(model, dispatch), details(model, dispatch));
    }
}

export function commands(dispatch) {
    return button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(0));
    })))], ["Get tokens"]);
}

export function settings(model, dispatch) {
    return react.createElement(react.Fragment, {}, versionBar(toText(printf("FSC - %s"))(model.Version)), input("token-defines", (arg) => {
        dispatch(new Msg(5, arg));
    }, "Defines", "Enter your defines separated with a space", model.Defines));
}

