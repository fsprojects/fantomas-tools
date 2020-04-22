import { Notyf } from 'notyf';
import 'notyf/notyf.min.css';

const notyf = new Notyf();

export function showSuccess(message) {
    notyf.success(message);
}

export function showError(message) {
    notyf.error(message)
}