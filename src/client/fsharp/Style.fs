namespace FantomasTools.Client

open Zanaptak.TypedCssClasses

type Style =
    CssClasses<"../src/styles/style.sass", Naming.PascalCase, commandFile= @"C:\Program Files\nodejs\npx.cmd", argumentPrefix="sass", logFile="TypedCssClasses.log">
