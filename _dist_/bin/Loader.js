import { SpinnerProps, spinner } from "./bin/.fable/Fable.Reactstrap.0.5.1/Spinner.fs.js";
import * as react from "../../web_modules/react.js";

export const loader = (() => {
    let children;
    const children_2 = [(children = [spinner([new SpinnerProps(2, "primary")], [])], react.createElement("div", {
        className: "inner",
    }, ...children))];
    return react.createElement("div", {
        className: "loader",
    }, ...children_2);
})();

