module AWSLambdaExtensions

open System.Collections.Generic

let createHeaders headers =
    Seq.fold
        (fun (acc: Dictionary<string, string>) (key, value) ->
            acc.[key] <- value
            acc)
        (Dictionary<string, string>())
        headers
