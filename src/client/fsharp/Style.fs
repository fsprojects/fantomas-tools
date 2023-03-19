namespace FantomasTools.Client

open Zanaptak.TypedCssClasses

module private Config =
    [<Literal>]
    let sassFile = __SOURCE_DIRECTORY__ + "/../src/styles/style.sass"

type Style =
    CssClasses<Config.sassFile, Naming.PascalCase, commandFile="dotnet", argumentPrefix="fsi ../sass.fsx", logFile="TypedCssClasses.log">
