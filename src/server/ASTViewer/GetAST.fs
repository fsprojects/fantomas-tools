module ASTViewer.GetAST

open System.Reflection
open Thoth.Json.Net
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
        sprintf "%i.%i.%i" version.Major version.Minor version.Revision

[<RequireQualifiedAccess>]
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
            if input.Expand then
                try
                    ExpandedAST.getExpandedAST ast
                with ex ->
                    $"Failed to expand AST, please contribute a fix for this.\nError:%s{ex.Message}"
            else
                $"%A{ast}"

        Encoders.encodeResponse astString errors |> Encode.toString 2 |> ASTResponse.Ok

    | Ok _ -> ASTResponse.TooLarge
    | Error err -> ASTResponse.InternalError $"%A{err}"
