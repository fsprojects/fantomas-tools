/// How the lambdas write JSON.
module LambdaJson

open System.Text.Encodings.Web
open System.Text.Json
open Thoth.Json.Core
open Thoth.Json.System.Text.Json

let private options =
    JsonSerializerOptions(Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, MaxDepth = 4096)

/// `Encode.toString 0`, without its limit on depth. Nothing reads the JSON but the client's decoders,
/// so it is never indented: indentation only makes a response larger, by most of its size for a deep
/// Oak. System.Text.Json stops writing at 64 levels, and every node of an Oak takes two, an object
/// and its `children`, so the Oak viewer failed on code nested some 30 levels deep.
let toString (value: IEncodable) : string = Encode.toStringWithOptions options value
