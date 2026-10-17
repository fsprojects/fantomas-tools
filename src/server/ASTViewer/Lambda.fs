module ASTViewer.Lambda

open System.Net
open ASTViewer.GetAST
open HttpConstants

let private mapASTResponse response =
    match response with
    | ASTResponse.Ok json -> HttpStatusCode.OK, HeaderValues.ApplicationJson, json
    | ASTResponse.TooLarge -> HttpStatusCode.RequestEntityTooLarge, HeaderValues.ApplicationText, "File was too large"
    | ASTResponse.InternalError error -> HttpStatusCode.InternalServerError, HeaderValues.ApplicationText, error

/// The functions of this lambda, by the handler name each is deployed with in infrastructure/Program.fs.
let handlers: (string * LambdaRuntime.Handler) list =
    [
        "GetVersion", (fun _ -> task { return HttpStatusCode.OK, HeaderValues.TextPlain, getVersion () })
        "PostUntypedAST", (fun body -> task { return getUntypedAST body |> mapASTResponse })
    ]
