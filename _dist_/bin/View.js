import { NavbarProps, navbar } from "./.fable/Fable.Reactstrap.0.5.1/Navbar.fs.js";
import { Prop, DOMAttr, HTMLAttr } from "./.fable/Fable.React.7.0.1/Fable.React.Props.fs.js";
import { toArray, ofArray, singleton } from "./.fable/fable-library.3.1.15/List.js";
import { NavbarBrandProps, navbarBrand } from "./.fable/Fable.Reactstrap.0.5.1/NavbarBrand.fs.js";
import { printf, toText } from "./.fable/fable-library.3.1.15/String.js";
import { createElement } from "../../_snowpack/pkg/react.js";
import * as react from "../../_snowpack/pkg/react.js";
import { ButtonProps, button } from "./.fable/Fable.Reactstrap.0.5.1/Button.fs.js";
import { ActiveTab, Msg } from "./Model.js";
import { ColProps, mkCol, col } from "./.fable/Fable.Reactstrap.0.5.1/Col.fs.js";
import { MonacoEditorProp, Editor } from "./Editor.js";
import { jumbotron } from "./.fable/Fable.Reactstrap.0.5.1/Jumbotron.fs.js";
import { commands as commands_1, settings as settings_1, view } from "./Trivia/View.js";
import { commands as commands_2, settings as settings_2, view as view_1 } from "./FSharpTokens/View.js";
import { commands as commands_3, settings as settings_3, view as view_2 } from "./ASTViewer/View.js";
import { commands as commands_4, settings as settings_4, view as view_3 } from "./FantomasOnline/View.js";
import { NavItemProps, navItem as navItem_1 } from "./.fable/Fable.Reactstrap.0.5.1/NavItem.fs.js";
import { NavLinkProps, navLink } from "./.fable/Fable.Reactstrap.0.5.1/NavLink.fs.js";
import { toHash } from "./Navigation.js";
import { equals } from "./.fable/fable-library.3.1.15/Util.js";
import { FantomasMode } from "./FantomasOnline/Model.js";
import { NavProps, nav } from "./.fable/Fable.Reactstrap.0.5.1/Nav.fs.js";

export function navigation(dispatch) {
    return navbar([new NavbarProps(1, true), new NavbarProps(7, singleton(new HTMLAttr(64, "bg-light")))], [navbarBrand([new NavbarBrandProps(2, singleton(new HTMLAttr(64, "py-0")))], [toText(printf("Fantomas tools"))]), react.createElement("div", {
        className: "navbar-text py1",
    }, button([new ButtonProps(9, ofArray([new HTMLAttr(94, "https://github.com/sponsors/nojaf"), new HTMLAttr(157, "_blank"), new HTMLAttr(99, "sponsor-button")])), new ButtonProps(1, "success"), new ButtonProps(2, true)], [react.createElement("i", {
        className: "far fa-heart mr-1 mt-1 text-danger",
    }), "Sponsor"]), button([new ButtonProps(9, ofArray([new HTMLAttr(94, "https://github.com/fsprojects/fantomas-tools"), new HTMLAttr(157, "_blank"), new HTMLAttr(64, "text-white ml-2")])), new ButtonProps(1, "dark")], [react.createElement("i", {
        className: "fab fa-github mr-1 mt-1",
    }), "GitHub"]), button([new ButtonProps(9, ofArray([new HTMLAttr(64, "ml-2 pointer"), new DOMAttr(40, (_arg1) => {
        dispatch(new Msg(6));
    })]))], [react.createElement("i", {
        className: "fas fa-sliders-h",
    })]))]);
}

export function editor(model, dispatch) {
    return col([new ColProps(1, mkCol(5)), new ColProps(7, singleton(new HTMLAttr(64, "border-right h-100 d-flex flex-column")))], [react.createElement("div", {
        id: "source",
        className: "flex-grow-1",
    }, createElement(Editor, {
        isReadOnly: false,
        props: ofArray([new MonacoEditorProp(3, (arg) => {
            dispatch(new Msg(1, arg));
        }), new MonacoEditorProp(2, model.SourceCode)]),
    }))]);
}

const homeTab = jumbotron([], [react.createElement("h1", {
    className: "display-3",
}, "Fantomas tool"), react.createElement("p", {
    className: "lead",
}, "Welcome at the Fantomas Tools!"), react.createElement("p", {}, "if you plan on using these tools extensively, consider cloning the repository and run everything locally.")]);

function settings(model, dispatch, inner) {
    let className;
    const arg10 = model.SettingsOpen ? "open" : "";
    className = toText(printf("settings %s"))(arg10);
    return react.createElement("div", {
        className: className,
        onClick: (ev) => {
            if (ev.target.className === "settings open") {
                dispatch(new Msg(6));
            }
        },
    }, react.createElement("div", {
        className: "inner",
    }, react.createElement("h1", {
        className: "text-center",
    }, react.createElement("i", {
        className: "fas fa-times close",
        onClick: (_arg1) => {
            dispatch(new Msg(6));
        },
    }), "Settings"), inner));
}

export function tabs(model, dispatch) {
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
        let page, query, hash, arg10;
        return navItem_1([new NavItemProps(2, singleton(new Prop(0, label)))], [navLink([new NavLinkProps(3, singleton(new HTMLAttr(94, (page = toHash(tab_1), (query = (hash = window.location.hash, (hash.indexOf("?") >= 0) ? (arg10 = hash.split("?")[1], toText(printf("?%s"))(arg10)) : ""), toText(printf("%s%s"))(page)(query)))))), new NavLinkProps(0, isActive)], [label])]);
    };
    const navItems = ofArray([navItem(new ActiveTab(0), "Home", equals(model.ActiveTab, new ActiveTab(0))), navItem(new ActiveTab(1), "FSharp Tokens", equals(model.ActiveTab, new ActiveTab(1))), navItem(new ActiveTab(2), "AST", equals(model.ActiveTab, new ActiveTab(2))), navItem(new ActiveTab(3), "Trivia", equals(model.ActiveTab, new ActiveTab(3))), navItem(new ActiveTab(4, new FantomasMode(3)), "Fantomas", (model.ActiveTab.tag === 4) ? true : false)]);
    return react.createElement("div", {
        className: "col-7 h-100",
    }, settings(model, dispatch, patternInput[1]), nav([new NavProps(0, true), new NavProps(9, singleton(new HTMLAttr(64, "")))], [toArray(navItems)]), react.createElement("div", {
        id: "tab-content",
    }, patternInput[0], react.createElement("div", {
        id: "commands",
    }, patternInput[2])));
}

