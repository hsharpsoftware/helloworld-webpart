
#r "../../node_modules/fable-core/Fable.Core.dll"
#r "../../node_modules/fable-react/Fable.React.dll"
#r "../../node_modules/fable-import-sp-pnp-js/fable-import-sp-pnp-js.dll"
#r "../../node_modules/fable-powerpack/Fable.PowerPack.dll"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack

module R = Fable.Helpers.React
open R.Props

type TodoApp(props) =
    inherit React.Component<obj,obj>(props)

    let mutable web: sharepoint.webs.Web = null

    let getWeb () =
        let _web = ref null

        (pnp.Globals.sp.web.select( [|"Title"|] ) :> sharepoint.queryable.Queryable).get()  
        |> Promise.iter( 
            fun w -> 
                _web := (w :?> sharepoint.webs.Web ) 
                web <- !_web
        )              

    do 
        base.setInitState()
        getWeb ()

    member this.render () =        
        R.div [] [
            R.header [ ClassName "header" ] [
                R.h1 [] [ R.str (sprintf "todos on %s" (if isNull web then "localhost" else web?Title.ToString() ) ) ]
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

let testWebTitle () =
        (pnp.Globals.sp.web.select( [|"Title"|] ) :> sharepoint.queryable.Queryable).get()  
        |> Promise.iter( 
            fun w -> 
                Fable.Import.Browser.console.log "_web"
                Fable.Import.Browser.console.log (w :?> sharepoint.webs.Web ) 
        )              

