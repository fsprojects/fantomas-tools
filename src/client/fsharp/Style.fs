namespace FantomasTools.Client

open Zanaptak.TypedCssClasses

module private Config =
    [<Literal>]
    let cssFile = __SOURCE_DIRECTORY__ + "/../src/styles/style.css"

type Style = CssClasses<Config.cssFile, Naming.PascalCase, logFile="TypedCssClasses.log">
