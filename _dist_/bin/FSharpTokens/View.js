import { printf, toText } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { DOMAttr, Prop, HTMLAttr } from "../bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { structuralHash, int32ToString } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { Msg } from "./Model.js";
import * as react from "../../../web_modules/react.js";
import { keyValueList } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/MapUtil.js";
import { map, groupBy, mapIndexed } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Array.js";
import { map as map_1 } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/Option.js";
import { loader } from "../Loader.js";
import { ButtonProps, button } from "../bin/.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { singleton } from "../.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { versionBar } from "../VersionBar.js";
import { input } from "../SettingControls.js";

function tokenNameClass(token) {
    const arg10 = token.TokenInfo.TokenName.toLocaleLowerCase();
    const clo1 = toText(printf("is-%s"));
    return clo1(arg10);
}

function lineToken(dispatch, index, token) {
    let copyOfStruct, props, arg10, clo1;
    const props_2 = [new HTMLAttr(64, "token"), new Prop(0, (copyOfStruct = (index | 0), int32ToString(copyOfStruct))), new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(3, index));
    })];
    const children_2 = [(props = [new HTMLAttr(64, (arg10 = tokenNameClass(token), (clo1 = toText(printf("tag %s")), clo1(arg10))))], react.createElement("span", keyValueList(props, 1), token.TokenInfo.TokenName))];
    return react.createElement("div", keyValueList(props_2, 1), ...children_2);
}

function line(dispatch, activeLine, lineNumber, tokens_1) {
    let al, al_1, clo1;
    let tokens_2;
    tokens_2 = mapIndexed((index, token) => lineToken(dispatch, index, token), tokens_1);
    const className = (activeLine != null) ? ((al = (activeLine | 0), al === lineNumber) ? (al_1 = (activeLine | 0), "line active") : "line") : "line";
    const props_4 = [new HTMLAttr(64, className), new Prop(0, (clo1 = toText(printf("line-%d")), clo1(lineNumber))), new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(2, lineNumber));
    })];
    const children_4 = [react.createElement("div", {
        className: "line-number",
    }, lineNumber), react.createElement("div", {
        className: "tokens",
    }, tokens_2)];
    return react.createElement("div", keyValueList(props_4, 1), ...children_4);
}

function tokens(model, dispatch) {
    let lines;
    let array_1;
    array_1 = groupBy((t) => t.LineNumber, model.Tokens, {
        Equals: (x, y) => (x === y),
        GetHashCode: structuralHash,
    });
    lines = map((tupledArg) => line(dispatch, model.ActiveLine, tupledArg[0], tupledArg[1]), array_1);
    const children_2 = [react.createElement("div", {
        className: "lines",
    }, lines)];
    return react.createElement("div", {
        id: "tokens",
    }, ...children_2);
}

function tokenDetailRow(label, content) {
    let children_2;
    const children_6 = [(children_2 = [react.createElement("strong", {}, label)], react.createElement("td", {}, ...children_2)), react.createElement("td", {}, content)];
    return react.createElement("tr", {}, ...children_6);
}

function tokenDetail(dispatch, index, token) {
    let copyOfStruct, children_2, children, s_1, clo1_1, children_12, children_10, children_6;
    let className;
    const arg10 = tokenNameClass(token);
    const clo1 = toText(printf("tag is-large %s"));
    className = clo1(arg10);
    const patternInput = token.TokenInfo;
    const props_14 = [new HTMLAttr(64, "detail"), new Prop(0, (copyOfStruct = (index | 0), int32ToString(copyOfStruct)))];
    const children_14 = [(children_2 = [token.TokenInfo.TokenName, (children = [(s_1 = (clo1_1 = toText(printf("(%d)")), clo1_1(index)), (s_1))], react.createElement("small", {}, ...children))], react.createElement("h3", {
        className: className,
        onClick: (_arg1) => {
            dispatch(new Msg(3, index));
        },
    }, ...children_2)), (children_12 = [(children_10 = [tokenDetailRow("TokenName", patternInput.TokenName), tokenDetailRow("LeftColumn", patternInput.LeftColumn), tokenDetailRow("RightColumn", patternInput.RightColumn), tokenDetailRow("Content", (children_6 = [react.createElement("code", {}, token.Content)], react.createElement("pre", {}, ...children_6))), tokenDetailRow("ColorClass", patternInput.ColorClass), tokenDetailRow("CharClass", patternInput.CharClass), tokenDetailRow("Tag", patternInput.Tag), tokenDetailRow("FullMatchedLength", react.createElement("span", {
        className: "has-text-weight-semibold",
    }, patternInput.FullMatchedLength))], react.createElement("tbody", {}, ...children_10))], react.createElement("table", {
        className: "table table-striped table-hover mb-0",
    }, ...children_12))];
    return react.createElement("div", keyValueList(props_14, 1), ...children_14);
}

function details(model, dispatch) {
    let o;
    o = map_1((activeLine) => {
        let children_2;
        let details_1;
        let array_1;
        array_1 = model.Tokens.filter((t) => (t.LineNumber === activeLine));
        details_1 = mapIndexed((index, token) => tokenDetail(dispatch, index, token), array_1);
        const children_6 = [(children_2 = ["Details of line ", react.createElement("span", {
            className: "has-text-grey",
        }, activeLine)], react.createElement("h4", {
            className: "p-2",
        }, ...children_2)), react.createElement("div", {
            className: "detail-container",
        }, details_1)];
        return react.createElement("div", {
            id: "details",
        }, ...children_6);
    }, model.ActiveLine);
    const o_1 = o;
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
        const children = [tokens(model, dispatch), details(model, dispatch)];
        return react.createElement("div", {
            className: "tab-result",
        }, ...children);
    }
}

export function commands(dispatch) {
    return button([new ButtonProps(1, "primary"), new ButtonProps(9, singleton(new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(0));
    })))], ["Get tokens"]);
}

export function settings(model, dispatch) {
    let clo1;
    const children = [versionBar((clo1 = toText(printf("FSC - %s")), clo1(model.Version))), input("token-defines", (arg) => {
        dispatch((new Msg(5, arg)));
    }, "Defines", "Enter your defines separated with a space", model.Defines)];
    return react.createElement(react.Fragment, {}, ...children);
}

