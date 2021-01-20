import { Notyf } from '../../_snowpack/pkg/notyf.js';
import '../../_snowpack/pkg/notyf/notyf.min.css.proxy.js';

const notyf = new Notyf();

export function showSuccess(message) {
    notyf.success(message);
}

export function showError(message) {
    notyf.error(message)
}