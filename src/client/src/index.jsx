import React from 'react';
import ReactDOM from 'react-dom';
import './bin/App.js';
import './styles/style.sass';
import 'notyf/notyf.min.css';

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/#hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept();
}
