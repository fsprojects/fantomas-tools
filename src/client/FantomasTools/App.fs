module FantomasTools.Client.App

open Elmish
open Elmish.React
open FantomasTools.Client

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Fable.Core.JsInterop.importSideEffects "../styles/style.sass"

Program.mkProgram State.init State.update View.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
|> Program.run