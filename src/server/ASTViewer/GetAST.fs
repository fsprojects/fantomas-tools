module ASTViewer.GetAST

open FSharp.Compiler.CodeAnalysis
open Thoth.Json.Net
open ASTViewer.Shared
open ASTViewer.Server

module Const =
    let sourceSizeLimit = 100 * 1024

let private checker = FSharpChecker.Create(keepAssemblyContents = true)

let getVersion () =
    let assembly = typeof<FSharpChecker>.Assembly

    let version = assembly.GetName().Version
    sprintf "%i.%i.%i" version.Major version.Minor version.Revision

let private parseAST
    ({ SourceCode = source
       Defines = defines
       IsFsi = isFsi })
    =
    let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"
    // create ISourceText
    let sourceText = FSharp.Compiler.Text.SourceText.ofString source

    async {
        // Get compiler options for a single script file
        let parsingOptions =
            { FSharpParsingOptions.Default with
                SourceFiles = [| fileName |]
                IsExe = true
                ConditionalCompilationDefines = Array.toList defines
                LangVersionText = "preview" }

        // Run the first phase (untyped parsing) of the compiler
        let untypedRes =
            checker.ParseFile(fileName, sourceText, parsingOptions)
            |> Async.RunSynchronously

        return Result.Ok(untypedRes.ParseTree, untypedRes.Diagnostics)
    }

[<RequireQualifiedAccess>]
type ASTResponse =
    | Ok of json: string
    | InvalidAST of string
    | TooLarge
    | InternalError of string

let getUntypedAST json : Async<ASTResponse> =
    async {
        let parseRequest = Decoders.decodeInputRequest json

        match parseRequest with
        | Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
            let! astResult = parseAST input

            match astResult with
            | Ok (ast, errors) ->
                let responseJson =
                    Encoders.encodeResponse (sprintf "%A" ast) errors
                    |> Encode.toString 2

                return ASTResponse.Ok responseJson
            | Error error -> return ASTResponse.InvalidAST(sprintf "%A" error)
        | Ok _ -> return ASTResponse.TooLarge
        | Error err -> return ASTResponse.InternalError(sprintf "%A" err)
    }

let private getProjectOptionsFromScript file source defines (checker: FSharpChecker) =
    async {
        let otherFlags = defines |> Array.map (sprintf "-d:%s")

        let! opts, errors =
            checker.GetProjectOptionsFromScript(
                file,
                source,
                otherFlags = otherFlags,
                assumeDotNetFramework = false,
                useSdkRefs = true
            )

        match errors with
        | [] -> return opts
        | errs -> return failwithf "Errors getting project options: %A" errs
    }

let private parseTypedAST
    ({ SourceCode = source
       Defines = defines
       IsFsi = isFsi })
    =
    let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"

    let sourceText = FSharp.Compiler.Text.SourceText.ofString source

    async {
        let! options =
            checker
            |> getProjectOptionsFromScript fileName sourceText defines

        let! parseRes, typedRes = checker.ParseAndCheckFileInProject(fileName, 1, sourceText, options)

        match typedRes with
        | FSharpCheckFileAnswer.Aborted ->
            return
                Error(
                    sprintf
                        "Type checking aborted. With Parse errors:\n%A\n And with options: \n%A"
                        parseRes.Diagnostics
                        options
                )
        | FSharpCheckFileAnswer.Succeeded res ->
            match res.ImplementationFile with
            | None -> return Error(sprintf "%A" res.Diagnostics)
            | Some fc -> return Result.Ok(fc.Declarations, parseRes.Diagnostics)
    }

let getTypedAST json : Async<ASTResponse> =
    async {
        let parseRequest = Decoders.decodeInputRequest json

        match parseRequest with
        | Result.Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
            let! tastResult = parseTypedAST input

            match tastResult with
            | Result.Ok (tast, errors) ->
                let responseJson =
                    Encoders.encodeResponse (sprintf "%A" tast) errors
                    |> Encode.toString 2

                return ASTResponse.Ok responseJson

            | Error error -> return ASTResponse.InternalError(string error)

        | Result.Ok _ -> return ASTResponse.TooLarge

        | Error err -> return ASTResponse.InternalError(string err)
    }
