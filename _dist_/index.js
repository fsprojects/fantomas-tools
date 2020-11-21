import __SNOWPACK_ENV__ from '../__snowpack__/env.js';
import.meta.env = __SNOWPACK_ENV__;

import React from "../web_modules/react.js";
import ReactDOM from "../web_modules/react-dom.js";
import "./bin/App.js";
import "./styles/style.css.proxy.js";
if (import.meta.hot) {
  import.meta.hot.accept();
}
