
#r "../../node_modules/fable-core/Fable.Core.dll"
#r "../../node_modules/fable-react/Fable.React.dll"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

module R = Fable.Helpers.React
open R.Props

type TodoApp(props) =
    inherit React.Component<obj,obj>(props)
    do base.setInitState()

    member this.render () =
        R.div [] [
            R.header [ ClassName "header" ] [
                R.h1 [] [ R.str "todos" ]
                R.input [
                    ClassName "new-todo"
                    Placeholder "What needs to be done?"
                    AutoFocus true
                ] []
            ]
        ]

let renderTodoApp (elementId:string) = 
    ReactDom.render(
        R.com<TodoApp,_,_> [] [],
        Browser.document.getElementById(elementId)
    )
