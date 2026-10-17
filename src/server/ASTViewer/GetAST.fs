module ASTViewer.GetAST

open System.Reflection
open Thoth.Json.Core
open Fantomas.FCS.Text
open Fantomas.FCS.Parse
open ASTViewer.Shared
open ASTViewer.Server

module Const =
    let sourceSizeLimit = 100 * 1024

let getVersion () =
    let assembly = typeof<FSharpParserDiagnostic>.Assembly

    match Option.ofObj (assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()) with
    | Some attr -> attr.InformationalVersion
    | None ->
        let version = assembly.GetName().Version
        $"%i{version.Major}.%i{version.Minor}.%i{version.Revision}"

[<RequireQualifiedAccess; Struct>]
type ASTResponse =
    | Ok of json: string
    | TooLarge
    | InternalError of string

let getUntypedAST json : ASTResponse =
    let parseRequest = Decoders.decodeInputRequest json

    match parseRequest with
    | Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
        let ast, errors =
            parseFile input.IsFsi (SourceText.ofString input.SourceCode) (List.ofArray input.Defines)

        let astString =
            try
                PrintAST.printAST input.Expand ast
            with
            // Too much code for the memory the function has, which LambdaRuntime answers with a 413.
            | :? System.OutOfMemoryException -> reraise ()
            | ex -> $"Failed to print AST, please contribute a fix for this.\nError:%s{ex.Message}"

        Encoders.encodeResponse astString errors
        |> LambdaJson.toString
        |> ASTResponse.Ok

    | Ok _ -> ASTResponse.TooLarge
    | Error err -> ASTResponse.InternalError err
