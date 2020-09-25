namespace ASTViewer.Server

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree
open System.Net
open System.Net.Http
open Thoth.Json.Net
open ASTViewer.Shared
open ASTViewer.Server

module Const =
    let sourceSizeLimit = 100 * 1024

module Result =
    let attempt f =
        try
            Result.Ok <| f ()
        with e -> Error e

module Async =
    let inline map f a = async.Bind(a, f >> async.Return)

    let inline bind f a = async.Bind(a, f)

module Reflection =
    open FSharp.Reflection

    let mapRecordToType<'t> (x: obj) =
        let values = FSharpValue.GetRecordFields x
        FSharpValue.MakeRecord(typeof<'t>, values) :?> 't

module GetAST =
    let private assemblies =
        [| "System.Collections.dll"
           "System.Core.dll"
           "System.Data.dll"
           "System.dll"
           "System.Drawing.dll"
           "System.IO.dll"
           "System.Linq.dll"
           "System.Linq.Expressions.dll"
           "System.Net.Requests.dll"
           "System.Numerics.dll"
           "System.Reflection.dll"
           "System.Runtime.dll"
           "System.Runtime.Numerics.dll"
           "System.Runtime.Remoting.dll"
           "System.Runtime.Serialization.Formatters.Soap.dll"
           "System.Threading.dll"
           "System.Threading.Tasks.dll"
           "System.Web.dll"
           "System.Web.Services.dll"
           "System.Windows.Forms.dll"
           "System.Xml.dll" |]

    let private additionalRefs =
        let refs =
            Directory.EnumerateFiles(Path.GetDirectoryName(typeof<System.Object>.Assembly.Location))
            |> Seq.filter (fun path -> Array.contains (Path.GetFileName(path)) assemblies)
            |> Seq.map (sprintf "-r:%s")

        [| "--simpleresolution"
           "--noframework"
           yield! refs |]

    let private sharedChecker =
        lazy (FSharpChecker.Create(keepAssemblyContents = true))

    let private sendJson json =
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let private sendText text =
        new HttpResponseMessage(HttpStatusCode.OK,
                                Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text"))

    let private sendInternalError err =
        new HttpResponseMessage(HttpStatusCode.InternalServerError,
                                Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

    let private sendTooLargeError () =
        new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge,
                                Content =
                                    new StringContent("File was too large",
                                                      System.Text.Encoding.UTF8,
                                                      "application/text"))

    let private sendBadRequest error =
        new HttpResponseMessage(HttpStatusCode.BadRequest,
                                Content = new StringContent(error, System.Text.Encoding.UTF8, "application/text"))

    let private notFound () =
        let json = Encode.string "Not found" |> Encode.toString 4

        new HttpResponseMessage(HttpStatusCode.NotFound,
                                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

    let getProjectOptionsFromScript file source defines (checker: FSharpChecker) =
        async {
            let otherFlags =
                defines
                |> Array.map (sprintf "-d:%s")
                |> Array.append additionalRefs

            let! (opts, errors) =
                checker.GetProjectOptionsFromScript
                    (file, source, otherFlags = otherFlags, assumeDotNetFramework = false, useSdkRefs = true)

            match errors with
            | [] -> return opts
            | errs -> return failwithf "Errors getting project options: %A" errs
        }

    let private getVersion () =
        let version =
            let assembly = typeof<FSharpChecker>.Assembly

            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        sendText version

    let parseAST (log: ILogger) ({ SourceCode = source; Defines = defines; IsFsi = isFsi }) =
        let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"
        // create ISourceText
        let sourceText = FSharp.Compiler.Text.SourceText.ofString (source)
        // Create an interactive checker instance (ignore notifications)
        let checker = sharedChecker.Value

        async {
            // Get compiler options for a single script file
            let! checkOptions =
                checker
                |> getProjectOptionsFromScript fileName sourceText defines
                |> Async.map (fun projOpts ->
                    checker.GetParsingOptionsFromProjectOptions projOpts
                    |> fst)

            // Run the first phase (untyped parsing) of the compiler
            let untypedRes =
                checker.ParseFile(fileName, sourceText, checkOptions)
                |> Async.RunSynchronously

            if untypedRes.ParseHadErrors then
                let errors =
                    untypedRes.Errors
                    |> Array.filter (fun e -> e.Severity = FSharpErrorSeverity.Error)

                if not <| Array.isEmpty errors
                then log.LogError(sprintf "Parsing failed with errors: %A\nAnd options: %A" errors checkOptions)

                return Error errors
            else
                match untypedRes.ParseTree with
                | Some tree -> return Result.Ok tree
                | _ -> return Error Array.empty // Not sure this branch can be reached.
        }

    let private getAST log (req: HttpRequest) =
        async {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
            let parseRequest = Decoders.decodeInputRequest json

            match parseRequest with
            | Result.Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
                let! astResult = parseAST log input

                match astResult with
                | Result.Ok ast ->
                    let node =
                        match ast with
                        | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (_, _, _, _, hds, mns, _)) ->
                            Fantomas.AstTransformer.astToNode hds mns

                        | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (_, _, _, _, mns)) ->
                            Fantomas.AstTransformer.sigAstToNode mns
                        |> Encoders.astNodeEncoder

                    let responseJson =
                        Encoders.encodeResponse node (sprintf "%A" ast)
                        |> Encode.toString 2

                    return sendJson responseJson
                | Error error -> return sendBadRequest (sprintf "%A" error)
            | Result.Ok _ -> return sendTooLargeError ()
            | Error err -> return sendInternalError (sprintf "%A" err)
        }

    let private parseTypedAST ({ SourceCode = source; Defines = defines; IsFsi = isFsi }) =
        let fileName = if isFsi then "tmp.fsi" else "tmp.fsx"
        let sourceText = FSharp.Compiler.Text.SourceText.ofString (source)
        let checker = sharedChecker.Value

        async {
            let! options =
                checker
                |> getProjectOptionsFromScript fileName sourceText defines

            let! (parseRes, typedRes) = checker.ParseAndCheckFileInProject(fileName, 1, sourceText, options)

            match typedRes with
            | FSharpCheckFileAnswer.Aborted ->
                return
                    Error
                        (sprintf "Type checking aborted. With Parse errors:\n%A\n And with options: \n%A"
                             parseRes.Errors options)
            | FSharpCheckFileAnswer.Succeeded res ->
                match res.ImplementationFile with
                | None -> return Error(sprintf "%A" res.Errors)
                | Some fc -> return Result.Ok fc.Declarations
        }


    let private getTypedAST (req: HttpRequest) =
        async {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
            let parseRequest = Decoders.decodeInputRequest json

            match parseRequest with
            | Result.Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
                let! tastResult = parseTypedAST input

                match tastResult with
                | Result.Ok tast ->
                    let node =
                        TastTransformer.tastToNode tast
                        |> Encoders.tastNodeEncoder

                    let responseJson =
                        Encoders.encodeResponse node (sprintf "%A" tast)
                        |> Encode.toString 2

                    return sendJson responseJson

                | Error error -> return sendInternalError (sprintf "%A" error)

            | Result.Ok _ -> return sendTooLargeError ()

            | Error err -> return sendInternalError (sprintf "%A" err)
        }

    [<FunctionName("GetAST")>]
    let run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequest)
        (log: ILogger)
        =
        async {
            log.LogInformation("F# HTTP trigger function processed a request.")
            let path = req.Path.Value.ToLower()
            let method = req.Method.ToUpper()

            match method, path with
            | "GET", "/api/version" -> return getVersion ()
            | "POST", "/api/untyped-ast" -> return! getAST log req
            | "POST", "/api/typed-ast" -> return! getTypedAST req
            | _ -> return notFound ()
        }
        |> Async.StartAsTask
