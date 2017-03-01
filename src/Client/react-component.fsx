
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
        R.div [] []
