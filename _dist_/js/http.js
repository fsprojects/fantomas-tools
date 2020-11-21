export function postJson(url, body) {
  return fetch(url, {
    headers: { "Content-Type": "application/json" },
    body,
    method: "POST"
  }).then((res) => Promise.all([Promise.resolve(res.status), res.text()]));
}

export function getJson(url) {
  return fetch(url, {
    headers: { "Content-Type": "application/json" },
    method: "GET"
  }).then((res) => res.text());
}
