export function setGetParam(value) {
  if (history.pushState) {
    var hashPieces = window.location.hash.split("?");
    var hash = (hashPieces.length && hashPieces[0]) || "";
    var params = new URLSearchParams();
    params.set("data", value);
    var newUrl =
      window.location.protocol +
      "//" +
      window.location.host +
      window.location.pathname +
      hash +
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
