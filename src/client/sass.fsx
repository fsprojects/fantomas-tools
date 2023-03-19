#r "nuget: JavaScriptEngineSwitcher.V8.Native.win-x64"
#r "nuget: JavaScriptEngineSwitcher.V8.Native.linux-x64"
#r "nuget: JavaScriptEngineSwitcher.V8.Native.osx-x64"
#r "nuget: JavaScriptEngineSwitcher.V8, 3.20.10"
#r "nuget: DartSassHost, 1.0.2"

open System.IO
open DartSassHost
open JavaScriptEngineSwitcher.V8

match Seq.tryLast fsi.CommandLineArgs with
| Some filePath when File.Exists filePath ->
    let sassCompiler = new SassCompiler(V8JsEngineFactory())
    let result = sassCompiler.CompileFile(filePath)
    printfn $"%s{result.CompiledContent}"
| Some nonExistingFile -> printfn $"(* File '%s{nonExistingFile}' does not exist. *)"
| None -> printfn "(* file path not found *)"
