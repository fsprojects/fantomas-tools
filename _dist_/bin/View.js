import { printf, toText } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/String.js";
import { NavbarProps, navbar } from "./bin/.fable/Fable.Reactstrap.0.5.1/Navbar.fs.js";
import { Prop, DOMAttr, HTMLAttr } from "./bin/.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { ofArray, singleton } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/List.js";
import { NavbarBrandProps, navbarBrand } from "./bin/.fable/Fable.Reactstrap.0.5.1/NavbarBrand.fs.js";
import { ButtonProps, button } from "./bin/.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import * as react from "../../web_modules/react.js";
import { ActiveTab, Msg } from "./Model.js";
import { mkCol, ColProps, col } from "./bin/.fable/Fable.Reactstrap.0.5.1/Col.fs.js";
import Editor from "../js/Editor.js";
import { jumbotron } from "./bin/.fable/Fable.Reactstrap.0.5.1/Jumbotron.fs.js";
import { commands as commands_1, settings as settings_1, view } from "./Trivia/View.js";
import { commands as commands_2, settings as settings_2, view as view_1 } from "./FSharpTokens/View.js";
import { commands as commands_3, settings as settings_3, view as view_2 } from "./ASTViewer/View.js";
import { commands as commands_4, settings as settings_4, view as view_3 } from "./FantomasOnline/View.js";
import { toHash } from "./Navigation.js";
import { NavItemProps, navItem as navItem_1 } from "./bin/.fable/Fable.Reactstrap.0.5.1/NavItem.fs.js";
import { NavLinkProps, navLink } from "./bin/.fable/Fable.Reactstrap.0.5.1/NavLink.fs.js";
import { equalsSafe } from "./.fable/fable-library.3.0.0-nagareyama-rc-008/Util.js";
import { FantomasMode } from "./FantomasOnline/Model.js";
import { NavProps, nav } from "./bin/.fable/Fable.Reactstrap.0.5.1/Nav.fs.js";

export function navigation(dispatch) {
    let children_4;
    const title = toText(printf("Fantomas tools"));
    return navbar([new NavbarProps(1, true), new NavbarProps(7, singleton(new HTMLAttr(64, "bg-light")))], [navbarBrand([new NavbarBrandProps(2, singleton(new HTMLAttr(64, "py-0")))], [title]), (children_4 = [button([new ButtonProps(9, ofArray([new HTMLAttr(94, "https://github.com/fsprojects/fantomas-tools"), new HTMLAttr(157, "_blank"), new HTMLAttr(64, "text-white")])), new ButtonProps(1, "dark")], [react.createElement("i", {
        className: "fab fa-github mr-1 mt-1",
    }), "GitHub"]), button([new ButtonProps(9, ofArray([new HTMLAttr(64, "ml-2 pointer"), new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(6));
    })]))], [react.createElement("i", {
        className: "fas fa-sliders-h",
    })])], react.createElement("div", {
        className: "navbar-text py1",
    }, ...children_4))]);
}

export function editor(model, dispatch) {
    let children_1, props_1;
    return col([new ColProps(1, mkCol(5)), new ColProps(7, singleton(new HTMLAttr(64, "border-right h-100 d-flex flex-column")))], [(children_1 = [(props_1 = {
        onChange: (arg) => {
            dispatch((new Msg(1, arg)));
        },
        value: model.SourceCode,
    }, react.createElement(Editor, props_1))], react.createElement("div", {
        id: "source",
        className: "flex-grow-1",
    }, ...children_1))]);
}

const homeTab = jumbotron([], [react.createElement("h1", {
    className: "display-3",
}, "Fantomas tool"), react.createElement("p", {
    className: "lead",
}, "Welcome at the Fantomas Tools!"), react.createElement("p", {}, "if you plan on using these tools extensively, consider cloning the repository and run everything locally.")]);

function settings(model, dispatch, inner) {
    let children_4, children_2;
    let className;
    const arg10 = model.SettingsOpen ? "open" : "";
    const clo1 = toText(printf("settings %s"));
    className = clo1(arg10);
    const children_6 = [(children_4 = [(children_2 = [react.createElement("i", {
        className: "fas fa-times close",
        onClick: (_arg1) => {
            dispatch(new Msg(6));
        },
    }), "Settings"], react.createElement("h1", {
        className: "text-center",
    }, ...children_2)), inner], react.createElement("div", {
        className: "inner",
    }, ...children_4))];
    return react.createElement("div", {
        className: className,
        onClick: (ev) => {
            if (ev.target.className === "settings open") {
                dispatch(new Msg(6));
            }
        },
    }, ...children_6);
}

export function tabs(model, dispatch) {
    let children_2;
    let patternInput;
    const matchValue = model.ActiveTab;
    switch (matchValue.tag) {
        case 3: {
            const triviaDispatch = (tMsg) => {
                dispatch(new Msg(2, tMsg));
            };
            patternInput = [view(model.TriviaModel, triviaDispatch), settings_1(model.TriviaModel, triviaDispatch), commands_1(triviaDispatch)];
            break;
        }
        case 1: {
            const tokensDispatch = (tMsg_1) => {
                dispatch(new Msg(3, tMsg_1));
            };
            patternInput = [view_1(model.FSharpTokensModel, tokensDispatch), settings_2(model.FSharpTokensModel, tokensDispatch), commands_2(tokensDispatch)];
            break;
        }
        case 2: {
            const astDispatch = (aMsg) => {
                dispatch(new Msg(4, aMsg));
            };
            patternInput = [view_2(model.ASTModel, astDispatch), settings_3(model.ASTModel, astDispatch), commands_3(astDispatch)];
            break;
        }
        case 4: {
            const fantomasDispatch = (fMsg) => {
                dispatch(new Msg(5, fMsg));
            };
            patternInput = [view_3(model.FantomasModel), settings_4(model.FantomasModel, fantomasDispatch), commands_4(model.SourceCode, model.FantomasModel, fantomasDispatch)];
            break;
        }
        default: {
            patternInput = [homeTab, null, null];
        }
    }
    const navItem = (tab_1, label, isActive) => {
        let href;
        const page = toHash(tab_1);
        let query;
        const hash = window.location.hash;
        if (hash.indexOf("?") >= 0) {
            const arg10 = hash.split("?")[1];
            const clo1 = toText(printf("?%s"));
            query = clo1(arg10);
        }
        else {
            query = "";
        }
        const clo1_1 = toText(printf("%s%s"));
        const clo2 = clo1_1(page);
        href = clo2(query);
        return navItem_1([new NavItemProps(2, singleton(new Prop(0, label)))], [navLink([new NavLinkProps(3, singleton(new HTMLAttr(94, href))), new NavLinkProps(0, isActive)], [label])]);
    };
    const navItems = ofArray([navItem(new ActiveTab(0), "Home", equalsSafe(model.ActiveTab, new ActiveTab(0))), navItem(new ActiveTab(1), "FSharp Tokens", equalsSafe(model.ActiveTab, new ActiveTab(1))), navItem(new ActiveTab(2), "AST", equalsSafe(model.ActiveTab, new ActiveTab(2))), navItem(new ActiveTab(3), "Trivia", equalsSafe(model.ActiveTab, new ActiveTab(3))), navItem(new ActiveTab(4, new FantomasMode(3)), "Fantomas", ((model.ActiveTab.tag === 4) ? true : false))]);
    const children_4 = [settings(model, dispatch, patternInput[1]), nav([new NavProps(0, true), new NavProps(9, singleton(new HTMLAttr(64, "")))], [Array.from(navItems)]), (children_2 = [patternInput[0], react.createElement("div", {
        id: "commands",
    }, patternInput[2])], react.createElement("div", {
        id: "tab-content",
    }, ...children_2))];
    return react.createElement("div", {
        className: "col-7 h-100",
    }, ...children_4);
}

