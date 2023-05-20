module ASTViewer.GetAST

open System.Reflection
open FSharp.Compiler.Text
open Thoth.Json.Net
open ASTViewer.Shared
open ASTViewer.Server
open Fantomas.FCS.Parse

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
                ExpandedAST.getExpandedAST ast
            else
                $"%A{ast}"

        Encoders.encodeResponse astString errors |> Encode.toString 2 |> ASTResponse.Ok

    | Ok _ -> ASTResponse.TooLarge
    | Error err -> ASTResponse.InternalError $"%A{err}"
