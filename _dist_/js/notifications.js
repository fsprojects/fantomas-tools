import { Notyf } from '../../web_modules/notyf.js';
import '../../web_modules/notyf/notyf.min.css.proxy.js';

const notyf = new Notyf();

export function showSuccess(message) {
    notyf.success(message);
}

export function showError(message) {
    notyf.error(message)
}