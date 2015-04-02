/// Contains a set of extensions and other functions to support common tasks in F# data processing.
module FSharp.Utilities

/// <summary>
/// Similar to F#'s Seq.groupBy function, but removes the key from the grouped values.
/// </summary>
let groupValuesBy (projection:'T->('Key*'Values)) (source:seq<'T>) =
    // Split the elements into (key,values) tuples
    Seq.map projection source
    // Group by key
    |> Seq.groupBy (fun (key,_) -> key)
    // Remove the key from the grouped values 
    |> Seq.map (fun (key,valsIncludingKey) -> key, valsIncludingKey |> Seq.map (fun (_,valsWithoutKey) -> valsWithoutKey))
