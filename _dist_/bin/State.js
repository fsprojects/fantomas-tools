import { updateUrlBy, restoreModelFromUrl } from "./UrlTools.js";
import { string, object } from "./.fable/Thoth.Json.5.0.0/Decode.fs.js";
import { cmdForCurrentTab, parseUrl } from "./Navigation.js";
import { RouterModule_urlSegments } from "./.fable/Feliz.Router.3.2.0/Router.fs.js";
import { update as update_1, init as init_1 } from "./Trivia/State.js";
import { equals } from "./.fable/fable-library.3.1.7/Util.js";
import { Msg, Model, ActiveTab } from "./Model.js";
import { update as update_2, init as init_2 } from "./FSharpTokens/State.js";
import { update as update_3, init as init_3 } from "./ASTViewer/State.js";
import { Model as Model_1, Msg as Msg_4, FantomasMode } from "./FantomasOnline/Model.js";
import { update as update_4, init as init_4 } from "./FantomasOnline/State.js";
import { Cmd_ofSub, Cmd_none, Cmd_OfFunc_result, Cmd_map, Cmd_batch } from "./.fable/Fable.Elmish.3.1.0/cmd.fs.js";
import { ofArray } from "./.fable/fable-library.3.1.7/List.js";
import { Msg as Msg_1 } from "./FSharpTokens/Model.js";
import { Msg as Msg_2 } from "./ASTViewer/Model.js";
import { Msg as Msg_3 } from "./Trivia/Model.js";
import { replace } from "./.fable/fable-library.3.1.7/String.js";

function getCodeFromUrl() {
    return restoreModelFromUrl((path_1, v) => object((get$) => get$.Required.Field("code", (path, value) => string(path, value)), path_1, v), "");
}

export function init(_arg1) {
    const sourceCode = getCodeFromUrl();
    const currentTab = parseUrl(RouterModule_urlSegments(window.location.hash, 1));
    const patternInput = init_1(equals(currentTab, new ActiveTab(3)));
    const triviaModel = patternInput[0];
    const triviaCmd = patternInput[1];
    const patternInput_1 = init_2(equals(currentTab, new ActiveTab(1)));
    const fsharpTokensModel = patternInput_1[0];
    const fsharpTokensCmd = patternInput_1[1];
    const patternInput_2 = init_3(equals(currentTab, new ActiveTab(2)));
    const astModel = patternInput_2[0];
    const astCmd = patternInput_2[1];
    let patternInput_3;
    let tab;
    if (currentTab.tag === 4) {
        const ft = currentTab.fields[0];
        tab = ft;
    }
    else {
        tab = (new FantomasMode(3));
    }
    patternInput_3 = init_4(tab);
    const fantomasModel = patternInput_3[0];
    const fantomasCmd = patternInput_3[1];
    const model = new Model(currentTab, sourceCode, false, triviaModel, fsharpTokensModel, astModel, fantomasModel);
    const initialCmd = cmdForCurrentTab(currentTab, model);
    const cmd = Cmd_batch(ofArray([Cmd_map((arg0) => (new Msg(2, arg0)), triviaCmd), Cmd_map((arg0_1) => (new Msg(3, arg0_1)), fsharpTokensCmd), Cmd_map((arg0_2) => (new Msg(4, arg0_2)), astCmd), Cmd_map((arg0_3) => (new Msg(5, arg0_3)), fantomasCmd), initialCmd]));
    return [model, cmd];
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
    let ft, ft_1, inputRecord;
    if (msg.tag === 1) {
        const code = msg.fields[0];
        return [new Model(model.ActiveTab, code, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel), Cmd_none()];
    }
    else if (msg.tag === 6) {
        const m = new Model(model.ActiveTab, model.SourceCode, !model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel);
        return [m, reload(m)];
    }
    else if (msg.tag === 2) {
        const tMsg = msg.fields[0];
        const patternInput = update_1(model.SourceCode, tMsg, model.TriviaModel);
        const tModel = patternInput[0];
        const tCmd = patternInput[1];
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, tModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel), Cmd_map((arg0) => (new Msg(2, arg0)), tCmd)];
    }
    else if (msg.tag === 3) {
        const ftMsg = msg.fields[0];
        const patternInput_1 = update_2(model.SourceCode, ftMsg, model.FSharpTokensModel);
        const fModel = patternInput_1[0];
        const fCmd = patternInput_1[1];
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, fModel, model.ASTModel, model.FantomasModel), Cmd_map((arg0_1) => (new Msg(3, arg0_1)), fCmd)];
    }
    else if (msg.tag === 4) {
        const aMsg = msg.fields[0];
        const patternInput_2 = update_3(model.SourceCode, aMsg, model.ASTModel);
        const aModel = patternInput_2[0];
        const aCmd = patternInput_2[1];
        return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, aModel, model.FantomasModel), Cmd_map((arg0_2) => (new Msg(4, arg0_2)), aCmd)];
    }
    else if (msg.tag === 5) {
        if (msg.fields[0].tag === 6) {
            const mode = msg.fields[0].fields[0];
            let cmd_1;
            const changeVersion = (hashWithoutQuery) => {
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
                const oldVersion = version(model.FantomasModel.Mode);
                const newVersion = version(mode);
                return replace(hashWithoutQuery, oldVersion, newVersion);
            };
            cmd_1 = Cmd_ofSub((dispatch) => {
                updateUrlBy(changeVersion);
                dispatch(new Msg(0, new ActiveTab(4, mode)));
            });
            return [model, cmd_1];
        }
        else {
            const fMsg = msg.fields[0];
            const isActiveTab = (model.ActiveTab.tag === 4) ? true : false;
            const patternInput_3 = update_4(isActiveTab, model.SourceCode, fMsg, model.FantomasModel);
            const fModel_1 = patternInput_3[0];
            const fCmd_1 = patternInput_3[1];
            return [new Model(model.ActiveTab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, fModel_1), Cmd_map((arg0_3) => (new Msg(5, arg0_3)), fCmd_1)];
        }
    }
    else {
        const tab = msg.fields[0];
        const nextModel = (tab.tag === 4) ? ((ft = tab.fields[0], !equals(ft, model.FantomasModel.Mode)) ? (ft_1 = tab.fields[0], new Model(tab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, (inputRecord = model.FantomasModel, new Model_1(inputRecord.IsFsi, inputRecord.Version, inputRecord.DefaultOptions, inputRecord.UserOptions, ft_1, inputRecord.State)))) : (new Model(tab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel))) : (new Model(tab, model.SourceCode, model.SettingsOpen, model.TriviaModel, model.FSharpTokensModel, model.ASTModel, model.FantomasModel));
        const cmd = cmdForCurrentTab(tab, model);
        return [nextModel, cmd];
    }
}

