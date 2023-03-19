namespace FantomasTools.Client

open Zanaptak.TypedCssClasses

module private Config =
    [<Literal>]
    let sassFile = __SOURCE_DIRECTORY__ + "/../src/styles/style.sass"

type Style =
    CssClasses<Config.sassFile, Naming.PascalCase, commandFile= @"C:\Program Files\nodejs\npx.cmd", argumentPrefix="sass", logFile="TypedCssClasses.log">
