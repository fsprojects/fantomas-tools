export function setGetParam(key, value) {
  if (history.pushState) {
    var params = new URLSearchParams(window.location.search);
    params.set(key, value);
    var newUrl =
      window.location.protocol +
      "//" +
      window.location.host +
      window.location.pathname +
      "?" +
      params.toString();
    window.history.pushState({ path: newUrl }, "", newUrl);
  }
}

import lzString from "lz-string";

export function compressToEncodedURIComponent(x) {
  return lzString.compressToEncodedURIComponent(x);
}

export function decompressFromEncodedURIComponent(x) {
  return lzString.decompressFromEncodedURIComponent(x);
}
