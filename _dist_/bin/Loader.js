import * as react from "../../web_modules/react.js";
import { SpinnerProps, spinner } from "./.fable/Fable.Reactstrap.0.5.1/Spinner.fs.js";

export const loader = react.createElement("div", {
    className: "loader",
}, react.createElement("div", {
    className: "inner",
}, spinner([new SpinnerProps(2, "primary")], [])));

