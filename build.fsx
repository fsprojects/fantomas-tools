#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.JavaScript
open Fake.Tools

Target.create "Fantomas-Git" (fun _ ->
    let targetDir = ".deps" @@ "fantomas"
    Fake.IO.Shell.cleanDir targetDir
    Git.Repository.cloneSingleBranch "." "https://github.com/fsprojects/fantomas.git" "master" targetDir
    DotNet.exec (fun opt -> { opt with WorkingDirectory = targetDir }) "tool" "restore" |> ignore
    DotNet.build (fun opt -> { opt with Configuration = DotNet.BuildConfiguration.Release }) "./.deps/fantomas/src/Fantomas/Fantomas.fsproj"
)

Target.create "Build" ignore

Target.runOrDefault "Build"