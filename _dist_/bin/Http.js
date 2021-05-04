import { Types_RequestProperties } from "./.fable/Fable.Fetch.2.2.0/Fetch.fs.js";
import { ofArray } from "./.fable/fable-library.3.1.15/List.js";
import { keyValueList } from "./.fable/fable-library.3.1.15/MapUtil.js";
import { PromiseBuilder__Delay_62FBFDE1, PromiseBuilder__Run_212F1D4B } from "./.fable/Fable.Promise.2.2.0/Promise.fs.js";
import { promise } from "./.fable/Fable.Promise.2.2.0/PromiseImpl.fs.js";

export function postJson(url, body) {
    let props;
    const pr = fetch(url, (props = ofArray([new Types_RequestProperties(1, {
        ["Content-Type"]: "application/json",
    }), new Types_RequestProperties(0, "POST"), new Types_RequestProperties(2, body)]), keyValueList(props, 1)));
    return pr.then(((res) => PromiseBuilder__Run_212F1D4B(promise, PromiseBuilder__Delay_62FBFDE1(promise, () => (res.text().then(((_arg1) => (Promise.resolve([res.status, _arg1])))))))));
}

export function getText(url) {
    let props;
    const pr = fetch(url, (props = ofArray([new Types_RequestProperties(1, {
        ["Content-Type"]: "application/json",
    }), new Types_RequestProperties(0, "GET")]), keyValueList(props, 1)));
    return pr.then(((res) => res.text()));
}

