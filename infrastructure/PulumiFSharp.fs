namespace Pulumi.FSharp

open Pulumi

[<AutoOpen>]
module Ops =

    /// <summary>
    /// Wraps a raw value into an <see cref="Input{'a}" />.
    /// </summary>
    let input<'a> (value: 'a): Input<'a> = Input.op_Implicit value

    /// <summary>
    /// Wraps an <see cref="Output" /> value into an <see cref="Input{'a}}" /> value.
    /// </summary>
    let io<'a> (v: Output<'a>): Input<'a> = Input.op_Implicit v

    /// <summary>
    /// Wraps a collection of items into an <see cref="InputList{'a}}" />.
    /// </summary>
    let inputList<'a> (items: seq<Input<'a>>) =
        let result = new InputList<'a>()
        for item in items do
            result.Add item
        result

    /// <summary>
    /// Wraps a collection of key-value pairs into an <see cref="InputMap{'a}}" />.
    /// </summary>
    let inputMap<'a> (items: seq<string * Input<'a>>) =
        let result = new InputMap<'a>()
        for item in items do
            result.Add item
        result

/// <summary>
/// Pulumi deployment functions.
/// </summary>
module Deployment =
    open System.Collections.Generic

    /// <summary>
    /// Runs a function as a Pulumi <see cref="Deployment" />.
    /// Blocks internally until the provided function completes,
    /// so that this function could be used directly from the main function.
    /// </summary>
    let run (f: unit -> IDictionary<string, obj>) =
        Deployment.RunAsync (fun () -> f())
        |> Async.AwaitTask
        |> Async.RunSynchronously

    /// <summary>
    /// Runs an async function as a Pulumi <see cref="Deployment" />.
    /// Blocks internally until the provided function completes,
    /// so that this function could be used directly from the main function.
    /// </summary>
    let runAsync (f: unit -> Async<IDictionary<string, obj>>) =
        Deployment.RunAsync (fun () -> f() |> Async.StartAsTask)
        |> Async.AwaitTask
        |> Async.RunSynchronously
