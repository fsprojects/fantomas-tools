#!/usr/bin/env -S dotnet fsi

// Generates SyntaxReader.fs: one function per type of the untyped syntax tree that turns a value into a
// SyntaxNode, so the AST viewer reads the tree without F# reflection and can run under Native AOT.
//
// The types come from SyntaxTree.fsi and SyntaxTrivia.fsi of the FCS commit the preview Fantomas
// checkout builds Fantomas.FCS from. Run this again after each update of that commit:
//
//   dotnet fsi src/server/ASTViewer/GenerateSyntaxReader.fsx
//
// A type the syntax files use but do not define is a leaf: it needs a function of the same name, starting
// with a lowercase letter, in the `Leaf` module of SyntaxNode.fs. The script stops and names the type when that function is missing.

#r "nuget: Fantomas.FCS, 8.0.10"

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.RegularExpressions
open Fantomas.FCS.Syntax
open Fantomas.FCS.Text

let repositoryRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "..", "..", ".."))

let outputFile = Path.Combine(__SOURCE_DIRECTORY__, "SyntaxReader.fs")

/// The value of an MSBuild property, read straight from the XML.
let readProperty (file: string) (name: string) : string =
    let m = Regex.Match(File.ReadAllText file, $"<%s{name}>(.*?)</%s{name}>")

    if not m.Success then
        failwith $"%s{name} not found in %s{file}"

    m.Groups[1].Value

/// The Fantomas checkout the AST viewer compiles against, see `FantomasPreviewRepository`.
let fantomasCheckout =
    readProperty (Path.Combine(repositoryRoot, "Directory.Build.props")) "FantomasPreviewRepository"
    |> fun path -> path.Replace("$(MSBuildThisFileDirectory)", repositoryRoot + "/").Replace('\\', '/')
    |> Path.GetFullPath

let fcsCommit =
    readProperty (Path.Combine(fantomasCheckout, "Directory.Build.props")) "FCSCommitHash"

let syntaxFiles =
    [ "SyntaxTree.fsi"; "SyntaxTrivia.fsi" ]
    |> List.map (fun name -> Path.Combine(fantomasCheckout, ".deps", fcsCommit, "src", "Compiler", "SyntaxTree", name))

for file in syntaxFiles do
    if not (File.Exists file) then
        failwith $"%s{file} does not exist, build the Fantomas checkouts first: dotnet fsi build.fsx -- -p Fantomas-Git"

type Field = { Name: string option; Type: SynType }

type TypeShape =
    | Union of cases: (string * Field list) list
    | Record of fields: (string * SynType) list
    | Enum of cases: string list
    | Abbreviation of SynType

/// Every type the syntax files define, by name.
let shapes: Map<string, TypeShape> =
    syntaxFiles
    |> List.collect (fun file ->
        let ast, diagnostics =
            Fantomas.FCS.Parse.parseFile true (SourceText.ofString (File.ReadAllText file)) []

        if not diagnostics.IsEmpty then
            failwith $"Could not parse %s{file}: %A{diagnostics}"

        match ast with
        | ParsedInput.SigFile(ParsedSigFileInput(contents = modules)) ->
            [
                for SynModuleOrNamespaceSig(decls = decls) in modules do
                    for decl in decls do
                        match decl with
                        | SynModuleSigDecl.Types(types = types) ->
                            for SynTypeDefnSig(
                                typeInfo = SynComponentInfo(typeParams = typeParams; synType = synType)
                                typeRepr = repr) in types do
                                let name =
                                    match synType with
                                    | Some(SynType.LongIdent(SynLongIdent(id = longId))) -> (List.last longId).idText
                                    | _ -> failwith $"Unexpected type name %A{synType}"

                                if typeParams.IsSome then
                                    failwith $"%s{name} has type parameters, the generator does not handle those"

                                match repr with
                                | SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Union(unionCases = cases)) ->
                                    let cases =
                                        cases
                                        |> List.map
                                            (fun (SynUnionCase(ident = SynIdent(caseIdent, _); caseType = kind)) ->
                                                match kind with
                                                | SynUnionCaseKind.Fields fields ->
                                                    caseIdent.idText,
                                                    fields
                                                    |> List.map
                                                        (fun (SynField(idOpt = idOpt; fieldType = fieldType)) ->
                                                            {
                                                                Name = idOpt |> Option.map _.idText
                                                                Type = fieldType
                                                            })
                                                | SynUnionCaseKind.FullType _ ->
                                                    failwith $"%s{name}.%s{caseIdent.idText} has a full type")

                                    yield name, Union cases
                                | SynTypeDefnSigRepr.Simple(
                                    repr = SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = fields)) ->
                                    let fields =
                                        fields
                                        |> List.map (function
                                            | SynFieldOrSpread.Field(SynField(idOpt = idOpt; fieldType = fieldType)) ->
                                                (Option.get idOpt).idText, fieldType
                                            | SynFieldOrSpread.Spread _ -> failwith $"%s{name} has a spread field")

                                    yield name, Record fields
                                | SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Enum(cases = cases)) ->
                                    yield
                                        name,
                                        Enum(
                                            cases
                                            |> List.map (fun (SynEnumCase(ident = SynIdent(caseIdent, _))) ->
                                                caseIdent.idText)
                                        )
                                | SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.TypeAbbrev(rhsType = rhsType)) ->
                                    yield name, Abbreviation rhsType
                                | _ ->
                                    // Classes and structs such as Ident are leaves.
                                    ()
                        | _ -> ()
            ]
        | ParsedInput.ImplFile _ -> failwith $"%s{file} is not a signature file")
    |> Map.ofList

/// Names of the functions in the `Leaf` module of SyntaxNode.fs.
let leaves: Set<string> =
    let syntaxNode =
        File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, "SyntaxNode.fs"))

    Regex.Matches(syntaxNode, @"^    let (\w+) ", RegexOptions.Multiline)
    |> Seq.map _.Groups[1].Value
    |> Set.ofSeq

let readerName (typeName: string) = $"read%s{typeName}"

/// The types that need a reader, in the order their readers are written.
let reachable = ResizeArray<string>()

let typeName (ty: SynType) : string option =
    match ty with
    | SynType.LongIdent(SynLongIdent(id = longId)) -> Some (List.last longId).idText
    | _ -> None

/// An F# expression of type `'T -> SyntaxNode` for the syntax type `ty`.
/// `depth` keeps the names of nested lambda parameters apart.
let rec mapper (depth: int) (ty: SynType) : string =
    match ty with
    | SynType.Paren(innerType = innerType) -> mapper depth innerType
    | SynType.App(typeName = SynType.LongIdent(SynLongIdent(id = [ name ])); typeArgs = [ argument ]) ->
        let inner = mapper (depth + 1) argument

        match name.idText with
        | "option" -> $"(Option.map %s{inner} >> SyntaxNode.Option)"
        | "list" -> $"(List.map %s{inner} >> SyntaxNode.List)"
        | "Set" -> $"(Set.toList >> List.map %s{inner} >> SyntaxNode.Set)"
        | "ref" -> $"(fun (cell%i{depth}: _ ref) -> SyntaxNode.Record [ \"contents\", %s{inner} cell%i{depth}.Value ])"
        | other -> failwith $"Generic type %s{other} is not supported"
    | SynType.Array(rank = 1; elementType = elementType) ->
        $"(Array.map %s{mapper (depth + 1) elementType} >> Array.toList >> SyntaxNode.Array)"
    | SynType.Tuple(path = path) ->
        let items =
            path
            |> List.choose (function
                | SynTupleTypeSegment.Type t -> Some t
                | _ -> None)
            |> List.mapi (fun index t -> $"t%i{depth}_%i{index}", t)

        let pattern = items |> List.map fst |> String.concat ", "

        let values =
            items
            |> List.map (fun (name, t) -> $"%s{mapper (depth + 1) t} %s{name}")
            |> String.concat "; "

        $"(fun (%s{pattern}) -> SyntaxNode.Tuple [ %s{values} ])"
    | _ ->

        match typeName ty with
        | None -> failwith $"Type %A{ty} is not supported"
        | Some name ->

            match Map.tryFind name shapes with
            | Some(Abbreviation rhs) -> mapper depth rhs
            | Some _ ->
                if not (reachable.Contains name) then
                    reachable.Add name

                readerName name
            | None ->

                let leaf = name.Substring(0, 1).ToLowerInvariant() + name.Substring 1

                if leaves.Contains leaf then
                    $"Leaf.%s{leaf}"
                else
                    failwith
                        $"%s{name} is not defined in the syntax files: add a function `%s{leaf}` to the Leaf module in SyntaxNode.fs"

let writeReader (sb: StringBuilder) (name: string) =
    sb.AppendLine $"let %s{readerName name} (syntax: %s{name}) : SyntaxNode ="
    |> ignore

    match shapes[name] with
    | Union cases ->
        sb.AppendLine "    match syntax with" |> ignore

        for caseName, fields in cases do
            let fields =
                fields
                |> List.mapi (fun index field ->
                    let displayName =
                        match field.Name with
                        | Some fieldName -> fieldName
                        | None when fields.Length = 1 -> "Item"
                        | None -> $"Item%i{index + 1}"

                    let variable =
                        PrettyNaming.NormalizeIdentifierBackticks(
                            displayName.Substring(0, 1).ToLowerInvariant() + displayName.Substring 1
                        )

                    displayName, variable, field.Type)

            let pattern =
                if fields.IsEmpty then
                    ""
                else
                    fields
                    |> List.map (fun (_, variable, _) -> variable)
                    |> String.concat ", "
                    |> sprintf "(%s)"

            let values =
                fields
                |> List.map (fun (displayName, variable, t) -> $"\"%s{displayName}\", %s{mapper 0 t} %s{variable}")
                |> String.concat "; "

            sb.AppendLine
                $"    | %s{name}.%s{caseName}%s{pattern} -> SyntaxNode.Case(\"%s{name}\", \"%s{caseName}\", [ %s{values} ])"
            |> ignore
    | Record fields ->
        let values =
            fields
            |> List.map (fun (fieldName, t) -> $"\"%s{fieldName}\", %s{mapper 0 t} syntax.%s{fieldName}")
            |> String.concat "; "

        sb.AppendLine $"    SyntaxNode.Record [ %s{values} ]" |> ignore
    | Enum cases ->
        sb.AppendLine "    match syntax with" |> ignore

        for caseName in cases do
            sb.AppendLine $"    | %s{name}.%s{caseName} -> SyntaxNode.Literal \"%s{name}.%s{caseName}\""
            |> ignore

        sb.AppendLine $"    | _ -> SyntaxNode.Literal(\"enum<%s{name}>(\" + (int syntax).ToString() + \")\")"
        |> ignore
    | Abbreviation _ -> failwith $"%s{name} is an abbreviation and has no reader"

    sb.AppendLine() |> ignore

let generate () =
    let sb = StringBuilder()

    sb.AppendLine
        $"// Generated by GenerateSyntaxReader.fsx from SyntaxTree.fsi and SyntaxTrivia.fsi of FCS commit %s{fcsCommit}."
    |> ignore

    sb.AppendLine "// Do not edit this file, run the script again instead."
    |> ignore

    sb.AppendLine "module rec ASTViewer.SyntaxReader" |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine "open Fantomas.FCS.Syntax" |> ignore
    sb.AppendLine "open Fantomas.FCS.SyntaxTrivia" |> ignore
    sb.AppendLine() |> ignore

    reachable.Add "ParsedInput"
    let mutable index = 0

    // Writing a reader can make more types reachable, so the list grows while it is walked.
    while index < reachable.Count do
        writeReader sb reachable[index]
        index <- index + 1

    File.WriteAllText(outputFile, sb.ToString())
    printfn $"Wrote %i{reachable.Count} readers to %s{outputFile}"

generate ()

let fantomas =
    Process.Start(ProcessStartInfo("dotnet", $"fantomas \"%s{outputFile}\"", WorkingDirectory = repositoryRoot))

fantomas.WaitForExit()
exit fantomas.ExitCode
