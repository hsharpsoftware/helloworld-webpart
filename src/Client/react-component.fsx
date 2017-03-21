
#r "../../node_modules/fable-core/Fable.Core.dll"
#r "../../node_modules/fable-react/Fable.React.dll"
#r "../../node_modules/fable-import-sp-pnp-js/fable-import-sp-pnp-js.dll"
#r "../../node_modules/fable-powerpack/Fable.PowerPack.dll"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack
open HSharp2

module R = Fable.Helpers.React
open R.Props

type [<Pojo>] SampleAppProps = { title: string }    

type [<Pojo>] SampleAppState = { title: string }  

type SampleApp(props) =
    inherit React.Component<SampleAppProps,SampleAppState>(props)

    do 
        base.setInitState( { title = "(connecting...)" } )

    member this.render () =   
        let state = this.state     
        R.div [] [
            R.header [ ClassName "header" ] [
                R.h1 [] [ R.str (sprintf "todos on %s" (state.title) ) ]
                R.input [
                    ClassName "new-todo"
                    Placeholder "What needs to be done?"
                    AutoFocus true
                ] []
            ]
        ]
    member this.componentDidMount () = 
        if isWorkbench then
            this.setState( { title = "localhost" } ) 
        else
            ( pnp.Globals.sp.web 
            |> selectQ( [|"Title"|] ) ).get()  
            |> Promise.iter( 
                fun w -> 
                    let web = (w :?> sharepoint.webs.Web )
                    this.setState( { title = web?Title.ToString() } ) 
            )                  

let renderApp (elementId:string) = 
    ReactDom.render(
        R.com<SampleApp,_,_> { title = "(loading...)" } [],
        Browser.document.getElementById(elementId)
    )
  

