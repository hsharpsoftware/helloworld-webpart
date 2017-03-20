import { createElement, Component } from "react";
import { setType } from "fable-core/Symbol";
import _Symbol from "fable-core/Symbol";
import { toString, extendInfo } from "fable-core/Util";
import { fsFormat } from "fable-core/String";
import { awaitPromise, startImmediate } from "fable-core/Async";
import { pnp } from "sp-pnp-js";
import { singleton } from "fable-core/AsyncBuilder";
import { render } from "react-dom";
import List from "fable-core/List";
export class TodoApp extends Component {
    [_Symbol.reflection]() {
        return extendInfo(TodoApp, {
            type: "React-component.TodoApp",
            interfaces: [],
            properties: {}
        });
    }

    constructor(props) {
        super(props);
        this.state = null;
        this.web = null;
    }

    render() {
        this.getWeb();
        return createElement("div", {}, createElement("header", {
            className: "header"
        }, createElement("h1", {}, fsFormat("todos on %s")(x => x)(this.web == null ? "localhost" : toString(this.web.Title))), createElement("input", {
            className: "new-todo",
            placeholder: "What needs to be done?",
            autoFocus: true
        })));
    }

    getWeb() {
        (arg00 => {
            startImmediate(arg00);
        })((builder_ => builder_.Delay(() => builder_.Bind(awaitPromise(pnp.sp.web.select("Title").get().then(w => w)), _arg1 => {
            this.web = _arg1;
            return builder_.Zero();
        })))(singleton));
    }

}
setType("React-component.TodoApp", TodoApp);
export function renderTodoApp(elementId) {
    render(createElement(TodoApp, new List()), document.getElementById(elementId));
}
export function testWebTitle() {
    (arg00 => {
        startImmediate(arg00);
    })((builder_ => builder_.Delay(() => builder_.Bind(awaitPromise(pnp.sp.web.select("Title").get().then(w => w)), _arg1 => {
        console.log("_web");
        console.log(_arg1);
        console.log("----------------------------------------------");
        return builder_.Zero();
    })))(singleton));
}
//# sourceMappingURL=react-component.js.map