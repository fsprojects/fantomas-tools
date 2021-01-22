import { updateUrlBy, restoreModelFromUrl } from "./UrlTools.js";
import { string, object } from "./.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { cmdForCurrentTab, parseUrl } from "./Navigation.js";
import { RouterModule_urlSegments } from "./.fable/Feliz.Router.3.2.0/Router.fs.js";
import { update as update_1, init as init_1 } from "./Trivia/State.js";
import { equals } from "./.fable/fable-library.3.1.1/Util.js";
import { Msg, Model, ActiveTab } from "./Model.js";
import { update as update_2, init as init_2 } from "./FSharpTokens/State.js";
import { update as update_3, init as init_3 } from "./ASTViewer/State.js";
import { update as update_4, init as init_4 } from "./FantomasOnline/State.js";
import { Model as Model_1, Msg as Msg_4, FantomasMode } from "./FantomasOnline/Model.js";
import { Cmd_ofSub, Cmd_none, Cmd_OfFunc_result, Cmd_map, Cmd_batch } from "./.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { ofArray } from "./.fable/fable-library.3.1.1/List.js";
import { Msg as Msg_1 } from "./FSharpTokens/Model.js";
import { Msg as Msg_2 } from "./ASTViewer/Model.js";
import { Msg as Msg_3 } from "./Trivia/Model.js";
import { replace } from "./.fable/fable-library.3.1.1/String.js";

function getCodeFromUrl() {
    return restoreModelFromUrl((path_1, v) => object((get$) => get$.Required.Field("code", string), path_1, v), "");
}

export function init(_arg1) {
    const sourceCode = getCodeFromUrl();
    const currentTab = parseUrl(RouterModule_urlSegments(window.location.hash, 1));
    const patternInput = init_1(equals(currentTab, new ActiveTab(3)));
    const patternInput_1 = init_2(equals(currentTab, new ActiveTab(1)));
    const patternInput_2 = init_3(equals(currentTab, new ActiveTab(2)));
    const patternInput_3 = init_4((currentTab.tag === 4) ? currentTab.fields[0] : (new FantomasMode(3)));
    const model = new Model(currentTab, sourceCode, false, patternInput[0], patternInput_1[0], patternInput_2[0], patternInput_3[0]);
    const initialCmd = cmdForCurrentTab(currentTab, model);
    return [model, Cmd_batch(ofArray([Cmd_map((arg0) => (new Msg(2, arg0)), patternInput[1]), Cmd_map((arg0_1) => (new Msg(3, arg0_1)), patternInput_1[1]), Cmd_map((arg0_2) => (new Msg(4, arg0_2)), patternInput_2[1]), Cmd_map((arg0_3) => (new Msg(5, arg0_3)), patternInput_3[1]), initialCmd]))];
}

function reload(model) {
    if (!model.SettingsOpen) {
        const matchValue = model.ActiveTab;
        switch (matchValue.tag) {
            case 1: {
                return Cmd_map((arg0) => (new Msg(3, arg0)), Cmd_OfFunc_result(new Msg_1(0)));
            }
            case 2: {
                return Cmd_map((arg0_1) => (new Msg(4, arg0_1)), Cmd_OfFunc_result(new Msg_2(2)));
            }
            case 3: {
                return Cmd_map((arg0_2) => (new Msg(2, arg0_2)), Cmd_OfFunc_result(new Msg_3(1)));
            }
            case 4: {
                return Cmd_map((arg0_3) => (new Msg(5, arg0_3)), Cmd_OfFunc_result(new Msg_4(3)));
            }
            default: {
                return Cmd_none();
            }
        }
    }
    else {
        return Cmd_none();
    }
}

export function update(msg, model) {
    let inputRecord;
    if (msg.tag === 1) {
        return [new Model(model.ActiveTab, msg.fields[0], model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel), Cmd_none()];
    }
    else if (msg.tag === 6) {
        const m = new Model(model.ActiveTab, model.SourceCode, !model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel);
        return [m, reload(m)];
    }
    else if (msg.tag === 2) {
        const patternInput = update_1(model.SourceCode, msg.fields[0], model.TriviaModel);
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, patternInput[0], model.FSharpTokensModel, model.ASTModel, model.FantomasModel), Cmd_map((arg0) => (new Msg(2, arg0)), patternInput[1])];
    }
    else if (msg.tag === 3) {
        const patternInput_1 = update_2(model.SourceCode, msg.fields[0], model.FSharpTokensModel);
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, patternInput_1[0], model.ASTModel, model.FantomasModel), Cmd_map((arg0_1) => (new Msg(3, arg0_1)), patternInput_1[1])];
    }
    else if (msg.tag === 4) {
        const patternInput_2 = update_3(model.SourceCode, msg.fields[0], model.ASTModel);
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, patternInput_2[0], model.FantomasModel), Cmd_map((arg0_2) => (new Msg(4, arg0_2)), patternInput_2[1])];
    }
    else if (msg.tag === 5) {
        if (msg.fields[0].tag === 6) {
            return [model, Cmd_ofSub((dispatch) => {
                updateUrlBy((hashWithoutQuery) => {
                    const version = (m_1) => {
                        switch (m_1.tag) {
                            case 1: {
                                return "v3";
                            }
                            case 2: {
                                return "v4";
                            }
                            case 3: {
                                return "preview";
                            }
                            default: {
                                return "v2";
                            }
                        }
                    };
                    return replace(hashWithoutQuery, version(model.FantomasModel.Mode), version(msg.fields[0].fields[0]));
                });
                dispatch(new Msg(0, new ActiveTab(4, msg.fields[0].fields[0])));
            })];
        }
        else {
            const patternInput_3 = update_4((model.ActiveTab.tag === 4) ? true : false, model.SourceCode, msg.fields[0], model.FantomasModel);
            return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, patternInput_3[0]), Cmd_map((arg0_3) => (new Msg(5, arg0_3)), patternInput_3[1])];
        }
    }
    else {
        return [(msg.fields[0].tag === 4) ? ((!equals(msg.fields[0].fields[0], model.FantomasModel.Mode)) ? (new Model(msg.fields[0], model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, (inputRecord = model.FantomasModel, new Model_1(inputRecord.IsFsi, inputRecord.Version, inputRecord.DefaultOptions, inputRecord.UserOptions, msg.fields[0].fields[0], inputRecord.State)))) : (new Model(msg.fields[0], model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel))) : (new Model(msg.fields[0], model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel)), cmdForCurrentTab(msg.fields[0], model)];
    }
}

