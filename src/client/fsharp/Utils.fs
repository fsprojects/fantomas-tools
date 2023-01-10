module FantomasTools.Client.Utils

let memoizeBy (g: 'a -> 'c) (f: 'a -> 'b) =
    let cache = System.Collections.Generic.Dictionary<_, _>()

    fun x ->
        let key = g x

        if cache.ContainsKey key then
            cache[key]
        else
            let y = f x
            cache.Add(key, y)
            y

let inline memoize f = memoizeBy id f

let inline memoize2 f =
    memoizeBy id (fun (x, y) -> f x y) |> fun f -> fun x y -> f (x, y)
