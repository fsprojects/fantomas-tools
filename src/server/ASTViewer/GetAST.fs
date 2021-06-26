namespace ASTViewer.Server

open System.IO
open System.Threading.Tasks
open System.Net
open System.Net.Http
open Microsoft.Azure.Functions.Worker.Http
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open Thoth.Json.Net
open FSharp.Control.Tasks
open FSharp.Compiler.CodeAnalysis
open ASTViewer.Shared
open ASTViewer.Server

module Const =
    let sourceSizeLimit = 100 * 1024

module Result =
    let attempt f =
        try
            Result.Ok <| f ()
        with
        | e -> Error e

module Async =
    let inline map f a = async.Bind(a, f >> async.Return)

    let inline bind f a = async.Bind(a, f)

module Reflection =
    open FSharp.Reflection

    let mapRecordToType<'t> (x: obj) =
        let values = FSharpValue.GetRecordFields x
        FSharpValue.MakeRecord(typeof<'t>, values) :?> 't

module GetAST =
    let private sharedChecker =
        lazy (FSharpChecker.Create(keepAssemblyContents = true))

    let private sendJson json (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            res.StatusCode <- HttpStatusCode.OK
            do! res.WriteStringAsync(json)
            res.Headers.Add("Content-Type", "application/json")
            return res
        }

    let private sendText (text: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(text, System.Text.Encoding.UTF8)
            res.StatusCode <- HttpStatusCode.OK
            res.Headers.Add("Content-Type", "text/plain")
            return res
        }

    let private sendInternalError (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(error)
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.InternalServerError
            return res
        }

    let private sendTooLargeError (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync("File was too large")
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.RequestEntityTooLarge
            return res
        }

    let private sendBadRequest (error: string) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            do! res.WriteStringAsync(error)
            res.Headers.Add("Content-Type", "application/text")
            res.StatusCode <- HttpStatusCode.BadRequest
            return res
        }

    let private notFound (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            let json = Encode.string "Not found" |> Encode.toString 4
            do! res.WriteStringAsync(json)
            res.StatusCode <- HttpStatusCode.NotFound
            return res
        }

    let getProjectOptionsFromScript file source defines (checker: FSharpChecker) =
        async {
            let otherFlags = defines |> Array.map (sprintf "-d:%s")

            let! (opts, errors) =
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

    let private getVersion (res: HttpResponseData) : Task<HttpResponseData> =
        let version =
            let assembly = typeof<FSharpChecker>.Assembly

            let version = assembly.GetName().Version
            sprintf "%i.%i.%i" version.Major version.Minor version.Revision

        sendText version res

    let parseAST
        (log: ILogger)
        ({ SourceCode = source
           Defines = defines
           IsFsi = isFsi })
        =
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
                |> Async.map
                    (fun projOpts ->
                        checker.GetParsingOptionsFromProjectOptions projOpts
                        |> fst)

            // Run the first phase (untyped parsing) of the compiler
            let! parseResult = checker.ParseFile(fileName, sourceText, checkOptions)

            return (parseResult.ParseTree, parseResult.Diagnostics)
        }

    let private getAST log (req: HttpRequestData) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
            let parseRequest = Decoders.decodeInputRequest json

            match parseRequest with
            | Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
                let! (ast, errors) = parseAST log input

                let responseJson =
                    Encoders.encodeResponse (sprintf "%A" ast) errors
                    |> Encode.toString 2

                return! sendJson responseJson res
            | Ok _ -> return! sendTooLargeError res
            | Error err -> return! sendInternalError (sprintf "%A" err) res
        }

    let private parseTypedAST
        ({ SourceCode = source
           Defines = defines
           IsFsi = isFsi })
        =
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


    let private getTypedAST (req: HttpRequestData) (res: HttpResponseData) : Task<HttpResponseData> =
        task {
            use stream = new StreamReader(req.Body)
            let! json = stream.ReadToEndAsync() |> Async.AwaitTask
            let parseRequest = Decoders.decodeInputRequest json

            match parseRequest with
            | Result.Ok input when (input.SourceCode.Length < Const.sourceSizeLimit) ->
                let! tastResult = parseTypedAST input

                match tastResult with
                | Result.Ok (tast, errors) ->
                    let responseJson =
                        Encoders.encodeResponse (sprintf "%A" tast) errors
                        |> Encode.toString 2

                    return! sendJson responseJson res

                | Error error -> return! sendInternalError (sprintf "%A" error) res

            | Result.Ok _ -> return! sendTooLargeError res

            | Error err -> return! sendInternalError (sprintf "%A" err) res
        }

    [<Function "GetAST">]
    let run
        (
            [<HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*any}")>] req: HttpRequestData,
            executionContext: FunctionContext
        )
        =
        let log : ILogger = executionContext.GetLogger("GetAST")
        log.LogInformation("F# HTTP trigger function processed a request.")
        let path = req.Url.LocalPath.ToLower()
        let method = req.Method.ToUpper()
        let res = req.CreateResponse()

        match method, path with
        | "GET", "/api/version" -> getVersion res
        | "POST", "/api/untyped-ast" -> getAST log req res
        | "POST", "/api/typed-ast" -> getTypedAST req res
        | _ -> notFound res
